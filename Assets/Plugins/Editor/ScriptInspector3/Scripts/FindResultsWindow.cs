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
using System.Linq;
using System.Threading;

public class FindResultsWindow : EditorWindow
{
	public enum ResultType
	{
		Default,
		RemoveResult,
		WriteReference,
		ReadReference,
		ReadWriteReference,
		MethodOverload,
		OverridingMethod,
		OverriddenMethod,
		VarReference,
		VarTemplateReference,
		UnresolvedSymbol,
		UnresolvedVarSymbol,
		InactiveCode,
		Comment,
		String,
	};
	
	private static GUIStyle evenItemStyle;
	private static GUIStyle oddItemStyle;
	private static GUIStyle pingStyle;
	private static GUIStyle pingStyleReference;
	private static GUIStyle toggleMixedStyle;
	private static GUIStyle boldToolbarButton;
	private static readonly GUILayoutOption[] scrollViewLayoutOptions = new GUILayoutOption[] {
		GUILayout.ExpandWidth(true),
		GUILayout.ExpandHeight(true),
	};
	private static readonly GUILayoutOption[] itemRectLayoutOptions = new GUILayoutOption[] {
		GUILayout.Height(21f),
		GUILayout.ExpandWidth(true),
	};
	private static readonly GUILayoutOption[] toolbarButtonLayoutOptions = new GUILayoutOption[] {
		GUILayout.Height(20f),
		GUILayout.ExpandHeight(true),
	};
	private static readonly GUILayoutOption[] infoTextLayoutOptions = new GUILayoutOption[] {
		GUILayout.Height(20f),
		GUILayout.MinWidth(0f),
	};
	private static readonly GUILayoutOption[] resultsCountTextLayoutOptions = new GUILayoutOption[] { GUILayout.Height(20f) };
	
	[System.Serializable]
	public class SearchOptions
	{
		public string text;
		public string altText1;
		public string altText2;
		public bool matchCase;
		public bool matchWord;
	}
	
	[System.Serializable]
	public class FilteringOptions
	{
		public bool writes;
		public bool reads;
		public bool overloads;
		public bool overridingMethods;
		public bool overriddenMethods;
		public bool vars;
		public bool typeArgumentsInVars;
		public bool unresolved;
		public bool messages;
		public bool inactiveCode;
		public bool strings;
		public bool comments;
		public bool jsScripts;
		public bool booScripts;
		public bool shaders;
		public bool textFiles;
	}
	
	[System.Serializable]
	private class FoundResult
	{
		public string description;
		public string assetGuid;
		public string assetPath;
		public string fileName;
		public int line;
		public int characterIndex;
		public int length;
		public int trimOffset;
		public bool selected;
		public ResultType resultType;
	}
	
	public delegate ResultType ResultTypeValidator(string guid, TextPosition location, int length, ref SymbolDefinition referencedSymbol);
	public delegate bool FileValidator(string guid, FilteringOptions options);
	
	[SerializeField]
	private string infoText;
	[SerializeField]
	private string resultsCountText = "Found 0 results.";
	private System.Action<System.Action<string, string, TextPosition, int>, string, SearchOptions> searchFunction;
	private ResultTypeValidator validateResultFunction;
	private FileValidator validateFileFunction;
	private SymbolDefinition referencedSymbol;
	[SerializeField]
	private List<string> assetGuids = new List<string>();
	[SerializeField]
	private List<string> skippedGuids = new List<string>();
	[SerializeField]
	private SearchOptions searchOptions = new SearchOptions { text = "" };
	[SerializeField]
	private FilteringOptions filteringOptions;
	[SerializeField]
	private bool foundSomeResults;
	
	[System.NonSerialized]
	private int currentAsset;
	[System.NonSerialized]
	private bool repaintOnUpdate;
	[SerializeField]
	private Vector2 scrollPosition;
	
	[SerializeField]
	private int currentItem = 0;
	[System.NonSerialized]
	private bool scrollToCurrentItem;
	[System.NonSerialized]
	private float listViewHeight;
	
	[System.NonSerialized]
	private List<FoundResult> results = new List<FoundResult>();
	[SerializeField]
	private List<FoundResult> flatResults = new List<FoundResult>();
	[SerializeField]
	private int resultsCount;
	[SerializeField]
	private int filesCount;
	private HashSet<string> collapsedPaths = new HashSet<string>();
	//[System.NonSerialized]
	//private ReaderWriterLockSlim resultsLock = new ReaderWriterLockSlim();
	
	[SerializeField]
	private bool keepResults;
	public bool KeepResults {
		get { return keepResults; }
	}
	
	[SerializeField]
	private bool groupByFile;
	private bool GroupByFile {
		get { return groupByFile; }
		set { SISettings.groupFindResultsByFile.Value = groupByFile = value; }
	}
	
	[System.NonSerialized]
	private string replaceText;
	[System.NonSerialized]
	private bool replaceAllAfterSearch;
	[System.NonSerialized]
	private EditorWindow focusAfterReplaceAll;
	
