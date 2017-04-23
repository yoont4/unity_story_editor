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

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
	
public class OptionsBase
{
	protected const string prefix = "FlipbookGames.ScriptInspector.";
	protected string key { get; private set; }
	protected OptionsBase(string key) { this.key = key; }
	public string ToJson() { return "\"" + prefix + key + "\" = \"" + ToString().Replace("\"", "\\\"") + "\""; }
}

public class BoolOption : OptionsBase
{
	private bool value;
	public bool Value
	{
		get { return value; }
		set {
			if (this.value == value)
				return;
			this.value = value;
			EditorPrefs.SetBool(prefix + key, value);
			FGTextEditor.RepaintAllInstances();
		}
	}
	public BoolOption(string key, bool defaultValue) : base(key)
	{
		value = EditorPrefs.GetBool(prefix + key, defaultValue);
	}
	public bool Toggle() { Value = !Value; return Value; }
	public override string ToString()
	{
		return Value.ToString();
	}
	public static implicit operator bool(BoolOption self) { return self.Value; }
}

public class IntOption : OptionsBase
{
	private int value;
	public int Value
	{
		get { return value; }
		set {
			if (this.value == value)
				return;
			this.value = value;
			EditorPrefs.SetInt(prefix + key, value);
			FGTextEditor.RepaintAllInstances();
		}
	}
	public IntOption(string key, int defaultValue) : base(key)
	{
		value = EditorPrefs.GetInt(prefix + key, defaultValue);
	}
	public override string ToString()
	{
		return Value.ToString();
	}
	public static implicit operator int(IntOption self) { return self.Value; }
}

public class FloatOption : OptionsBase
{
	private float value;
	public float Value
	{
		get { return value; }
		set {
			if (this.value == value)
				return;
			this.value = value;
			EditorPrefs.SetFloat(prefix + key, value);
			FGTextEditor.RepaintAllInstances();
		}
	}
	public FloatOption(string key, float defaultValue) : base(key)
	{
		value = EditorPrefs.GetFloat(prefix + key, defaultValue);
	}
	public override string ToString()
	{
		return Value.ToString();
	}
	public static implicit operator float(FloatOption self) { return self.Value; }
}

public class StringOption : OptionsBase
{
	private string value;
	public string Value
	{
		get { return value; }
		set {
			if (this.value == value)
				return;
			this.value = value;
			EditorPrefs.SetString(prefix + key, value);
			FGTextEditor.RepaintAllInstances();
		}
	}
	public StringOption(string key, string defaultValue) : base(key)
	{
		value = EditorPrefs.GetString(prefix + key, defaultValue);
	}
	public override string ToString()
	{
		return Value.ToString();
	}
	public static implicit operator string(StringOption self) { return self.Value; }
}

public static class SISettings
{
	public static IntOption expandTabTitles = Create("ExpandTabTitles", 0);
	public static BoolOption navToolbarSortByName = Create("NavToolbarSortByName", true);
	public static BoolOption navToolbarGroupByRegion = Create("NavToolbarGroupByRegion", true);
	public static BoolOption navToolbarGroupNonMethods = Create("NavToolbarGroupNonMethods", true);
	public static BoolOption useStandardColorInPopups = Create("UseStdColorsInPopups", false);
	public static BoolOption showThickerCaret = Create("ShowThickerCaret", false);
	public static IntOption autoFocusConsole = Create("AutoFocusConsole", 0);
	
	public static BoolOption handleOpenAssets = Create("HandleOpenAsset", false);
	public static BoolOption dontOpenAssets = Create("dontOpenAsset", false);
	public static BoolOption handleOpeningScripts = Create("HandleOpenFromProject", true);
	public static BoolOption handleOpeningShaders = Create("HandleOpenShaderFromProject", true);
	public static BoolOption handleOpeningText = Create("HandleOpenTextFromProject", true);

