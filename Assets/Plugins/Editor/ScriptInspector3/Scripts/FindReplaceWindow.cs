/* SCRIPT INSPECTOR 3
 * version 3.0.17, December 2016
 * Copyright © 2012-2016, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

namespace ScriptInspector
{

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum FindReplace_LookIn
{
	WholeProject,
	OpenTabsOnly,
	CurrentTabOnly,
	
	AllGameAssemblies,
	AllEditorAssemblies,
	GameAssemblies,
	EditorAssemblies,
	FirstPassGameAssemblies,
	FirstPassEditorAssemblies,
}

public enum FindReplace_LookFor
{
	AllAssets,
	AllScriptTypes,
	CSharpScripts,
	JSScripts,
	BooScripts,
	Shaders,
	TextAssets,
}

public class FindReplaceWindow : EditorWindow
{
	const float fixedWidth = 320f;
	const float fixedHeightFind = 232f;
	const float fixedHeightReplace = 260f;
	
	private static string[] lookInOptionsAll = new [] {
		"Whole Project",
		"Open Tabs Only",
		"Current Tab Only",
		
		"All Game Assemblies",
		"All Editor Assemblies",
		"Game Assemblies",
		"Editor Assemblies",
		"First-Pass Game Assemblies",
		"First-Pass Editor Assemblies",
	};
	
	private static string[] lookInOptionsNoAssemblies = new [] {
		"Whole Project",
		"Open Tabs Only",
		"Current Tab Only",
	};
	
	private static readonly GUILayoutOption[] historyLayoutOptions = new GUILayoutOption[] { GUILayout.Height(16f), GUILayout.Width(13f) };
	
	[System.NonSerialized]
	private static FindReplaceWindow instance;
	
	private string findText;
	private string replaceText;
	private string initialFindText;
	private FindReplace_LookIn lookInOption;
	private FindReplace_LookFor lookForOption;
	
	[System.NonSerialized]
	private EditorWindow ownerWindow;
	
	private FGTextEditor editor;
	private bool setInitialFocus = true;
	private bool setReplaceFocus = false;
	private bool resetFocus = false;
	
	bool isReplace;
	
	bool matchCase;
	bool matchWholeWord;
	bool listResultsInNewWindow;
	
	private static string[] searchHistory = new string[20];
	private static string[] replaceHistory = new string[20];
	
	private static readonly string[] toolbarTexts = new[] { "Find in Files", "Replace in Files" };
	
	[MenuItem("Window/Script Inspector 3/Find in Files... _%#f", false, 600)]
	public static void ShowFindInFilesWindow()
	{
		if (!SISettings.captureShiftCtrlF)
		{
			if ((FGTextBuffer.activeEditor == null || focusedWindow != FGTextBuffer.activeEditor.OwnerWindow) &&
				!(focusedWindow is FGConsole || focusedWindow is FindResultsWindow))
			{
				if (EditorApplication.ExecuteMenuItem("GameObject/Align With View"))
					return;
			}
		}
		
		EditorApplication.delayCall += () => {
			if (instance != null)
			{
				instance.isReplace = false;
				instance.setInitialFocus = true;
				instance.Repaint();
			}
			else
			{
				Create(false);
			}
		};
	}
	
	[MenuItem("Window/Script Inspector 3/Replace in Files... _%#h", false, 600)]
	public static void ShowReplaceInFilesWindow()
	{
		EditorApplication.delayCall += () => {
			if (instance != null)
			{
				instance.isReplace = true;
				instance.setReplaceFocus = true;
				instance.Repaint();
			}
			else
			{
				Create(true);
			}
		};
	}
	
	public static void Create(bool replace)
	{
		var owner = EditorWindow.focusedWindow;
		var wnd = EditorWindow.GetWindow<FindReplaceWindow>(true);
		
		wnd.ownerWindow = owner;
		
		var editor = FGTextBuffer.activeEditor;
		wnd.editor = editor;
		
		if (editor != null && editor.selectionStartPosition != null &&
			editor.selectionStartPosition.line == editor.caretPosition.line)
		{
			var from = Mathf.Min(editor.selectionStartPosition.characterIndex, editor.caretPosition.characterIndex);
			var to = Mathf.Max(editor.selectionStartPosition.characterIndex, editor.caretPosition.characterIndex);
			wnd.findText = editor.TextBuffer.lines[editor.caretPosition.line].Substring(from, to - from);
		}
		else
		{
			wnd.findText = editor != null ? editor.GetSearchTextFromSelection() : "";
		}
		wnd.initialFindText = wnd.findText;
		wnd.replaceText = null;
		wnd.isReplace = replace;
		
		if (owner != null)
		{
			var center = owner.position.center;
			wnd.position = new Rect(
				(int)(center.x - 0.5f * fixedWidth),
				(int)(center.y - 0.5f * fixedHeightFind),
				fixedWidth,
				fixedHeightFind);
		}
		wnd.ShowAuxWindow();
	}

	private void OnEnable()
	{
		instance = this;
		
		lookInOption = (FindReplace_LookIn) EditorPrefs.GetInt("FlipbookGames.ScriptInspector.FindReplace.LookIn", 0);
		lookForOption = (FindReplace_LookFor) EditorPrefs.GetInt("FlipbookGames.ScriptInspector.FindReplace.LookFor", 0);
		
		matchCase = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.FindReplace.MatchCase", false);
		matchWholeWord = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.FindReplace.MatchWholeWord", false);
		listResultsInNewWindow = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.FindReplace.ListResultsInNewWindow", false);
		
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		title = "Find Text";
#else
		titleContent.text = "Find Text";
#endif
		minSize = new Vector2(fixedWidth, fixedHeightFind);
		maxSize = new Vector2(fixedWidth, fixedHeightFind);
		Repaint();
		
		for (var i = 0; i < searchHistory.Length; i++)
		{
			searchHistory[i] = EditorPrefs.GetString("FlipbookGames.ScriptInspector.SearchHistory_" + i.ToString());
			if (searchHistory[i] == "")
			{
				searchHistory[i] = null;
				break;
			}
		}
		
		for (var i = 0; i < replaceHistory.Length; i++)
		{
			replaceHistory[i] = EditorPrefs.GetString("FlipbookGames.ScriptInspector.ReplaceHistory_" + i.ToString());
			if (replaceHistory[i] == "")
			{
				replaceHistory[i] = null;
				break;
			}
		}
	}
	
	private void OnDisable()
	{
		EditorPrefs.SetInt("FlipbookGames.ScriptInspector.FindReplace.LookIn", (int) lookInOption);
		EditorPrefs.SetInt("FlipbookGames.ScriptInspector.FindReplace.LookFor", (int) lookForOption);
		
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.FindReplace.MatchCase", matchCase);
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.FindReplace.MatchWholeWord", matchWholeWord);
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.FindReplace.ListResultsInNewWindow", listResultsInNewWindow);
		
		instance = null;
		if (editor != null && editor.OwnerWindow)
			editor.OwnerWindow.Focus();
		else if (ownerWindow)
			ownerWindow.Focus();
	}
	
	private void SelectFindHistory(object s)
	{
		var str = (string) s;
		for (var i = searchHistory.Length; i --> 0; )
		{
			if (searchHistory[i] == str)
			{
				while (i --> 0)
					searchHistory[i+1] = searchHistory[i];
				searchHistory[0] = str;
				break;
			}
		}
		findText = str;
		delayCounter = 1;
		EditorApplication.update += ReFocusFindField;
	}
	
	private void SelectReplaceHistory(object s)
	{
		var str = (string) s;
		for (var i = replaceHistory.Length; i --> 0; )
		{
			if (replaceHistory[i] == str)
			{
				while (i --> 0)
					replaceHistory[i+1] = replaceHistory[i];
				replaceHistory[0] = str;
				break;
			}
		}
		replaceText = str;
		delayCounter = 1;
		EditorApplication.update += ReFocusReplaceField;
	}
	
	private int delayCounter;
	private void ReFocusFindField()
	{
		if (delayCounter > 0)
		{
			--delayCounter;
			resetFocus = true;
		}
		else
		{
			EditorApplication.update -= ReFocusFindField;
			setInitialFocus = true;
		}
		Repaint();
	}
	
	private void ReFocusReplaceField()
	{
		if (delayCounter > 0)
		{
			--delayCounter;
			resetFocus = true;
		}
		else
		{
			EditorApplication.update -= ReFocusReplaceField;
			setReplaceFocus = true;
		}
		Repaint();
	}
	
	private void ShowFindHistory()
	{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		string suffix = " _";
#else
		string suffix = "";
#endif
		GenericMenu menu = new GenericMenu();
		if (findText != initialFindText && initialFindText.Trim() != "" && initialFindText.IndexOfAny(new [] {'_', '/'}) < 0)
			menu.AddItem(new GUIContent(initialFindText + suffix), false, SelectFindHistory, initialFindText);
		for (var i = 0; i < searchHistory.Length && searchHistory[i] != null; i++)
			if (findText != searchHistory[i] && searchHistory[i].IndexOfAny(new [] {'_', '/'}) < 0)
				menu.AddItem(new GUIContent(searchHistory[i] + suffix), false, SelectFindHistory, searchHistory[i]);
		if (menu.GetItemCount() > 0)
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			menu.DropDown(new Rect(14f, 58f, Screen.width - 10f, 18f));
#else
			menu.DropDown(new Rect(14f, 58f, EditorGUIUtility.currentViewWidth - 10f, 18f));
#endif
		Event.current.Use();
	}

	private void ShowReplaceHistory()
	{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		string suffix = " _";
#else
		string suffix = "";
#endif
		GenericMenu menu = new GenericMenu();
		if (replaceText != initialFindText && initialFindText.Trim() != "" && initialFindText.IndexOfAny(new [] {'_', '/'}) < 0)
			menu.AddItem(new GUIContent(initialFindText + suffix), false, SelectReplaceHistory, initialFindText);
		for (var i = 0; i < replaceHistory.Length && replaceHistory[i] != null; i++)
			if (replaceText != replaceHistory[i] && replaceHistory[i].IndexOfAny(new [] {'_', '/'}) < 0)
				menu.AddItem(new GUIContent(replaceHistory[i] + suffix), false, SelectReplaceHistory, replaceHistory[i]);
		if (menu.GetItemCount() > 0)
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			menu.DropDown(new Rect(14f, 96f, Screen.width - 10f, 18f));
#else
			menu.DropDown(new Rect(14f, 96f, EditorGUIUtility.currentViewWidth - 10f, 18f));
#endif
		Event.current.Use();
	}
	
	private void OnGUI()
	{
		if (Event.current.type == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				Event.current.Use();
				if (findText != "" && (!isReplace || findText != replaceText))
				{
					if (isReplace)
						ReplaceSelected();
					else
						FindAll();
				}
				return;
			}
			else if (Event.current.character == '\n')
			{
				Event.current.Use();
				return;
			}
			else if (Event.current.keyCode == KeyCode.Escape)
			{
				Event.current.Use();
				Close();
				if (editor != null && editor.OwnerWindow)
					editor.OwnerWindow.Focus();
				else if (ownerWindow)
					ownerWindow.Focus();
				return;
			}
			else if (Event.current.keyCode == KeyCode.DownArrow)
			{
				if (GUI.GetNameOfFocusedControl() == "Find field")
				{
					ShowFindHistory();
				}
				else if (GUI.GetNameOfFocusedControl() == "Replace field")
				{
					ShowReplaceHistory();
				}
			}
		}
		
		// Left margin
		GUILayout.BeginHorizontal();
		GUILayout.Space(10f);
		{
			// Top margin
			GUILayout.BeginVertical();
			GUILayout.Space(10f);
			
			isReplace = 1 == GUILayout.Toolbar(isReplace ? 1 : 0, toolbarTexts);
			GUILayout.Space(10f);
			
			GUILayout.Label("Find what:");
			
			GUILayout.BeginHorizontal();
			
			GUI.SetNextControlName("Find field");
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
			if (setInitialFocus)
				EditorGUI.FocusTextInControl("Find field");
#endif
			try { findText = EditorGUILayout.TextField(findText); } catch {}
			if (setInitialFocus)
				GUI.FocusControl("Find field");
			setInitialFocus = false;
			
			if (GUILayout.Button(GUIContent.none, EditorStyles.toolbarDropDown, historyLayoutOptions))
				ShowFindHistory();
			
			GUILayout.Space(4f);
			GUILayout.EndHorizontal();
			
			if (isReplace)
			{
				GUILayout.Space(10f);
				
				GUILayout.Label("Replace with:");
				
				replaceText = replaceText ?? findText;
				
				GUILayout.BeginHorizontal();
				
				GUI.SetNextControlName("Replace field");
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
				if (setReplaceFocus)
					EditorGUI.FocusTextInControl("Replace field");
#endif
				try { replaceText = EditorGUILayout.TextField(replaceText); } catch {}
				if (setReplaceFocus)
					GUI.FocusControl("Replace field");
				setReplaceFocus = false;
				
				if (GUILayout.Button(GUIContent.none, EditorStyles.toolbarDropDown, historyLayoutOptions))
					ShowReplaceHistory();
				
				GUILayout.Space(4f);
				GUILayout.EndHorizontal();
			}
			
			GUILayout.Space(10f);
			
			GUI.SetNextControlName("Asset types");
			lookForOption = (FindReplace_LookFor) EditorGUILayout.EnumPopup("Asset types:", lookForOption);
			if (resetFocus)
				GUI.FocusControl("Asset types");
			resetFocus = false;
			
			if (lookForOption != FindReplace_LookFor.AllAssets &&
				lookForOption != FindReplace_LookFor.Shaders && lookForOption != FindReplace_LookFor.TextAssets)
			{
				lookInOption = (FindReplace_LookIn) EditorGUILayout.Popup("Search scope:", (int) lookInOption, lookInOptionsAll);
			}
			else
			{
				var option = (int) lookInOption;
				if (lookInOption > FindReplace_LookIn.CurrentTabOnly)
					option = (int) FindReplace_LookIn.WholeProject;
				var newOption = EditorGUILayout.Popup("Search scope:", option, lookInOptionsNoAssemblies);
				if (newOption != option)
					lookInOption = (FindReplace_LookIn) newOption;
			}
			
			GUILayout.Space(10f);
			
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
			matchCase = EditorGUILayout.ToggleLeft(" Match case", matchCase);
			matchWholeWord = EditorGUILayout.ToggleLeft(" Match whole words", matchWholeWord);
			if (!isReplace)
				listResultsInNewWindow = EditorGUILayout.ToggleLeft(" List results in a new window", listResultsInNewWindow);
#else
			matchCase = GUILayout.Toggle(matchCase, " Match case");
			matchWholeWord = GUILayout.Toggle(matchWholeWord, " Match whole words");
			if (!isReplace)
				listResultsInNewWindow = GUILayout.Toggle(listResultsInNewWindow, " List results in new window");
#endif
			
			GUILayout.Space(10f);
			
			GUI.enabled = findText != "" && (!isReplace || findText != replaceText);
			
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (isReplace)
				{
					if (GUILayout.Button("Replace Selected"))
					{
						ReplaceSelected();
					}
					
					/* Uncomment this block to enable the "Replace All" button
					 * ("Replace Selected" will still be default)
					 */
					//GUILayout.Space(6f);
					//if (GUILayout.Button("Replace All"))
					//{
					//	ReplaceAll();
					//}
				}
				else
				{
					if (GUILayout.Button("Find All"))
					{
						FindAll();
					}
				}
				GUILayout.Space(6f);
				
				GUI.enabled = true;
				
				if (GUILayout.Button("Cancel"))
				{
					Close();
					if (editor != null && editor.OwnerWindow)
						editor.OwnerWindow.Focus();
					else if (ownerWindow)
						ownerWindow.Focus();
				}
			}
			GUILayout.EndHorizontal();
		
			GUILayout.Space(20f);
			GUILayout.EndVertical();
		}
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		
		if (isReplace)
		{
			if (position.height != fixedHeightReplace)
			{
				maxSize = new Vector2(fixedWidth, fixedHeightReplace);
				minSize = new Vector2(fixedWidth, fixedHeightReplace);
			}
		}
		else
		{
			if (position.height != fixedHeightFind)
			{
				maxSize = new Vector2(fixedWidth, fixedHeightFind);
				minSize = new Vector2(fixedWidth, fixedHeightFind);
			}
		}
	}
	
	private static List<string> ignoreFileTypes = new List<string> { ".dll", ".a", ".so", ".dylib", ".exe" };
	private static List<string> scriptFileTypes = new List<string> { ".cs", ".js", ".boo" };
	public static List<string> shaderFileTypes = new List<string> { ".shader", ".cg", ".cginc", ".hlsl", ".hlslinc" };
	private static List<string> nonTextFileTypes = new List<string> {
		".dll", ".a", ".so", ".dylib", ".exe", ".cs", ".js", ".boo", ".shader", ".cg", ".cginc", ".hlsl", ".hlslinc"
		};
	
	private void ReplaceAll()
	{
		SaveHistory(replaceHistory, replaceText, "FlipbookGames.ScriptInspector.ReplaceHistory_");
		
		var resultsWindow = FindAll();
		if (resultsWindow)
		{
			resultsWindow.SetReplaceText(replaceText);
			resultsWindow.ReplaceAllAfterSearchAndSetFocus(editor != null && editor.OwnerWindow ? editor.OwnerWindow : ownerWindow);
		}
	}
	
	private void ReplaceSelected()
	{
		SaveHistory(replaceHistory, replaceText, "FlipbookGames.ScriptInspector.ReplaceHistory_");
		
		var resultsWindow = FindAll();
		if (resultsWindow)
			resultsWindow.SetReplaceText(replaceText);
	}
	
	private void SaveHistory(string[] history, string newItem, string preferencePrefix)
	{
		if (newItem.Trim() != "")
		{
			var historyIndex = System.Array.IndexOf(history, newItem);
			if (historyIndex < 0)
				historyIndex = history.Length - 1;
			for (var i = historyIndex; i --> 0; )
				history[i + 1] = history[i];
			history[0] = newItem;
		}
		
		for (var i = 0; i < history.Length; i++)
			if (history[i] != null)
				EditorPrefs.SetString(preferencePrefix + i.ToString(), history[i]);
	}
	
	private FindResultsWindow FindAll()
	{
		SaveHistory(searchHistory, findText, "FlipbookGames.ScriptInspector.SearchHistory_");
		
		Close();
		if (editor != null && editor.OwnerWindow)
			editor.OwnerWindow.Focus();
		else if (ownerWindow)
			ownerWindow.Focus();
		
		return ListAllResults();
	}
	
	public FindResultsWindow ListAllResults()
	{
		string[] allTextAssetGuids;
		var lookInOption = this.lookInOption;
		if (lookForOption == FindReplace_LookFor.AllAssets ||
			lookForOption == FindReplace_LookFor.Shaders ||
			lookForOption == FindReplace_LookFor.TextAssets)
		{
			if (lookInOption > FindReplace_LookIn.CurrentTabOnly)
				lookInOption = FindReplace_LookIn.WholeProject;
		}
		
		if (lookInOption == FindReplace_LookIn.OpenTabsOnly)
		{
			allTextAssetGuids = (from w in FGCodeWindow.CodeWindows select w.TargetAssetGuid).Distinct().ToArray();
		}
		else if (lookInOption == FindReplace_LookIn.CurrentTabOnly)
		{
			allTextAssetGuids = new [] { editor != null ? editor.targetGuid : FGCodeWindow.GetGuidHistory().FirstOrDefault() };
		}
		else if (lookInOption != FindReplace_LookIn.WholeProject &&
			lookForOption != FindReplace_LookFor.AllAssets &&
			lookForOption != FindReplace_LookFor.Shaders &&
			lookForOption != FindReplace_LookFor.TextAssets)
		{
			if (FGFindInFiles.assets != null)
				FGFindInFiles.assets.Clear();
			
			if (lookInOption == FindReplace_LookIn.FirstPassGameAssemblies || lookInOption == FindReplace_LookIn.AllGameAssemblies)
			{
				if (lookForOption == FindReplace_LookFor.CSharpScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.CSharpFirstPass);
				if (lookForOption == FindReplace_LookFor.JSScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.UnityScriptFirstPass);
				if (lookForOption == FindReplace_LookFor.BooScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.BooFirstPass);
			}
			if (lookInOption == FindReplace_LookIn.GameAssemblies || lookInOption == FindReplace_LookIn.AllGameAssemblies)
			{
				if (lookForOption == FindReplace_LookFor.CSharpScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.CSharp);
				if (lookForOption == FindReplace_LookFor.JSScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.UnityScript);
				if (lookForOption == FindReplace_LookFor.BooScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.Boo);
			}
			if (lookInOption == FindReplace_LookIn.FirstPassEditorAssemblies || lookInOption == FindReplace_LookIn.AllEditorAssemblies)
			{
				if (lookForOption == FindReplace_LookFor.CSharpScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass);
				if (lookForOption == FindReplace_LookFor.JSScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass);
				if (lookForOption == FindReplace_LookFor.BooScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.BooEditorFirstPass);
			}
			if (lookInOption == FindReplace_LookIn.EditorAssemblies || lookInOption == FindReplace_LookIn.AllEditorAssemblies)
			{
				if (lookForOption == FindReplace_LookFor.CSharpScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.CSharpEditor);
				if (lookForOption == FindReplace_LookFor.JSScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.UnityScriptEditor);
				if (lookForOption == FindReplace_LookFor.BooScripts || lookForOption == FindReplace_LookFor.AllScriptTypes)
					FGFindInFiles.FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly.BooEditor);
			}
			
			allTextAssetGuids = FGFindInFiles.assets.ToArray();
		}
		else
		{
			allTextAssetGuids = FGFindInFiles.FindAllTextAssets().ToArray();

			IEnumerable<string> realTextAssets = null;
			switch (lookForOption)
			{
			case FindReplace_LookFor.AllAssets:
				realTextAssets =
					from guid in allTextAssetGuids
					where ! ignoreFileTypes.Contains(Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()))
					select guid;
				break;
			case FindReplace_LookFor.AllScriptTypes:
				realTextAssets =
					from guid in allTextAssetGuids
					where scriptFileTypes.Contains(Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()))
					select guid;
				break;
			case FindReplace_LookFor.CSharpScripts:
				realTextAssets =
					from guid in allTextAssetGuids
					where Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()) == ".cs"
					select guid;
				break;
			case FindReplace_LookFor.JSScripts:
				realTextAssets =
					from guid in allTextAssetGuids
					where Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()) == ".js"
					select guid;
				break;
			case FindReplace_LookFor.BooScripts:
				realTextAssets =
					from guid in allTextAssetGuids
					where Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()) == ".boo"
					select guid;
				break;
			case FindReplace_LookFor.Shaders:
				realTextAssets =
					from guid in allTextAssetGuids
					where shaderFileTypes.Contains(Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()))
					select guid;
				break;
			case FindReplace_LookFor.TextAssets:
				realTextAssets =
					from guid in allTextAssetGuids
					where ! nonTextFileTypes.Contains(Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid).ToLowerInvariant()))
					select guid;
				break;
			}
			
			allTextAssetGuids = realTextAssets.ToArray();
		}
		
		if (allTextAssetGuids.Length == 0 || allTextAssetGuids.Length == 1 && allTextAssetGuids[0] == null)
		{
			Debug.LogWarning("No asset matches selected searching scope!");
			return null;
		}
		
		var searchOptions = new FindResultsWindow.SearchOptions {
			text = findText,
			matchCase = matchCase,
			matchWord = matchWholeWord,
		};
		
		FindResultsWindow resultsWindow = FindResultsWindow.Create(
			"Searching for '" + findText + "'...",
			FGFindInFiles.FindAllInSingleFile,
			allTextAssetGuids,
			searchOptions,
			isReplace ? "<b>Replace</b>" : listResultsInNewWindow ? "" : "Find Results");
		return resultsWindow;
	}
}

}