	public new string title {
		get {
			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				return base.title;
			#else
				return titleContent.text;
			#endif
		}
		set {
			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				base.title = value;
			#else
				titleContent.text = value;
			#endif
		}
	}
	
	internal static FindResultsWindow Create(
		string description,
		System.Action<System.Action<string, string, TextPosition, int>, string, SearchOptions> searchFunction,
		string[] assetGuids,
		SearchOptions searchOptions,
		string reuseWindowByTitle)
	{
		var previousResults = (FindResultsWindow[]) Resources.FindObjectsOfTypeAll(typeof(FindResultsWindow));
		var reuseWnd = previousResults.FirstOrDefault(w => !w.KeepResults && w.title == reuseWindowByTitle);
		var wnd = reuseWnd ?? CreateInstance<FindResultsWindow>();
		
		wnd.infoText = description;
		wnd.searchFunction = searchFunction;
		wnd.assetGuids = new List<string>(assetGuids);
		wnd.searchOptions = searchOptions;
		wnd.filteringOptions = new FilteringOptions {
			reads = true,
			writes = true,
			overloads = true,
			overridingMethods = true,
			overriddenMethods = true,
			vars = true,
			typeArgumentsInVars = true,
			unresolved = true,
			inactiveCode = true,
			jsScripts = true,
			booScripts = true,
			shaders = true,
			textFiles = true,
		};
		wnd.groupByFile = SISettings.groupFindResultsByFile;
		
		if (!reuseWnd && reuseWindowByTitle != "Rename")
		{
			var docked = false;
			foreach (var prevWnd in previousResults)
			{
				if (prevWnd != wnd && prevWnd)
				{
					docked = FGCodeWindow.DockNextTo(wnd, prevWnd);
					if (docked)
						break;
				}
			}
			if (!docked)
			{
				var console = FGConsole.FindInstance() ?? (Resources.FindObjectsOfTypeAll(FGConsole.consoleWindowType).FirstOrDefault() as EditorWindow);
				if (console)
					docked = FGCodeWindow.DockNextTo(wnd, console);
			}
		}
		
		wnd.ClearResults();
		
		if (reuseWindowByTitle != "Rename")
			wnd.Show();
		else
			wnd.ShowUtility();
		wnd.Focus();
		
		return wnd;
	}
	
	public void SetFilesValidator(FileValidator validateFileFunction)
	{
		this.validateFileFunction = validateFileFunction;
	}
	
	public void SetResultsValidator(ResultTypeValidator validateResultFunction, SymbolDefinition referencedSymbol)
	{
		this.validateResultFunction = validateResultFunction;
		this.referencedSymbol = referencedSymbol;
		title = "References";
		Repaint();
	}
	
	public void SetReplaceText(string replaceText)
	{
		FGTextBuffer.onInsertedTextAll -= OnBufferModified;
		FGTextBuffer.onInsertedTextAll += OnBufferModified;
		FGTextBuffer.onRemovedTextAll -= OnBufferModified;
		FGTextBuffer.onRemovedTextAll += OnBufferModified;
		this.replaceText = replaceText;
		title = validateResultFunction != null ? "Rename" : "<b>Replace</b>";
		Repaint();
	}
	
	public void ReplaceAllAfterSearchAndSetFocus(EditorWindow toWindow)
	{
		replaceAllAfterSearch = true;
		focusAfterReplaceAll = toWindow;
	}
		
	private void ClearResults()
	{
		resultsCountText = "Found 0 results.";
		foundSomeResults = true;
		
		currentAsset = 0;
		scrollPosition = Vector2.zero;
		
		currentItem = 0;
		listViewHeight = 0f;
		
		skippedGuids.Clear();
		
		results.Clear();
		flatResults.Clear();
		resultsCount = 0;
		filesCount = 0;
		collapsedPaths.Clear();
	}
	
	private void OnEnable()
	{
		if (title != "References")
			title = "Find Results";
		
		UpdateFilters();
		Repaint();
		
		FGTextBuffer.onInsertedLinesAll -= OnInsertedLines;
		FGTextBuffer.onInsertedLinesAll += OnInsertedLines;
		FGTextBuffer.onRemovedLinesAll -= OnRemovedLines;
		FGTextBuffer.onRemovedLinesAll += OnRemovedLines;
	}
	
	private void Unsubscribe()
	{
		assetGuids.Clear();
		skippedGuids.Clear();
		currentAsset = 0;
		searchFunction = null;
		
		FGTextBuffer.onInsertedLinesAll -= OnInsertedLines;
		FGTextBuffer.onRemovedLinesAll -= OnRemovedLines;
		
		FGTextBuffer.onInsertedTextAll -= OnBufferModified;
		FGTextBuffer.onRemovedTextAll -= OnBufferModified;
	}
	
	private void OnDisable()
	{
		Unsubscribe();
	}
	
	private void OnDestroy()
	{
		Unsubscribe();
	}
	
