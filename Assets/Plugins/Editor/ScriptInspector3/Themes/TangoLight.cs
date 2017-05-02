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
	public class TangoLight
	{
		private static string _themeName = "Tango Light";
		
		static TangoLight()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background				= Color.white,
			text					= TangoColors.aluminium7,
			hyperlinks				= Color.blue,
			
			keywords				= TangoColors.skyblue3,
			constants               = TangoColors.skyblue3,
			strings					= TangoColors.plum2,
			builtInLiterals			= TangoColors.plum1,
			operators				= TangoColors.aluminium7,
			
			referenceTypes			= TangoColors.skyblue2,
			valueTypes				= TangoColors.chameleon3,
			interfaceTypes			= TangoColors.chameleon3,
			enumTypes				= TangoColors.chameleon3,
			delegateTypes			= TangoColors.skyblue2,
			builtInTypes			= Color.clear,
			
			namespaces				= TangoColors.aluminium7,
			methods					= TangoColors.plum3,
			fields					= TangoColors.plum3,
			properties				= TangoColors.plum3,
			events					= TangoColors.plum3,
			
			parameters              = TangoColors.aluminium7,
			variables               = TangoColors.aluminium7,
			typeParameters          = TangoColors.chameleon3,
			enumMembers             = TangoColors.aluminium7,
			
			preprocessor            = TangoColors.orange3,
			defineSymbols           = TangoColors.orange2,
			inactiveCode			= TangoColors.aluminium3,
			comments				= TangoColors.chameleon3,
			xmlDocs					= TangoColors.chameleon3,
			xmlDocsTags				= TangoColors.chameleon3,
			
			lineNumbers				= TangoColors.aluminium4,
			lineNumbersHighlight	= TangoColors.aluminium5,
			lineNumbersBackground	= Color.white,
			fold					= TangoColors.aluminium3,
			
			tooltipBackground       = new Color32(253, 255, 153, 255),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = new Color32(22, 22, 22, 255),
			
			listPopupBackground		= Color.white,
			
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			searchResults			= new Color32(0xff, 0xe2, 0xb9, 0xff),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = TangoColors.aluminium1,
			currentLineInactive     = TangoColors.aluminium1,
			
			preprocessorStyle       = FontStyle.Italic
		};
	}
}
