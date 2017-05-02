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
	public class Monokai
	{
		private static string _themeName = "Monokai";
		
		static Monokai()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background              = new Color32(39, 40, 34, 255),
			text                    = new Color32(248, 248, 242, 255),
			hyperlinks              = new Color32(127, 74, 129, 255),
			
			keywords                = new Color32(249, 38, 114, 255),
			constants               = new Color32(174, 129, 255, 255),
			strings                 = new Color32(230, 219, 106, 255),
			builtInLiterals         = new Color32(174, 129, 255, 255),
			operators               = new Color32(248, 248, 242, 255),
			
			referenceTypes          = new Color32(102, 218, 236, 255),
			valueTypes              = new Color32(102, 218, 236, 255),
			interfaceTypes          = new Color32(102, 218, 236, 255),
			enumTypes               = new Color32(102, 218, 236, 255),
			delegateTypes           = new Color32(102, 218, 236, 255),
			builtInTypes            = Color.clear,
			
			namespaces              = new Color32(230, 219, 106, 255),
			methods                 = new Color32(166, 226, 46, 255),
			fields                  = new Color32(248, 248, 242, 255),
			properties              = new Color32(248, 248, 242, 255),
			events                  = new Color32(248, 248, 242, 255),
			
			parameters              = new Color32(0xFD, 0x97, 0x1F, 0xFF),
			variables               = new Color32(248, 248, 242, 255),
			typeParameters          = new Color32(0xFD, 0x97, 0x1F, 0xFF),
			enumMembers             = new Color32(174, 129, 255, 255),
			
			preprocessor            = new Color32(166, 226, 46, 255),
			defineSymbols           = new Color32(166, 226, 46, 255),
			inactiveCode            = new Color32(117, 113, 94, 255),
			comments                = new Color32(117, 113, 94, 255),
			xmlDocs                 = new Color32(117, 113, 94, 255),
			xmlDocsTags             = new Color32(117, 113, 94, 255),
			
			lineNumbers             = new Color32(188, 188, 188, 255),
			lineNumbersHighlight    = new Color32(248, 248, 242, 255),
			lineNumbersBackground   = new Color32(39, 40, 34, 255),
			fold                    = new Color32(59, 58, 50, 255),
			
			activeSelection			= new Color32(73, 72, 62, 255),
			passiveSelection		= new Color32(56, 56, 48, 255),
			searchResults           = new Color32(0, 96, 96, 128),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color32(62, 61, 49, 255),
			currentLineInactive     = new Color32(50, 50, 41, 255),
			
			referenceHighlight      = new Color32(48, 65, 87, 144),
			referenceModifyHighlight = new Color32(105, 48, 49, 144),
			
			tooltipBackground       = new Color32(62, 61, 49, 255),
			tooltipFrame            = new Color32(188, 188, 188, 255),
			tooltipText             = new Color32(208, 208, 208, 255),
			
			listPopupFrame          = new Color32(188, 188, 188, 255),
			listPopupBackground     = new Color32(62, 61, 49, 255),
			
			typesStyle              = FontStyle.Italic,
			typeParametersStyle     = FontStyle.Italic,
			parametersStyle         = FontStyle.Italic,
		};
	}
}
