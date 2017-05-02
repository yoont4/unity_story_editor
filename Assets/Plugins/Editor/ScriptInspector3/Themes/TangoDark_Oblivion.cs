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
	public class TangoDark_Oblivion
	{
		private static string _themeName = "Tango Dark (Oblivion)";
		
		static TangoDark_Oblivion()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background				= TangoColors.aluminium6,
			text					= TangoColors.aluminium2,
			hyperlinks				= TangoColors.butter2,
			
			keywords				= TangoColors.plum1,
			constants               = TangoColors.butter2,
			strings					= TangoColors.butter2,
			builtInLiterals			= TangoColors.orange3,
			operators				= TangoColors.aluminium2,
			
			referenceTypes			= TangoColors.chameleon1,
			valueTypes				= TangoColors.chameleon1,
			interfaceTypes			= TangoColors.chameleon1,
			enumTypes				= TangoColors.chameleon1,
			delegateTypes			= TangoColors.chameleon1,
			builtInTypes			= TangoColors.plum1,
			
			namespaces				= TangoColors.aluminium2,
			methods					= TangoColors.aluminium2,
			fields					= TangoColors.aluminium2,
			properties				= TangoColors.aluminium2,
			events					= TangoColors.aluminium2,
			
			parameters              = TangoColors.aluminium2,
			variables               = TangoColors.aluminium2,
			typeParameters          = TangoColors.chameleon1,
			enumMembers             = TangoColors.aluminium2,
			
			preprocessor            = TangoColors.skyblue1,
			defineSymbols           = TangoColors.skyblue1,
			inactiveCode			= TangoColors.aluminium4,
			comments				= TangoColors.aluminium4,
			xmlDocs					= TangoColors.aluminium4,
			xmlDocsTags				= TangoColors.aluminium4,
			
			lineNumbers				= TangoColors.aluminium5,
			lineNumbersHighlight	= TangoColors.aluminium3,
			lineNumbersBackground	= TangoColors.aluminium7,
			fold					= TangoColors.aluminium3,
			
			activeSelection			= TangoColors.aluminium5,
			passiveSelection		= TangoColors.aluminium5,
			searchResults			= new Color32(0x00, 0x60, 0x60, 0xff),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = TangoColors.aluminium7,
			currentLineInactive     = new Color32(0x11, 0x11, 0x11, 0x80),
			
			referenceHighlight      = new Color32(48, 65, 87, 255),
			referenceModifyHighlight = new Color32(105, 48, 49, 192),
			
			tooltipBackground       = (Color)TangoColors.aluminium7 * 0.5f + (Color)TangoColors.aluminium6 * 0.5f,
			tooltipFrame            = TangoColors.aluminium4,
			tooltipText             = TangoColors.aluminium2,
			
			listPopupBackground		= (Color)TangoColors.aluminium7 * 0.5f + (Color)TangoColors.aluminium6 * 0.5f,
			
			preprocessorStyle       = FontStyle.Italic,
		};
	}
}
