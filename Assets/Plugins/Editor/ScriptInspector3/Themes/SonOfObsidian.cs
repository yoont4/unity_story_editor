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
	public class SonOfObsidian
	{
		private static string _themeName = "Son of Obsidian";
		
		static SonOfObsidian()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background              = new Color32(0x22, 0x28, 0x2A, 0xFF),
			text                    = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			hyperlinks              = new Color32(0x99, 0xDA, 0xF9, 0xFF),
			
			keywords                = new Color32(0x93, 0xC7, 0x63, 0xFF),
			constants               = new Color32(0xFF, 0xCD, 0x22, 0xFF),
			strings                 = new Color32(0xEC, 0x76, 0x00, 0xFF),
			builtInLiterals         = new Color32(0x93, 0xC7, 0x63, 0xFF),
			operators               = new Color32(0xE8, 0xE2, 0xB7, 0xFF),
			
			referenceTypes          = new Color32(0x8C, 0x8C, 0xB4, 0xFF),
			valueTypes              = new Color32(0x8C, 0x8C, 0xB4, 0xFF),
			interfaceTypes          = new Color32(0x67, 0x8C, 0xB1, 0xFF),
			enumTypes               = new Color32(0x67, 0x8C, 0xB1, 0xFF),
			delegateTypes           = new Color32(0x67, 0x8C, 0xB1, 0xFF),
			builtInTypes            = new Color32(0x93, 0xC7, 0x63, 0xFF),
			
			namespaces              = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			methods                 = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			fields                  = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			properties              = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			events                  = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			
			parameters              = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			variables               = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			typeParameters          = new Color32(0x67, 0x8C, 0xB1, 0xFF),
			enumMembers             = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			
			preprocessor            = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			defineSymbols           = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			inactiveCode            = new Color32(0x61, 0x61, 0x61, 0xFF),
			comments                = new Color32(0x66, 0x74, 0x7B, 0xFF),
			xmlDocs                 = new Color32(0x99, 0xA3, 0x8A, 0xFF),
			xmlDocsTags             = new Color32(0x80, 0x80, 0x80, 0xFF),
			
			lineNumbers             = new Color32(0x3F, 0x4E, 0x49, 0xFF),
			lineNumbersHighlight    = new Color32(0x7E, 0x9D, 0x92, 0xFF),
			lineNumbersBackground   = new Color32(0x29, 0x31, 0x34, 0xFF),
			fold                    = new Color32(0x29, 0x31, 0x34, 0xFF),
			
			activeSelection			= new Color32(0x96, 0xAD, 0xB2, 0x44),
			passiveSelection		= new Color32(0x17, 0x1B, 0x1C, 0xFF),
			searchResults           = new Color32(0x57, 0x4E, 0x40, 0xFF),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color32(0x31, 0x3A, 0x3E, 0xFF),
			currentLineInactive     = new Color32(0x29, 0x31, 0x36, 0xFF),
			
			referenceHighlight      = new Color32(48, 65, 87, 144),
			referenceModifyHighlight = new Color32(105, 48, 49, 144),
			
			tooltipBackground       = new Color32(0x29, 0x31, 0x34, 0xFF),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = TangoColors.aluminium2,
			
			listPopupBackground		= new Color32(0x17, 0x1B, 0x1C, 0xFF),
			
			//typesStyle              = FontStyle.Italic
		};
	}
}