	public static BoolOption highlightCurrentLine = Create("HighlightCurrentLine", true);
	public static FloatOption highlightCurrentLineAlpha = Create("HighlightCurrentLineAlpha", 0.5f);
	public static BoolOption frameCurrentLine = Create("FrameCurrentLine", true);
	public static BoolOption showLineNumbersCode = Create("LineNumbersCode", true);
	public static BoolOption showLineNumbersCodeInspector = Create("LineNumbersCodeInspector", false);
	public static BoolOption showLineNumbersText = Create("LineNumbersText", false);
	public static BoolOption showLineNumbersTextInspector = Create("LineNumbersTextInspector", false);
	public static BoolOption trackChangesCode = Create("TrackChangesCode", true);
	public static BoolOption trackChangesCodeInspector = Create("TrackChangesCodeInspector", false);
	public static BoolOption trackChangesText = Create("TrackChangesText", true);
	public static BoolOption trackChangesTextInspector = Create("TrackChangesTextInspector", false);
	public static BoolOption wordWrapCode = Create("WordWrapCode", false);
	public static BoolOption wordWrapCodeInspector = Create("WordWrapCodeInspector", true);
	public static BoolOption wordWrapText = Create("WordWrapText", true);
	public static BoolOption wordWrapTextInspector = Create("WordWrapTextInspector", true);
	public static StringOption editorFont = Create("EditorFont", "");
	public static BoolOption fontHinting = Create("FontHinting", Application.platform != RuntimePlatform.OSXEditor);
	public static IntOption fontSizeDelta = Create("FontSizeDelta", 0);
	public static IntOption fontSizeDeltaInspector = Create("FontSizeDeltaInspector", 0);
	public static BoolOption changeFontSizeUsingWheel = Create("ChangeFontSizeUsingWheel", true);
	public static StringOption themeNameCode = Create("ThemeNameCode",
		EditorGUIUtility.isProSkin ? "Darcula" :
		Application.platform == RuntimePlatform.OSXEditor ? "Xcode" :
		"Visual Studio");
	public static StringOption themeNameText = Create("ThemeNameText", "");
	public static BoolOption autoReloadAssemblies = Create("AutoReloadAssemblies", true);
	public static BoolOption compileOnSave = Create("CompileOnSave", true);
	public static BoolOption cancelReloadOnEdit = Create("CancelReloadOnEdit", true);
	public static BoolOption alwaysKeepInMemory = Create("AlwaysKeepInMemory", false);
	
	//public static BoolOption semanticHighlighting = Create("SemanticHighlighting", true);
	public static BoolOption referenceHighlighting = Create("ReferenceHighlighting", true);
	public static BoolOption keepLastHighlight = Create("KeepLastHighlight", true);
	public static BoolOption highlightWritesInRed = Create("HighlightWritesInRed", true);
	
	public static BoolOption useLocalUnityDocumentation = Create("UseLocalUnityDocumentation", true);
	public static BoolOption copyCutFullLine = Create("CopyCutFullLine", false);
	public static BoolOption smartSemicolonPlacement = Create("SmartSemicolonPlacement", false);
	public static BoolOption loopSearchResults = Create("LoopSearchResults", true);
	public static BoolOption smoothScrolling = Create("smoothScrolling2", true);
	public static BoolOption sortRegionsByName = Create("sortRegionsByName", false);
	public static BoolOption openAutoCompleteOnEscape = Create("OpenAutoCompleteOnEscape", Application.platform == RuntimePlatform.OSXEditor);
	public static BoolOption autoCompleteAggressively = Create("AutoCompleteAggressively", true);
	public static BoolOption captureShiftCtrlF = Create("CaptureShiftCtrlF", true);
	public static BoolOption wordBreak_UseBothModifiers = Create("WordBreak_UseBothModifiers", Application.platform == RuntimePlatform.OSXEditor);
	public static BoolOption wordBreak_StopOnSubwords = Create("WordBreak_StopOnSubwords", Application.platform == RuntimePlatform.OSXEditor);
	public static BoolOption wordBreak_IgnorePunctuations = Create("WordBreak_IgnorePunctuations", Application.platform == RuntimePlatform.OSXEditor);
	public static BoolOption wordBreak_RightArrowStopsAtWordEnd = Create("WordBreak_RightArrowStopsAtWordEnd", Application.platform == RuntimePlatform.OSXEditor);
	
	public static BoolOption magicMethods_insertWithComments = Create("MagicMethods.InsertWithComments", true);
	public static BoolOption magicMethods_openingBraceOnSameLine = Create("MagicMethods.OpeningBraceOnSameLine", false);
	
