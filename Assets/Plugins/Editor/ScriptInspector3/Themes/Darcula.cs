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
	public class Darcula
	{
		private static string _themeName = "Darcula";
		
		static Darcula()
		{
            FGTextEditor.AddTheme(_colourTheme, _themeName); // Loaded by default
		}
		
		public static Theme _colourTheme = new Theme
		{
			background				= new Color32( 56,  56,  56, 255),
			text					= new Color32(183, 196, 208, 255),
			hyperlinks				= new Color32(0x99, 0xDA, 0xF9, 0xFF),
			
			keywords				= new Color32(215, 139,  54, 255),
			constants               = new Color32(0x68, 0x97, 0xBB, 0xFF),
			strings					= new Color32(0xA5, 0xC2, 0x5C, 255),
			builtInLiterals			= new Color32(215, 139,  54, 255),
			operators				= new Color32(0xE8, 0xE2, 0xB7, 255),
			
			referenceTypes			= new Color32(212, 106, 0, 0xFF),
			valueTypes				= new Color32(212, 106, 0, 0xFF),
			interfaceTypes			= new Color32(0x21, 0x95, 0x98, 0xFF),
			enumTypes				= new Color32(0x76, 0x9A, 0xA5, 0xFF),
			delegateTypes			= new Color32(212, 106, 0, 0xFF),
			builtInTypes			= new Color32(215, 139,  54, 255),
			
			namespaces				= new Color32(183, 196, 208, 255),
			methods					= new Color32(0xC4, 0xB3, 0xA3, 255),
			fields					= new Color32(183, 196, 208, 255),
			properties				= new Color32(183, 196, 208, 255),
			events					= new Color32(183, 196, 208, 255),
			
			parameters              = new Color32(0xC4, 0xB3, 0xA3, 0xFF),
			variables				= new Color32(183, 196, 208, 255),
			typeParameters          = new Color32(0x76, 0x9A, 0xA5, 0xFF),
			enumMembers             = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			
			preprocessor            = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			defineSymbols           = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			inactiveCode			= new Color32(122, 118, 138, 255),
			comments				= new Color32(114, 162, 102, 255),
			xmlDocs					= new Color32(114, 162, 102, 255),
			xmlDocsTags				= new Color32(122, 118, 138, 255),
			
			lineNumbers				= new Color32(0x2B, 0x91, 0xAF, 0xFF),
			lineNumbersHighlight	= new Color32(183, 196, 208, 255),
			lineNumbersBackground	= new Color32( 41,  41,  41, 255),
			fold					= new Color32(0x2B, 0x91, 0xAF, 0xFF),
			
			activeSelection			= new Color32( 68, 134, 244, 80),
			passiveSelection		= new Color32( 72,  72,  72, 255),
			searchResults			= new Color32(0x67, 0x47, 0x07, 0xFF),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238,  98, 255),
			trackReverted           = new Color32(246, 201,  60, 255),
			
			currentLine             = new Color32( 73,  73,  73, 255),
			currentLineInactive     = new Color32( 73,  73,  73, 255),
			
			referenceHighlight      = (Color)TangoColors.skyblue2 * 0.3f + (Color)TangoColors.aluminium6 * 0.7f,
			referenceModifyHighlight = (Color)TangoColors.scarletred1 * 0.3f + (Color)TangoColors.aluminium6 * 0.7f,
			
			tooltipBackground       = new Color32( 64,  64,  64, 255),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = new Color32(183, 196, 208, 255),
			
			listPopupBackground       = new Color32( 48,  48,  48, 255),
			
			preprocessorStyle       = FontStyle.Italic,
			commentsStyle           = FontStyle.Italic,
		};
	}
}
