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
	public class VSCodeMonokai
	{
		private static string _themeName = "VS Code Monokai";
		
		static VSCodeMonokai()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background              = new Color32(39, 40, 34, 255),
			text                    = new Color32(248, 248, 242, 255),
			hyperlinks              = new Color32(127, 74, 129, 255),
			
			keywords                = new Color32(249, 38, 114, 255),
			constants               = new Color32(174, 129, 220, 255),
			strings                 = new Color32(230, 219, 116, 255),
			builtInLiterals         = new Color32(174, 129, 255, 255),
			operators               = new Color32(248, 248, 242, 255),
			
			referenceTypes          = new Color32(166, 226, 46, 255),
			valueTypes              = new Color32(102, 217, 239, 255),
			interfaceTypes          = new Color32(102, 217, 239, 255),
			enumTypes               = new Color32(102, 217, 239, 255),
			delegateTypes           = new Color32(102, 217, 239, 255),
			builtInTypes            = new Color32(102, 217, 239, 255),
			
			namespaces              = new Color32(248, 248, 242, 255),
			methods                 = new Color32(166, 226, 46, 255),
			fields                  = new Color32(248, 248, 242, 255),
			properties              = new Color32(248, 248, 242, 255),
			events                  = new Color32(248, 248, 242, 255),
			
			parameters              = new Color32(253, 151, 31, 255),
			variables               = new Color32(248, 248, 242, 255),
			typeParameters          = new Color32(60, 145, 181, 255),
			enumMembers             = new Color32(174, 129, 255, 255),
			
			preprocessor            = new Color32(166, 226, 46, 255),
			defineSymbols           = new Color32(248, 248, 242, 255),
			inactiveCode            = new Color32(117, 113, 94, 255),
			comments                = new Color32(117, 113, 94, 255),
			xmlDocs                 = new Color32(117, 113, 94, 255),
			xmlDocsTags             = new Color32(117, 113, 94, 255),
			
			lineNumbers             = new Color32(90, 90, 90, 255),
			lineNumbersHighlight    = new Color32(90, 90, 90, 255),
			lineNumbersBackground   = new Color32(39, 40, 34, 255),
			fold                    = new Color32(39, 40, 34, 255),
			
			activeSelection			= new Color32(73, 72, 62, 255),
			passiveSelection		= new Color32(73, 72, 62, 255),
			searchResults           = new Color32(0, 96, 96, 128),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color32(62, 61, 49, 255),
			currentLineInactive     = new Color32(50, 50, 41, 255),
			
			referenceHighlight      = new Color32(48, 65, 87, 144),
			referenceModifyHighlight = new Color32(105, 48, 49, 144),
			
			tooltipBackground       = new Color32(45, 45, 48, 255),
			tooltipFrame            = new Color32(85, 85, 85, 255),
			tooltipText             = new Color32(208, 208, 208, 255),
			
			listPopupFrame          = new Color32(85, 85, 85, 255),
			listPopupBackground     = new Color32(45, 45, 48, 255),
		};
	}
}
