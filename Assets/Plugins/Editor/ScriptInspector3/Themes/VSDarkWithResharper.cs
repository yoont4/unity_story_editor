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
	public class VSDarkWithResharper
	{
		private static string _themeName = "VS Dark with Resharper"; // Visual Studio Dark with Resharper 9.1 (courtesy of Sarper Soher)
		
		static VSDarkWithResharper()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background = new Color32(0x1E, 0x1E, 0x1E, 0xFF),
			text = new Color32(0xDC, 0xDC, 0xDC, 0XFF),
			hyperlinks = new Color32(0x00, 0x00, 0xFF, 0xFF),
			
			keywords = new Color32(0x56, 0x9C, 0xD6, 0xFF),
			constants = new Color32(0xB5, 0xCE, 0xA8, 0xFF),
			strings = new Color32(0xD6, 0x9D, 0x85, 0xFF),
			builtInLiterals = new Color32(0xDA, 0xDA, 0xDA, 0xFF),
			operators = new Color32(0xB4, 0xB4, 0xB4, 0xFF),
			
			referenceTypes = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			valueTypes = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			interfaceTypes = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			enumTypes = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			delegateTypes = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			builtInTypes = new Color32(0x56, 0x9C, 0xD6, 0xFF),
			
			namespaces = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			methods = new Color32(0x00, 0xFF, 0xFF, 0xFF),
			fields = new Color32(0xEE, 0x82, 0xEE, 0xFF),
			properties = new Color32(0xEE, 0x82, 0xEE, 0xFF),
			events = new Color32(0xDD, 0xA0, 0xDD, 0xFF),
			
			parameters = new Color32(0xDC, 0xDC, 0xDC, 0xFF),
			variables = new Color32(0xDC, 0xDC, 0xDC, 0xFF),
			typeParameters = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			enumMembers = new Color32(0xEE, 0x82, 0xEE, 0xFF),
			
			preprocessor = new Color32(0xDC, 0xDC, 0xDC, 0xFF),
			defineSymbols = new Color32(0xDA, 0xDA, 0xDA, 0xFF),
			inactiveCode = new Color32(0x93, 0xA1, 0xA1, 0xFF),
			comments = new Color32(0x57, 0xA6, 0x4A, 0xFF),
			xmlDocs = new Color32(0x00, 0x64, 0x00, 0xFF),
			xmlDocsTags = new Color32(0x00, 0x64, 0x00, 0xFF),
			
			lineNumbers = new Color32(43, 145, 175, 255),
			lineNumbersHighlight = new Color32(0xAD, 0xD8, 0xE6, 0xFF),
			lineNumbersBackground = new Color32(30, 30, 30, 255),
			fold = new Color32(165, 165, 165, 255),
			
			activeSelection = new Color32(51, 153, 255, 102),
			passiveSelection = new Color32(86, 86, 86, 102),
			searchResults = new Color32(119, 56, 0, 255),
			
			trackSaved = new Color32(0x71, 0x9A, 0x07, 0xFF),
			trackChanged = new Color32(0xB5, 0x89, 0x00, 0xFF),
			trackReverted = new Color32(95, 149, 250, 255),
			
			currentLine = new Color32(0x0F, 0x0F, 0x0F, 0xFF),
			currentLineInactive = new Color32(0x24, 0x24, 0x24, 0xFF),
			
			referenceHighlight = new Color32(72, 61, 139, 144),
			referenceModifyHighlight = new Color32(128, 0, 0, 144),
			
			tooltipBackground = new Color32(66, 66, 69, 255),
			tooltipText = new Color32(241, 241, 241, 255),
			tooltipFrame = new Color32(102, 102, 102, 255),
			
			listPopupBackground = new Color32(37, 37, 38, 255),
		};
	}
}