	private void OnBufferModified(string guid, FGTextBuffer.CaretPos from, FGTextBuffer.CaretPos to)
	{
		for (var i = 0; i < flatResults.Count; i++)
		{
			var result = flatResults[i];
			if (result.description != null && guid == result.assetGuid)
			{
				Unsubscribe();
				Close();
				break;
			}
		}
	}
	
	private void OnInsertedLines(string guid, int lineIndex, int numLines)
	{
		for (var i = 0; i < flatResults.Count; i++)
		{
			var result = flatResults[i];
			if (result.description != null && guid == result.assetGuid)
			{
				if (lineIndex <= result.line)
				{
					result.line += numLines;
					repaintOnUpdate = true;
				}
			}
		}
	}
	
	private void OnRemovedLines(string guid, int lineIndex, int numLines)
	{
		for (var i = 0; i < flatResults.Count; i++)
		{
			var result = flatResults[i];
			if (result.description != null && guid == result.assetGuid)
			{
				if (lineIndex <= result.line)
				{
					if (lineIndex + numLines <= result.line)
						result.line -= numLines;
					else
						result.line = lineIndex;
					repaintOnUpdate = true;
				}
			}
		}
	}
	
	private void Update()
	{
		if (repaintOnUpdate)
		{
			repaintOnUpdate = false;
			Repaint();
			return;
		}
		
		if (searchFunction != null && currentAsset < assetGuids.Count)
		{
			var guid = assetGuids[currentAsset++];
			if (validateFileFunction != null)
			{
				while (guid != null && !validateFileFunction(guid, filteringOptions))
				{
					skippedGuids.Add(guid);
					if (currentAsset < assetGuids.Count)
						guid = assetGuids[currentAsset++];
					else
						guid = null;
				}
			}
			if (guid != null)
			{
				searchFunction(AddResult, guid, searchOptions);
				return;
			}
		}
		
		if (searchFunction != null && currentAsset == assetGuids.Count)
		{
			assetGuids.Clear();
			currentAsset = 1;
			
			if (referencedSymbol != null)
				infoText = "References to " + referencedSymbol.FullName;
			else if (title != "References")
				infoText = "Find results for '" + searchOptions.text + "'";
			foundSomeResults = resultsCount > 0;
			if (replaceAllAfterSearch)
			{
				replaceAllAfterSearch = false;
				ReplaceAll();
				if (focusAfterReplaceAll)
					focusAfterReplaceAll.Focus();
				focusAfterReplaceAll = null;
			}
			else
			{
				Repaint();
			}
		}
	}
	
	private void ReplaceAll()
	{
		Unsubscribe();
		
		FGTextBuffer buffer = null;
		string guid = null;
		bool canEditAll = true;
		var canEditBuffers = new HashSet<FGTextBuffer>();
		
		for (var i = flatResults.Count; i --> 0; )
		{
			var result = flatResults[i];
			if (!result.selected)
				continue;
			
			if (result.assetGuid != guid)
			{
				guid = result.assetGuid;
				buffer = FGTextBufferManager.GetBuffer(guid);
				if (buffer.IsLoading)
					buffer.LoadImmediately();
				
				if (FGCodeWindow.CodeWindows.All(x => x.TargetAssetGuid != guid))
				{
					var wnd = FGCodeWindow.OpenAssetInTab(guid);
					if (wnd)
						wnd.OnFirstUpdate();
				}
				if (!buffer.TryEdit())
					canEditAll = false;
				else
					canEditBuffers.Add(buffer);
			}
		}
		
		if (!canEditAll)
		{
			if (!EditorUtility.DisplayDialog("Replace", "Some assets are locked and cannot be edited!", "Continue Anyway", "Cancel"))
			{
				Close();
				return;
			}
		}
		
		buffer = null;
		guid = null;
		var updatedLines = new HashSet<int>();
		for (var i = flatResults.Count; i --> 0; )
		{
			var result = flatResults[i];
			if (!result.selected)
				continue;
			
			if (result.assetGuid != guid)
			{
				if (buffer != null)
				{
					foreach (var line in updatedLines)
						buffer.UpdateHighlighting(line, line);
					buffer.EndEdit();
					FGTextBufferManager.AddBufferToGlobalUndo(buffer);
					updatedLines.Clear();
				}
				
				guid = result.assetGuid;
				buffer = FGTextBufferManager.GetBuffer(guid);
				
				if (buffer != null)
				{
					if (canEditBuffers.Contains(buffer))
						buffer.BeginEdit("*Replace All");
					else
						buffer = null;
				}
			}
			
			if (buffer != null)
			{
				var from = new FGTextBuffer.CaretPos { line = result.line, characterIndex = result.characterIndex };
				var to = new FGTextBuffer.CaretPos { line = result.line, characterIndex = result.characterIndex + result.length };
				var insertAt = buffer.DeleteText(from, to);
				if (replaceText != "")
					buffer.InsertText(insertAt, replaceText);
				
				updatedLines.Add(result.line);
			}
		}
		if (buffer != null)
		{
			foreach (var line in updatedLines)
				buffer.UpdateHighlighting(line, line);
			buffer.EndEdit();
			FGTextBufferManager.AddBufferToGlobalUndo(buffer);
		}
		
		FGTextBufferManager.RecordGlobalUndo();
		Close();
	}
	
