/* SCRIPT INSPECTOR 3
 * version 3.0.18, February 2017
 * Copyright © 2012-2017, Flipbook Games
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

namespace ScriptInspector.Themes
{
	using UnityEngine;
	using UnityEditor;
	
	public class Theme
	{
		public Color background = Color.gray;
		public Color text = Color.red;
		public Color hyperlinks = Color.red;
		
		public Color keywords = Color.red;
		public Color constants = Color.red;
		public Color strings = Color.red;
		public Color builtInLiterals = Color.red;
		public Color operators = Color.red;
		public Color punctuators = Color.clear;
		
		public Color referenceTypes = Color.red;
		public Color valueTypes = Color.red;
		public Color interfaceTypes = Color.red;
		public Color enumTypes = Color.red;
		public Color delegateTypes = Color.red;
		public Color builtInTypes = Color.red;
		
		public Color namespaces = Color.red;
		public Color methods = Color.red;
		public Color fields = Color.red;
		public Color properties = Color.red;
		public Color events = Color.red;
		
		public Color parameters = Color.red;
		public Color variables = Color.red;
		public Color typeParameters = Color.red;
		public Color enumMembers = Color.red;
		
		public Color preprocessor = Color.red;
		public Color defineSymbols = Color.red;
		public Color inactiveCode = Color.gray;
		public Color comments = Color.red;
		public Color xmlDocs = Color.red;
		public Color xmlDocsTags = Color.red;
		
		public Color lineNumbers = Color.red;
		public Color lineNumbersHighlight = Color.red;
		public Color lineNumbersBackground = Color.gray;
		public Color fold = Color.red;
		
		public Color activeSelection = new Color32(51, 153, 255, 102);
		public Color passiveSelection = new Color32(191, 205, 219, 102);
		public Color searchResults = Color.yellow;
		
		public Color trackSaved = new Color32(108, 226, 108, 255);
		public Color trackChanged = new Color32(255, 238, 98, 255);
		public Color trackReverted = new Color32(246, 201, 60, 255);
		
		public Color currentLine = Color.green;
		public Color currentLineInactive = Color.magenta;
		
		public Color referenceHighlight = new Color32(0xe0, 0xff, 0xff, 0xff);
		public Color referenceModifyHighlight = new Color32(0xff, 0xdd, 0xdd, 0xff);
		
		public Color tooltipBackground = new Color32(253, 255, 153, 255);
		public Color tooltipFrame = new Color32(128, 128, 128, 255);
		public Color tooltipText = new Color32(22, 22, 22, 255);
		
		public Color listPopupFrame = Color.clear;
		public Color listPopupBackground = Color.gray;
		
		public FontStyle commentsStyle = FontStyle.Normal;
		public FontStyle stringsStyle = FontStyle.Normal;
		public FontStyle keywordsStyle = FontStyle.Normal;
		public FontStyle constantsStyle = FontStyle.Normal;
		public FontStyle typesStyle = FontStyle.Normal;
		public FontStyle namespacesStyle = FontStyle.Normal;
		public FontStyle methodsStyle = FontStyle.Normal;
		public FontStyle fieldsStyle = FontStyle.Normal;
		public FontStyle propertiesStyle = FontStyle.Normal;
		public FontStyle eventsStyle = FontStyle.Normal;
		public FontStyle hyperlinksStyle = FontStyle.Normal;
		public FontStyle preprocessorStyle = FontStyle.Normal;
		public FontStyle defineSymbolsStyle = FontStyle.Normal;
		public FontStyle inactiveCodeStyle = FontStyle.Normal;
		public FontStyle parametersStyle = FontStyle.Normal;
		public FontStyle variablesStyle = FontStyle.Normal;
		public FontStyle typeParametersStyle = FontStyle.Normal;
		public FontStyle enumMembersStyle = FontStyle.Normal;
		
		public override string ToString()
		{
			var index = FGTextEditor.themes.IndexOf(this);
			return index < 0 ? "Unregistered theme" : FGTextEditor.availableThemes[index];
		}
    }
}