	public static IntOption tabSize = Create("TabSize", 4);
	public static BoolOption insertSpacesOnTab = Create("InsertSpacesOnTab", false);
	
	public static BoolOption groupFindResultsByFile = Create("GroupFindResultsByFile", true);
	
	public static BoolOption inspectPropertyValues = Create("InspectPropertyValues", false);
	
	private static System.Type preferencesWindowType;
	private static System.Type sectionType;
	private static FieldInfo field_Sections;
	private static FieldInfo field_SelectedSectionIndex;
	private static FieldInfo field_Content;
	private static MethodInfo method_ShowPreferencesWindow;
	
	private static float labelWidth = 200f;
	
	//[MenuItem("Window/Script Inspector 3/Copy Preferences", false, 991)]
	public static void CopyPreferences()
	{
		var sb = new System.Text.StringBuilder();
		sb.Append(expandTabTitles.ToJson()); sb.Append(",\n");
		sb.Append(navToolbarSortByName.ToJson()); sb.Append(",\n");
		sb.Append(navToolbarGroupByRegion.ToJson()); sb.Append(",\n");
		sb.Append(navToolbarGroupNonMethods.ToJson()); sb.Append(",\n");
		sb.Append(useStandardColorInPopups.ToJson()); sb.Append(",\n");
		sb.Append(showThickerCaret.ToJson()); sb.Append(",\n");
		sb.Append(autoFocusConsole.ToJson()); sb.Append(",\n");
		sb.Append(handleOpenAssets.ToJson()); sb.Append(",\n");
		sb.Append(dontOpenAssets.ToJson()); sb.Append(",\n");
		sb.Append(handleOpeningScripts.ToJson()); sb.Append(",\n");
		sb.Append(handleOpeningShaders.ToJson()); sb.Append(",\n");
		sb.Append(handleOpeningText.ToJson()); sb.Append(",\n");
		sb.Append(highlightCurrentLine.ToJson()); sb.Append(",\n");
		sb.Append(highlightCurrentLineAlpha.ToJson()); sb.Append(",\n");
		sb.Append(frameCurrentLine.ToJson()); sb.Append(",\n");
		sb.Append(showLineNumbersCode.ToJson()); sb.Append(",\n");
		sb.Append(showLineNumbersCodeInspector.ToJson()); sb.Append(",\n");
		sb.Append(showLineNumbersText.ToJson()); sb.Append(",\n");
		sb.Append(showLineNumbersTextInspector.ToJson()); sb.Append(",\n");
		sb.Append(trackChangesCode.ToJson()); sb.Append(",\n");
		sb.Append(trackChangesCodeInspector.ToJson()); sb.Append(",\n");
		sb.Append(trackChangesText.ToJson()); sb.Append(",\n");
		sb.Append(trackChangesTextInspector.ToJson()); sb.Append(",\n");
		sb.Append(wordWrapCode.ToJson()); sb.Append(",\n");
		sb.Append(wordWrapCodeInspector.ToJson()); sb.Append(",\n");
		sb.Append(wordWrapText.ToJson()); sb.Append(",\n");
		sb.Append(wordWrapTextInspector.ToJson()); sb.Append(",\n");
		sb.Append(editorFont.ToJson()); sb.Append(",\n");
		sb.Append(fontHinting.ToJson()); sb.Append(",\n");
		sb.Append(fontSizeDelta.ToJson()); sb.Append(",\n");
		sb.Append(fontSizeDeltaInspector.ToJson()); sb.Append(",\n");
		sb.Append(themeNameCode.ToJson()); sb.Append(",\n");
		sb.Append(themeNameText.ToJson()); sb.Append(",\n");
		sb.Append(autoReloadAssemblies.ToJson()); sb.Append(",\n");
		sb.Append(compileOnSave.ToJson()); sb.Append(",\n");
		sb.Append(cancelReloadOnEdit.ToJson()); sb.Append(",\n");
		sb.Append(alwaysKeepInMemory.ToJson()); sb.Append(",\n");
		sb.Append(referenceHighlighting.ToJson()); sb.Append(",\n");
		sb.Append(keepLastHighlight.ToJson()); sb.Append(",\n");
		sb.Append(highlightWritesInRed.ToJson()); sb.Append(",\n");
		sb.Append(useLocalUnityDocumentation.ToJson()); sb.Append(",\n");
		sb.Append(copyCutFullLine.ToJson()); sb.Append(",\n");
		sb.Append(smartSemicolonPlacement.ToJson()); sb.Append(",\n");
		sb.Append(loopSearchResults.ToJson()); sb.Append(",\n");
		sb.Append(smoothScrolling.ToJson()); sb.Append(",\n");
		sb.Append(sortRegionsByName.ToJson()); sb.Append(",\n");
		sb.Append(openAutoCompleteOnEscape.ToJson()); sb.Append(",\n");
		sb.Append(autoCompleteAggressively.ToJson()); sb.Append(",\n");
		sb.Append(captureShiftCtrlF.ToJson()); sb.Append(",\n");
		sb.Append(wordBreak_UseBothModifiers.ToJson()); sb.Append(",\n");
		sb.Append(wordBreak_StopOnSubwords.ToJson()); sb.Append(",\n");
		sb.Append(wordBreak_IgnorePunctuations.ToJson()); sb.Append(",\n");
		sb.Append(wordBreak_RightArrowStopsAtWordEnd.ToJson()); sb.Append(",\n");
		sb.Append(magicMethods_insertWithComments.ToJson()); sb.Append(",\n");
		sb.Append(magicMethods_openingBraceOnSameLine.ToJson()); sb.Append(",\n");
		sb.Append(groupFindResultsByFile.ToJson());
		sb.Append(inspectPropertyValues.ToJson());
		
		EditorGUIUtility.systemCopyBuffer = sb.ToString();
	}
	