	private bool CheckFiltering(ResultType resultType)
	{
		if (referencedSymbol == null)
			return true;
		
		switch (resultType)
		{
		case ResultType.ReadReference:
			return filteringOptions.reads;
		case ResultType.WriteReference:
			return filteringOptions.writes;
		case ResultType.ReadWriteReference:
			return filteringOptions.reads || filteringOptions.writes;
		case ResultType.MethodOverload:
			return filteringOptions.overloads;
		case ResultType.OverridingMethod:
			return filteringOptions.overridingMethods;
		case ResultType.OverriddenMethod:
			return filteringOptions.overriddenMethods;
		case ResultType.VarReference:
			return filteringOptions.vars;
		case ResultType.VarTemplateReference:
			return filteringOptions.typeArgumentsInVars;
		case ResultType.UnresolvedSymbol:
			return filteringOptions.unresolved;
		case ResultType.UnresolvedVarSymbol:
			return filteringOptions.unresolved && (filteringOptions.vars || filteringOptions.typeArgumentsInVars);
		case ResultType.InactiveCode:
			return filteringOptions.inactiveCode;
		case ResultType.String:
			return filteringOptions.strings;
		case ResultType.Comment:
			return filteringOptions.comments;
		}
		
		return true;
	}
	
	private void UpdateFilters()
	{
		if (flatResults.Count > 0)
		{
			resultsCountText = "Found 0 results.";
			foundSomeResults = false;
			
			scrollPosition = Vector2.zero;
			
			currentItem = 0;
			listViewHeight = 0f;
			
			results.Clear();
			resultsCount = 0;
			filesCount = 0;
			collapsedPaths.Clear();
			
			for (var i = 0; i < flatResults.Count; i++)
			{
				var r = flatResults[i];
				
				var resultType = ResultType.Default;
				if (validateResultFunction != null)
				{
					resultType = validateResultFunction(r.assetGuid, new TextPosition(r.line, r.characterIndex), r.length, ref referencedSymbol);
					if (resultType == ResultType.RemoveResult)
						continue;
				}
				
				if (!CheckFiltering(resultType))
					continue;
				
				var lastAssetGuid = results.Count > 0 ? results[results.Count - 1].assetGuid : null;
				if (r.assetGuid != lastAssetGuid)
				{
					++filesCount;
					
					if (GroupByFile)
					{
						results.Add(
							new FoundResult {
							assetGuid = r.assetGuid,
							assetPath = AssetDatabase.GUIDToAssetPath(r.assetGuid),
							selected = true,
						});
					}
				}
				else if (lastAssetGuid == null)
				{
					++filesCount;
				}
				
				results.Add(r);
				++resultsCount;
				resultsCountText = "Found " + resultsCount + " results in " + filesCount + " files.";
			}
			
			if (referencedSymbol != null)
				infoText = "References to " + referencedSymbol.FullName;
			else if (title != "References")
				infoText = "Find results for '" + searchOptions.text + "'";
			foundSomeResults = resultsCount > 0;
		}
		
		if (skippedGuids.Count > 0)
		{
			if (currentAsset > assetGuids.Count)
				currentAsset = assetGuids.Count;
			assetGuids.AddRange(skippedGuids);
			skippedGuids.Clear();
			
			foundSomeResults = true;
		}
	}
	
	private void AddResult(string text, string guid, TextPosition location, int length)
	{
		try
		{
			var resultType = ResultType.Default;
			if (validateResultFunction != null)
			{
				resultType = validateResultFunction(guid, location, length, ref referencedSymbol);
				if (resultType == ResultType.RemoveResult)
					return;
			}
			
			bool visible = CheckFiltering(resultType);
			
			//resultsLock.EnterWriteLock();
			var trim = !char.IsWhiteSpace(text, location.index) && !char.IsWhiteSpace(text, location.index + length - 1);
			
			if (visible)
			{
				var lastAssetGuid = results.Count > 0 ? results[results.Count - 1].assetGuid : null;
				if (guid != lastAssetGuid)
				{
					++filesCount;
					
					if (GroupByFile)
					{
						results.Add(
							new FoundResult {
							assetGuid = guid,
							assetPath = AssetDatabase.GUIDToAssetPath(guid),
							selected = true,
						});
					}
				}
				else if (lastAssetGuid == null)
				{
					++filesCount;
				}
			}
			
			var trimmed = trim ? text.TrimStart() : text;
			var trimOffset = text.Length - trimmed.Length;
			trimmed = trim ? trimmed.TrimEnd() : text;
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var newFindResult =
				new FoundResult {
					description = trimmed,
					assetGuid = guid,
					assetPath = assetPath,
					fileName = System.IO.Path.GetFileName(assetPath),
					line = location.line,
					characterIndex = location.index,
					length = length,
					trimOffset = trimOffset,
					resultType = resultType,
					selected = true,
				};
			if (visible)
			{
				results.Add(newFindResult);
				++resultsCount;
				resultsCountText = "Found " + resultsCount + " results in " + filesCount + " files.";
			}
			flatResults.Add(newFindResult);
		}
		finally
		{
			//resultsLock.ExitWriteLock();
		}
		
		repaintOnUpdate = true;
	}
	
