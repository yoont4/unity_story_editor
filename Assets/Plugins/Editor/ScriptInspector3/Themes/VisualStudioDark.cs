/* SCRIPT INSPECTOR 3
 * version 3.0.16, October 2016
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
	
	[InitializeOnLoad]
	public class VisualStudioDark
	{
		private static string _themeName = "Visual Studio Dark"; // Visual Studio Dark (courtesy of Killcycle)
		
		static VisualStudioDark()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background = new Color32(30, 30, 30, 255),
			text = new Color32(218, 218, 218, 255),
			hyperlinks = new Color32(86, 156, 214, 255),
			
			keywords = new Color32(86, 156, 214, 255),
			constants = new Color32(181, 206, 168, 255),
			strings = new Color32(214, 157, 133, 255),
			builtInLiterals = new Color32(86, 156, 214, 255),
			operators = new Color32(180, 180, 180, 255),
			
			referenceTypes = new Color32(78, 201, 176, 255),
			valueTypes = new Color32(78, 201, 176, 255),
			interfaceTypes = new Color32(184, 215, 163, 255),
			enumTypes = new Color32(184, 215, 163, 255),
			delegateTypes = new Color32(78, 201, 176, 255),
			builtInTypes = new Color32(86, 156, 214, 255),
			
			namespaces = new Color32(200, 200, 200, 255),
			methods = new Color32(200, 200, 200, 255),
			fields = new Color32(218, 218, 218, 255),
			properties = new Color32(200, 200, 200, 255),
			events = new Color32(200, 200, 200, 255),
			
			parameters = new Color32(127, 127, 127, 255),
			variables = new Color32(200, 200, 200, 255),
			typeParameters = new Color32(184, 215, 163, 255),
			enumMembers = new Color32(189, 99, 197, 255),
			
			preprocessor = new Color32(155, 155, 155, 255),
			defineSymbols = new Color32(189, 99, 197, 255),
			inactiveCode = new Color32(155, 155, 155, 255),
			comments = new Color32(87, 166, 74, 255),
			xmlDocs = new Color32(87, 166, 74, 255),
			xmlDocsTags = new Color32(87, 166, 74, 255),
			
			lineNumbers = new Color32(43, 145, 175, 255),
			lineNumbersHighlight = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			lineNumbersBackground = new Color32(30, 30, 30, 255),
			fold = new Color32(165, 165, 165, 255),
			
			activeSelection = new Color32(51, 153, 255, 102),
			passiveSelection = new Color32(86, 86, 86, 102),
			searchResults = new Color32(119, 56, 0, 255),
			
			trackSaved = new Color32(87, 116, 48, 255),
			trackChanged = new Color32(239, 242, 132, 255),
			trackReverted = new Color32(95, 149, 250, 255),
			
			currentLine = new Color32(0, 0, 0, 255),
			currentLineInactive = new Color32(42, 42, 42, 255),
			
			referenceHighlight = new Color32(14, 69, 131, 162),
			referenceModifyHighlight = new Color32(131, 14, 69, 162),
			
			tooltipBackground = new Color32(66, 66, 69, 255),
			tooltipText = new Color32(241, 241, 241, 255),
			tooltipFrame = new Color32(102, 102, 102, 255),
			
			listPopupBackground = new Color32(37, 37, 38, 255),
		};
	}
}