	[MenuItem("Window/Script Inspector 3/Preferences...", false, 990)]
	[MenuItem("CONTEXT/MonoScript/Preferences...", false, 192)]
	public static void OpenSIPreferences()
	{
		if (preferencesWindowType == null)
		{
			preferencesWindowType = typeof(Editor).Assembly.GetType("UnityEditor.PreferencesWindow");
			sectionType = typeof(Editor).Assembly.GetType("UnityEditor.PreferencesWindow+Section");
			if (preferencesWindowType != null && sectionType != null)
			{
				field_Sections = preferencesWindowType.GetField("m_Sections", BindingFlags.Instance | BindingFlags.NonPublic);
				field_SelectedSectionIndex = preferencesWindowType.GetField("m_SelectedSectionIndex", BindingFlags.Instance | BindingFlags.NonPublic);
				field_Content = sectionType.GetField("content");
				if (field_Sections != null && field_SelectedSectionIndex != null && field_Content != null)
				{
					method_ShowPreferencesWindow = preferencesWindowType.GetMethod("ShowPreferencesWindow", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
			}
		}
		
		if (method_ShowPreferencesWindow != null)
		{
			method_ShowPreferencesWindow.Invoke(null, null);
			EditorApplication.update -= SwitchToScriptInspectorPage;
			EditorApplication.update += SwitchToScriptInspectorPage;
		}
	}
	
	private static void SwitchToScriptInspectorPage()
	{
		var wnd = EditorWindow.focusedWindow;
		if (!wnd || wnd.GetType() != preferencesWindowType)
			return;
		
		EditorApplication.update -= SwitchToScriptInspectorPage;
		
		var sections = field_Sections.GetValue(wnd) as IList;
		if (sections == null)
			return;
		for (var i = sections.Count; i --> 0; )
		{
			var sectionObject = sections[i];
			var content = field_Content.GetValue(sectionObject) as GUIContent;
			if (content != null && content.text == "Script Inspector")
			{
				field_SelectedSectionIndex.SetValue(wnd, i as object);
				wnd.Repaint();
				return;
			}
		}
	}
	
	private static BoolOption Create(string key, bool defaultValue)
	{
		return new BoolOption(key, defaultValue);
	}
	
	private static IntOption Create(string key, int defaultValue)
	{
		return new IntOption(key, defaultValue);
	}
		
	private static FloatOption Create(string key, float defaultValue)
	{
		return new FloatOption(key, defaultValue);
	}
	
	private static StringOption Create(string key, string defaultValue)
	{
		return new StringOption(key, defaultValue);
	}

	public static void SaveSettings()
	{
		FGTextEditor.RepaintAllInstances();
	}
	
	static readonly GUIContent[] modeToggles =
	{
		new GUIContent("General"),
		new GUIContent("View"),
		new GUIContent("Editor"),
	};
	
	static int selectedMode;
	
	static SISettings()
	{
		selectedMode = EditorPrefs.GetInt("FlipbookGames.ScriptInspector.SettingsMode", 0);
	}
	
	static class Styles
	{
		public static GUIStyle largeButton = "LargeButton";
	}
	
	[PreferenceItem("Script Inspector")]
	static void SettingsGUI()
	{
		EditorGUILayout.Space();
		int newSelectedMode = GUILayout.Toolbar(selectedMode, modeToggles, Styles.largeButton, noLayoutOptions);
		if (newSelectedMode != selectedMode)
		{
			selectedMode = newSelectedMode;
			EditorPrefs.SetInt("FlipbookGames.ScriptInspector.SettingsMode", selectedMode);
		}
		
		EditorGUILayout.Space();
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_3_5
		EditorGUIUtility.LookLikeControls();
#endif
		
		switch (selectedMode)
		{
		case 0:
			GeneralSettings();
			break;
		case 1:
			ViewSettings();
			break;
		case 2:
			EditorSettings();
			break;
		}
	}
	
	static void GeneralSettings()
	{
		labelWidth = 200f;
		
		GUILayout.Label("Use Si3 instead of the external IDE", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Always Open in Script Inspector", handleOpenAssets);
		if (handleOpenAssets)
			dontOpenAssets.Value = false;
		Draw("Always Open in External IDE", dontOpenAssets);
		if (dontOpenAssets)
			handleOpenAssets.Value = false;
		EditorGUILayout.Space();
		
		bool enable = !handleOpenAssets && !dontOpenAssets;
		
		GUILayout.Label("Open on double-click", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Scripts", enable ? handleOpeningScripts : handleOpenAssets, enable);
		Draw("Shaders", enable ? handleOpeningShaders : handleOpenAssets, enable);
		Draw("Text Assets", enable ? handleOpeningText : handleOpenAssets, enable);
		EditorGUILayout.Space();
		
		GUILayout.Label("Si3 tabs titles", EditorStyles.boldLabel, noLayoutOptions);
		expandTabTitles.Value = EditorGUILayout.Toggle("Expand on Mouse-Over", expandTabTitles.Value == 1, noLayoutOptions) ? 1 : 0;
	}
	
	static readonly GUILayoutOption[] noLayoutOptions = {};
	static readonly GUILayoutOption[] dontExpandWidth = { GUILayout.ExpandWidth(false) };
	static Vector2 viewScrollPosition;
	static void ViewSettings()
	{
		bool isOSX = Application.platform == RuntimePlatform.OSXEditor;
		
		labelWidth = 250f;
		
		viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, noLayoutOptions);
		
		Draw("Show thicker caret", showThickerCaret);
		if (isOSX)
			Draw("Use Cmd-MouseWheel to change font size", changeFontSizeUsingWheel);
		else
			Draw("Use Ctrl+MouseWheel to change font size", changeFontSizeUsingWheel);
		EditorGUILayout.Space();

		GUILayout.Label("Current Line Highlighting", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Frame Current Line", frameCurrentLine);
		Draw("Highlight Current Line", highlightCurrentLine);
		EditorGUILayout.Space();

		labelWidth = 200f;
		
		GUILayout.BeginHorizontal(noLayoutOptions);
		GUILayout.Label("Word Wrap", EditorStyles.boldLabel, noLayoutOptions);
		GUILayout.FlexibleSpace();
		GUILayout.Label("Si3 Tabs", EditorStyles.boldLabel, dontExpandWidth);
		GUILayout.Space(16);
		GUILayout.Label("Inspector Tab", EditorStyles.boldLabel, dontExpandWidth);
		GUILayout.EndHorizontal();
		Draw2("Scripts & Shaders", wordWrapCode, wordWrapCodeInspector);
		Draw2("Text Assets", wordWrapText, wordWrapTextInspector);
		EditorGUILayout.Space();
		
		GUILayout.Label("Show Line Numbers", EditorStyles.boldLabel, noLayoutOptions);
		Draw2("Scripts & Shaders", showLineNumbersCode, showLineNumbersCodeInspector);
		Draw2("Text Assets", showLineNumbersText, showLineNumbersTextInspector);
		EditorGUILayout.Space();
		
		GUILayout.Label("Track Changes", EditorStyles.boldLabel, noLayoutOptions);
		Draw2("Scripts & Shaders", trackChangesCode, trackChangesCodeInspector);
		Draw2("Text Assets", trackChangesText, trackChangesTextInspector);
		EditorGUILayout.Space();
		
		labelWidth = 250f;
		
		GUILayout.Label("C# Code Navigation Toolbar", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Order Symbols by Name", navToolbarSortByName);
		Draw("Group Symbols by #region", navToolbarGroupByRegion);
		//Draw("Group Non-Method Members", navToolbarGroupNonMethods);
		EditorGUILayout.Space();
		
		GUILayout.Label("C# Code", EditorStyles.boldLabel, noLayoutOptions);
		//Draw("Semantic Highlighting", semanticHighlighting);
		Draw("Use Neutral Colors in Popups", useStandardColorInPopups);
		Draw("Reference Highlighting", referenceHighlighting);
		Draw(".  Keep Last Highlighted Symbol", keepLastHighlight, referenceHighlighting);
		Draw(".  Highlight Writes in Red", highlightWritesInRed, referenceHighlighting);
		Draw("Inspect Values of Properties", inspectPropertyValues);
		EditorGUILayout.HelpBox("Inspecting values of properties requires executing their getters code.\n\n" +
			"Only enable if executing that may not have some unwanted side-effects!\n\n" +
			"Alternatively disable inspecting values only on properties with side-effects by decorating them with DebuggerBrowsableAttribute",
			inspectPropertyValues ? MessageType.Warning : MessageType.Info, true);
		
		EditorGUILayout.Space();
		EditorGUILayout.EndScrollView();
	}
	
	static Vector2 editorScrollPosition;
	static void EditorSettings()
	{
		bool isOSX = Application.platform == RuntimePlatform.OSXEditor;
		
		editorScrollPosition = GUILayout.BeginScrollView(editorScrollPosition, noLayoutOptions);
		labelWidth = 250f;
		
		GUILayout.Label("Saving Scripts", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Compile on Save", compileOnSave, true);
		if (compileOnSave && !isOSX)
		{
			Draw("Auto Reload Assemblies", autoReloadAssemblies, true);
		}
		else
		{
			bool saved = autoReloadAssemblies;
			autoReloadAssemblies.Value = true;
			Draw("Auto Reload Assemblies", autoReloadAssemblies, false);
			autoReloadAssemblies.Value = saved || isOSX;
		}
		//		Draw("... Unless Editing Continues", cancelReloadOnEdit, compileOnSave && autoReloadAssemblies);
		cancelReloadOnEdit.Value = false;
		
		var ctrlR = isOSX ? "Cmd-Alt-R" : "Ctrl+R";
		if (!compileOnSave)
		{
			EditorGUILayout.HelpBox(
				@"Saving will NOT recompile assemblies automatically.

Compile and reload assemblies with " + ctrlR + " or with a 'double-save'.",
				MessageType.None, true);
		}
		else if (autoReloadAssemblies)
		{
			EditorGUILayout.HelpBox(cancelReloadOnEdit ?
			"Saving will recompile and reload assemblies automatically if you don't edit scripts while they are compiling.\n\nIf you edit scripts while compiling then you can reload previously compiled assemblies with " + ctrlR + " or save the changes." :
				"Saving will recompile and reload assemblies automatically.",
				MessageType.None, true);
		}
		else
		{
			EditorGUILayout.HelpBox(
			"Saving will recompile assemblies in background.\n\nReload them with " + ctrlR + " or with a 'double-save'.",
				MessageType.None, true);
		}
		Draw("Don't ask to save, just 'Keep in Memory'", alwaysKeepInMemory);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Editor Keyboard", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Show Auto-Complete on 'Esc' key", openAutoCompleteOnEscape);
		Draw("Auto-Complete aggressively", autoCompleteAggressively);
		Draw("Handle Shift+Ctrl+F globally", captureShiftCtrlF);
		Draw("Copy/Cut full line if no selection", copyCutFullLine);
		Draw("Smart semicolon placement", smartSemicolonPlacement);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Word Break Mode", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Use both modifiers (Ctrl & Alt)", wordBreak_UseBothModifiers);
		if (wordBreak_UseBothModifiers)
			Draw(isOSX ? "Ctrl stops on subwords" : "Alt stops on subwords", wordBreak_UseBothModifiers, false);
		else
			Draw(isOSX ? "Stop on subwords with Alt" : "Stop on subwords with Ctrl", wordBreak_StopOnSubwords);
		Draw("Ignore puntuation marks", wordBreak_IgnorePunctuations);
		Draw("Right arrow stops at word end", wordBreak_RightArrowStopsAtWordEnd);
		
		GUILayout.BeginHorizontal(noLayoutOptions);
		if (GUILayout.Button("Windows Defaults", noLayoutOptions))
		{
			wordBreak_UseBothModifiers.Value = false;
			wordBreak_StopOnSubwords.Value = false;
			wordBreak_IgnorePunctuations.Value = false;
			wordBreak_RightArrowStopsAtWordEnd.Value = false;
		}
		GUILayout.Space(2f);
		if (GUILayout.Button("OS X Defaults", noLayoutOptions))
		{
			wordBreak_UseBothModifiers.Value = true;
			wordBreak_StopOnSubwords.Value = true;
			wordBreak_IgnorePunctuations.Value = true;
			wordBreak_RightArrowStopsAtWordEnd.Value = true;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Tabs", EditorStyles.boldLabel, noLayoutOptions);
		labelWidth = 100f;
		Draw("Tab size", tabSize, 1, 8);
		labelWidth = 250f;
		//Draw("Insert spaces on Tab key", insertSpacesOnTab);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("Unity Magic Methods", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Insert with comments", magicMethods_insertWithComments);
		Draw("Opening brace on same line", magicMethods_openingBraceOnSameLine);
		
		EditorGUILayout.Space();
		
		GUILayout.Label("More options...", EditorStyles.boldLabel, noLayoutOptions);
		Draw("Search results looping", loopSearchResults);
		Draw("Smooth scrolling", smoothScrolling);
		Draw("Use Local Unity Documentation", useLocalUnityDocumentation);
		
		EditorGUILayout.Space();
		EditorGUILayout.EndScrollView();
	}

	static void Draw2(string label, BoolOption option1, BoolOption option2)
	{
		GUILayout.BeginHorizontal(noLayoutOptions);
		Draw(label, option1);
		Draw(null, option2);
		GUILayout.EndHorizontal();
	}
	
	static bool Draw(string label, BoolOption option, bool enabled = true, GUIStyle style = null)
	{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_3_5
		EditorGUIUtility.LookLikeControls(label == null ? 35f : labelWidth);
#else
		EditorGUIUtility.labelWidth = label == null ? 35f : labelWidth;
#endif
		var old = GUI.enabled;
		if (!enabled)
			GUI.enabled = false;
		if (style != null)
			option.Value = EditorGUILayout.Toggle(label, option.Value, style, noLayoutOptions);
		else
			option.Value = EditorGUILayout.Toggle(label, option.Value, noLayoutOptions);
		if (!enabled)
			GUI.enabled = old;
		return option;
	}
	
	static int Draw(string label, IntOption option, int min, int max, bool enabled = true)
	{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_3_5
		EditorGUIUtility.LookLikeControls(label == null ? 35f : labelWidth);
#else
		EditorGUIUtility.labelWidth = label == null ? 35f : labelWidth;
#endif
		var old = GUI.enabled;
		if (!enabled)
			GUI.enabled = false;
		option.Value = EditorGUILayout.IntSlider(label, option.Value, min, max, noLayoutOptions);
		if (!enabled)
			GUI.enabled = old;
		return option;
	}
}

}