	private void GoToResult(int index)
	{
		if (currentItem >= results.Count)
			return;
		
		var result = results[currentItem];
		FGCodeWindow.OpenAssetInTab(result.assetGuid, result.line, result.characterIndex, result.length);
	}
	
	private void OnGUI()
	{
		if (Event.current.isKey && TabSwitcher.OnGUIGlobal())
			return;
		
		bool needsRepaint = false;
		
		if (Event.current.type == EventType.KeyDown)
		{
			var nextItem = currentItem;
			
			if (Event.current.keyCode == KeyCode.DownArrow)
			{
				++nextItem;
				if (GroupByFile)
				{
					while (nextItem < results.Count && results[nextItem].description != null &&
					  collapsedPaths.Contains(results[nextItem].assetPath))
						++nextItem;
				}
				if (nextItem == results.Count)
					nextItem = currentItem;
			}
			else if (Event.current.keyCode == KeyCode.RightArrow && currentItem < results.Count)
			{
				if (results[currentItem].description == null && collapsedPaths.Contains(results[currentItem].assetPath))
				{
					collapsedPaths.Remove(results[currentItem].assetPath);
					needsRepaint = true;
				}
				else
				{
					++nextItem;
				}
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				--nextItem;
				if (GroupByFile)
				{
					while (nextItem > 0 && results[nextItem].description != null &&
					  collapsedPaths.Contains(results[nextItem].assetPath))
						--nextItem;
				}
			}
			else if (Event.current.keyCode == KeyCode.LeftArrow && currentItem < results.Count)
			{
				if (results[currentItem].description == null)
				{
					collapsedPaths.Add(results[currentItem].assetPath);
					needsRepaint = true;
				}
				else if (GroupByFile)
				{
					while (results[nextItem].description != null)
						--nextItem;
				}
				else
				{
					--nextItem;
				}
			}
			else if (Event.current.keyCode == KeyCode.Home)
			{
				nextItem = 0;
			}
			else if (Event.current.keyCode == KeyCode.End)
			{
				nextItem = results.Count - 1;
				if (GroupByFile)
				{
					while (nextItem > 0 && results[nextItem].description != null &&
					  collapsedPaths.Contains(results[nextItem].assetPath))
						--nextItem;
				}
			}
			
			nextItem = Mathf.Max(0, Mathf.Min(nextItem, results.Count - 1));
			scrollToCurrentItem = scrollToCurrentItem || needsRepaint || nextItem != currentItem;
			needsRepaint = needsRepaint || nextItem != currentItem;
			currentItem = nextItem;
			
			if (Event.current.keyCode == KeyCode.Return ||
				Event.current.keyCode == KeyCode.KeypadEnter ||
				Event.current.keyCode == KeyCode.Space)
			{
				if (currentItem < results.Count)
				{
					Event.current.Use();
					
					if (results[currentItem].description != null)
					{
						if (replaceText == null || Event.current.keyCode != KeyCode.Space)
						{
							GoToResult(currentItem);
						}
						else
						{
							results[currentItem].selected = !results[currentItem].selected;
							needsRepaint = true;
						}
					}
					else
					{
						var path = results[currentItem].assetPath;
						if (collapsedPaths.Contains(path))
							collapsedPaths.Remove(path);
						else
							collapsedPaths.Add(path);
						
						needsRepaint = true;
					}
				}
			}
			else if (needsRepaint)
			{
				Event.current.Use();
			}
			
			if (needsRepaint)
			{
				needsRepaint = false;
				Repaint();
				return;
			}
		}
		
		//if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
		//{
		//	Close();
		//	editor.OwnerWindow.Focus();
		//	return;
		//}
		
		if (evenItemStyle == null)
		{
			evenItemStyle = new GUIStyle("PR Label");
			evenItemStyle.padding.top = 2;
			evenItemStyle.padding.bottom = 2;
			evenItemStyle.padding.left = 2;
			evenItemStyle.margin.right = 0;
			evenItemStyle.fixedHeight = 0;
			evenItemStyle.richText = false;
			evenItemStyle.stretchWidth = true;
			evenItemStyle.wordWrap = false;
			
			oddItemStyle = new GUIStyle(evenItemStyle);
			
			var evenBackground = (GUIStyle) "CN EntryBackEven";
			var oddBackground = (GUIStyle) "CN EntryBackodd";
			evenItemStyle.normal.background = evenBackground.normal.background;
			evenItemStyle.focused.background = evenBackground.normal.background;
			oddItemStyle.normal.background = oddBackground.normal.background;
			oddItemStyle.focused.background = oddBackground.normal.background;
			
			pingStyle = (GUIStyle) "PR Ping";
			pingStyleReference = new GUIStyle(pingStyle);
			pingStyleReference.normal.background = FGTextEditor.LoadEditorResource<Texture2D>("yellowPing.png");
			toggleMixedStyle = (GUIStyle) "ToggleMixed";
			
			boldToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
			boldToolbarButton.fontStyle = FontStyle.Bold;
		}
		
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		var rcToolbar = new Rect(0f, 0f, Screen.width, 20f);
#else
		var rcToolbar = new Rect(0f, 0f, EditorGUIUtility.currentViewWidth, 20f);
#endif
		GUI.Label(rcToolbar, GUIContent.none, EditorStyles.toolbar);
		
		GUILayout.BeginHorizontal();
		
		if (assetGuids.Count > 0)
		{
			if (GUILayout.Toggle(false, "Stop", boldToolbarButton, toolbarButtonLayoutOptions))
			{
				searchFunction = null;
				currentAsset = 1;
				assetGuids.Clear();
				skippedGuids.Clear();
				
				replaceAllAfterSearch = false;
				focusAfterReplaceAll = null;
				if (referencedSymbol != null)
					infoText = "Incomplete references to " + referencedSymbol.FullName;
				else
					infoText = "Incomplete find results for '" + searchOptions.text + "'";
				foundSomeResults = resultsCount > 0;
				replaceText = null;
				
				if (referencedSymbol == null)
					title = "Find Results";
			}
			EditorGUILayout.Space();
		}
		
		GUILayout.Label(infoText, infoTextLayoutOptions);
		EditorGUILayout.Space();
		GUILayout.Label(resultsCountText, resultsCountTextLayoutOptions);
		GUILayout.FlexibleSpace();
		
		if (referencedSymbol != null)
		{
			var newReads = filteringOptions.reads;
			var newWrites = filteringOptions.writes;
			var newOverloads = filteringOptions.overloads;
			var newOverridingMethods = filteringOptions.overridingMethods;
			var newOverriddenMethods = filteringOptions.overriddenMethods;
			var newVars = filteringOptions.vars;
			var newTypeArgsInVars = filteringOptions.typeArgumentsInVars;
			var newUnresolved = filteringOptions.unresolved;
			if (referencedSymbol is InstanceDefinition)
			{
				newReads = GUILayout.Toggle(filteringOptions.reads, "Read", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				newWrites = GUILayout.Toggle(filteringOptions.writes, "Write", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			}
			else if (referencedSymbol.kind == SymbolKind.Method || referencedSymbol.kind == SymbolKind.MethodGroup)
			{
				newReads = GUILayout.Toggle(filteringOptions.reads, "Refs", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				
				newOverloads = GUILayout.Toggle(filteringOptions.overloads, "Overload", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				newOverriddenMethods = GUILayout.Toggle(filteringOptions.overriddenMethods, "Overridden", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				newOverridingMethods = GUILayout.Toggle(filteringOptions.overridingMethods, "Overriding", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			}
			else
			{
				newReads = GUILayout.Toggle(filteringOptions.reads, "Refs", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				
				if (referencedSymbol is TypeDefinitionBase)
				{
					newVars = GUILayout.Toggle(filteringOptions.vars, "var", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
					newTypeArgsInVars = GUILayout.Toggle(filteringOptions.typeArgumentsInVars, "var<T>", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
				}
			}
			newUnresolved = GUILayout.Toggle(filteringOptions.unresolved, "???", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			
			var newInactiveCode = GUILayout.Toggle(filteringOptions.inactiveCode, "#if", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			var newStrings = GUILayout.Toggle(filteringOptions.strings, "String", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			var newComments = GUILayout.Toggle(filteringOptions.comments, "Comment", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			
			if (newReads != filteringOptions.reads ||
				newWrites != filteringOptions.writes ||
				newOverloads != filteringOptions.overloads ||
				newOverridingMethods != filteringOptions.overridingMethods ||
				newOverriddenMethods != filteringOptions.overriddenMethods ||
				newVars != filteringOptions.vars ||
				newTypeArgsInVars != filteringOptions.typeArgumentsInVars ||
				newUnresolved != filteringOptions.unresolved ||
				newInactiveCode != filteringOptions.inactiveCode ||
				newComments != filteringOptions.comments ||
				newStrings != filteringOptions.strings)
			{
				filteringOptions.reads = newReads;
				filteringOptions.writes = newWrites;
				filteringOptions.overloads = newOverloads;
				filteringOptions.overridingMethods = newOverridingMethods;
				filteringOptions.overriddenMethods = newOverriddenMethods;
				filteringOptions.vars = newVars;
				filteringOptions.typeArgumentsInVars = newTypeArgsInVars;
				filteringOptions.unresolved = newUnresolved;
				filteringOptions.inactiveCode = newInactiveCode;
				filteringOptions.comments = newComments;
				filteringOptions.strings = newStrings;
				
				UpdateFilters();
			}
			
			EditorGUILayout.Space();
		}
		
		GUI.enabled = foundSomeResults;
		var newGroupByFile = GUILayout.Toggle(GroupByFile, "Group by file", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
		GUI.enabled = true;
		
		if (replaceText != null)
		{
			EditorGUILayout.Space();
			
			GUI.enabled = assetGuids.Count == 0 && foundSomeResults;
			if (GUILayout.Toggle(false, "Replace all selected", boldToolbarButton, toolbarButtonLayoutOptions))
			{
				ReplaceAll();
			}
			GUI.enabled = true;
		}
		else
		{
			GUI.enabled = foundSomeResults;
			keepResults = GUILayout.Toggle(keepResults, "Keep this results", EditorStyles.toolbarButton, toolbarButtonLayoutOptions);
			GUI.enabled = true;
		}
		
		EditorGUILayout.Space();
		GUILayout.EndHorizontal();
		
		if (newGroupByFile != GroupByFile)
		{
			var currentResult = currentItem < results.Count ? results[currentItem] : null;
			if (currentResult != null && currentResult.description == null)
				currentResult = results[currentItem + 1];
			
			GroupByFile = newGroupByFile;
			UpdateFilters();
			
			if (currentResult != null)
				currentItem = Mathf.Max(0, results.IndexOf(currentResult));
			else
				currentItem = 0;
			
			needsRepaint = true;
			scrollToCurrentItem = true;
		}
		
#if !UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		listViewHeight = Screen.height - rcToolbar.height - 20f;
#else
		listViewHeight = (int)(Screen.height / EditorGUIUtility.pixelsPerPoint - rcToolbar.height - 20f);
#endif
		
		Vector2 scrollToPosition;
		try
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, scrollViewLayoutOptions);
			scrollToPosition = scrollPosition;
			
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			
			//resultsLock.EnterReadLock();
			
			if (!foundSomeResults)
			{
				GUILayout.Label("No Results...");
			}
			else
			{
				var currentPath = "";
				bool isExpanded = true;
				int drawnItemIndex = 0;
				for (var i = 0; i < results.Count; ++i)
				{
					var result = results[i];
					if (result.description != null && !isExpanded)
						continue;
					
					var itemStyle = (drawnItemIndex & 1) == 0 ? evenItemStyle : oddItemStyle;
					
					++drawnItemIndex;
					
					var rc = GUILayoutUtility.GetRect(GUIContent.none, itemStyle, itemRectLayoutOptions);
					rc.xMin = 0f;
					
					if (Event.current.type == EventType.Repaint)
						itemStyle.Draw(rc, GUIContent.none, false, false, i == currentItem, this == focusedWindow);
					
					if (result.description == null)
					{
						currentPath = result.assetPath;
						isExpanded = !collapsedPaths.Contains(currentPath);
						var rcToggle = rc;
						rcToggle.xMax = 18f;
						rcToggle.yMin += 2f;
						bool expand = GUI.Toggle(rcToggle, isExpanded, GUIContent.none, EditorStyles.foldout);
						if (expand != isExpanded)
						{
							currentItem = i;
							if (expand && !isExpanded)
								collapsedPaths.Remove(currentPath);
							else if (!expand && isExpanded)
								collapsedPaths.Add(currentPath);
							needsRepaint = true;
						}
						if (replaceText != null)
						{
							rc.xMin += 18f;
							rcToggle = rc;
							rcToggle.width = 18f;
							rcToggle.yMin += 2f;
							
							var hasSelected = false;
							var hasUnselected = false;
							for (var j = i + 1; j < results.Count && !(hasSelected && hasUnselected); ++j)
							{
								var child = results[j];
								if (child.description == null)
									break;
								if (child.selected)
									hasSelected = true;
								else
									hasUnselected = true;
							}
							var selected = hasSelected;
							if (selected != GUI.Toggle(rcToggle, selected, GUIContent.none, hasSelected && hasUnselected ? toggleMixedStyle : EditorStyles.toggle))
							{
								selected = !selected;
								for (var j = i + 1; j < results.Count; ++j)
								{
									var child = results[j];
									if (child.description == null)
										break;
									child.selected = selected;
								}
							}
						}
					}
					else if (replaceText != null)
					{
						if (GroupByFile)
							rc.xMin += 36f;
						else
							rc.xMin += 4f;
						var rcToggle = rc;
						rcToggle.width = 18f;
						rcToggle.yMin += 2f;
						result.selected = GUI.Toggle(rcToggle, result.selected, GUIContent.none);
					}
					
					if (scrollToCurrentItem && i == currentItem && Event.current.type == EventType.Repaint)
					{
						if (rc.yMin < scrollPosition.y)
						{
							scrollToPosition.y = rc.yMin;
							needsRepaint = true;
						}
						else if (rc.yMax > scrollPosition.y + listViewHeight)
						{
							scrollToPosition.y = rc.yMax - listViewHeight;
							needsRepaint = true;
						}						
					}
					
					if (rc.yMax < scrollPosition.y || rc.yMin > scrollPosition.y + listViewHeight)
					{
						continue;
					}
					
					if (Event.current.type == EventType.MouseDown && rc.Contains(Event.current.mousePosition))
					{
						if (i == currentItem && Event.current.button == 0 && Event.current.clickCount == 2)
						{
							if (result.description == null)
							{
								if (collapsedPaths.Contains(result.assetPath))
									collapsedPaths.Remove(result.assetPath);
								else
									collapsedPaths.Add(result.assetPath);
								
								needsRepaint = true;
							}
							else
							{
								FGCodeWindow.OpenAssetInTab(result.assetGuid, result.line, result.characterIndex, result.length);
							}
						}
						else if (Event.current.button == 1 && result.description == null)
						{
							GenericMenu menu = new GenericMenu();
							menu.AddItem(new GUIContent("Expand All"), false, () => {
								collapsedPaths.Clear();
							});
							menu.AddItem(new GUIContent("Collapse All"), false, () => {
								foreach (var r in results)
									if (r.description == null)
										collapsedPaths.Add(r.assetPath);
							});
							menu.ShowAsContext();
						}
						currentItem = i;
						needsRepaint = true;
						scrollToCurrentItem = true;
						
						Event.current.Use();
					}
					
					GUIContent contentContent;
					int lineInfoLength = 0;
					if (result.description == null)
					{
						contentContent = new GUIContent(result.assetPath, AssetDatabase.GetCachedIcon(result.assetPath));
						rc.xMin += 16f;
					}
					else
					{
						string lineInfo;
						if (GroupByFile)
							lineInfo = (result.line + 1).ToString() + ":   ";
						else
							lineInfo = result.fileName + " (" + (result.line + 1).ToString() + "):   ";
						lineInfoLength = lineInfo.Length;
						contentContent = new GUIContent(lineInfo + result.description);
						if (GroupByFile)
							rc.xMin += 18f;
						else if (replaceText != null)
							rc.xMin += 18f;
						else
							rc.xMin += 2f;
					}
					
					if (Event.current.type == EventType.Repaint)
					{
						if (result.description != null)
						{
							var dotContent = new GUIContent(".");
							var preContent = new GUIContent(contentContent.text.Substring(0, lineInfoLength + result.characterIndex - result.trimOffset) + ".");
							var resultContent = new GUIContent("." + contentContent.text.Substring(0, lineInfoLength + result.characterIndex + result.length - result.trimOffset) + ".");
							var dotSize = itemStyle.CalcSize(dotContent);
							var preSize = itemStyle.CalcSize(preContent); preSize.x -= dotSize.x;
							var resultSize = itemStyle.CalcSize(resultContent); resultSize.x -= dotSize.x * 2f;
							var rcHighlight = new Rect(rc.x + preSize.x - 4f, rc.y + 2f, resultSize.x - preSize.x + 14f, rc.height - 4f);
							GUI.color = new Color(1f, 1f, 1f, 0.4f);
							if (result.resultType == ResultType.ReadReference)
							{
								var oldBgColor = GUI.backgroundColor;
								GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color32(14, 69, 131, 162) : new Color32(0xa0, 0xff, 0xff, 0xff);
								pingStyleReference.Draw(rcHighlight, false, false, false, false);
								GUI.backgroundColor = oldBgColor;
							}
							else if (result.resultType == ResultType.WriteReference || result.resultType == ResultType.ReadWriteReference
								|| result.resultType == ResultType.MethodOverload)
							{
								var oldBgColor = GUI.backgroundColor;
								GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color32(131, 14, 69, 162) : new Color32(0xff, 0xa0, 0xa0, 0xff);
								pingStyleReference.Draw(rcHighlight, false, false, false, false);
								GUI.backgroundColor = oldBgColor;
							}
							else if (result.resultType == ResultType.VarReference || result.resultType == ResultType.VarTemplateReference
								|| result.resultType == ResultType.OverriddenMethod || result.resultType == ResultType.OverridingMethod)
							{
								var oldBgColor = GUI.backgroundColor;
								if (result.resultType == ResultType.VarReference || result.resultType == ResultType.OverriddenMethod)
									GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color32(14, 131, 69, 162) : new Color32(0xa0, 0xff, 0xa0, 0xff);
								else
									GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color32(131, 69, 131, 162) : new Color32(0xff, 0xa0, 0xff, 0xff);
								pingStyleReference.Draw(rcHighlight, false, false, false, false);
								GUI.backgroundColor = oldBgColor;
							}
							else
							{
								pingStyle.Draw(rcHighlight, false, false, false, false);
							}
							GUI.color = Color.white;
						}
						
						GUI.backgroundColor = Color.clear;
						itemStyle.Draw(rc, contentContent, false, false, i == currentItem, this == focusedWindow);
						GUI.backgroundColor = Color.white;
					}					
				}
			}
			
			GUILayout.FlexibleSpace();
		}
		finally
		{
			//resultsLock.ExitReadLock();
			GUILayout.EndScrollView();
		}
		
		if (Event.current.type == EventType.Repaint)
		{
			if (needsRepaint)
			{
				scrollToCurrentItem = false;
				scrollPosition = scrollToPosition;
				Repaint();
			}
			else
			{
				scrollToCurrentItem = false;
			}
		}
	}
}

}
