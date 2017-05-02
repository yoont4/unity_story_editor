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
	public class MDBrownDark
	{
		private static string _themeName = "MD Brown - Dark";
		
		static MDBrownDark()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background              = new Color (0.22f, 0.22f, 0.22f),  // Dark Grey (Pro)
			text                    = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			hyperlinks              = new Color (0.0f, 0.75f, 0.75f),   // Light Blue
			
			keywords                = new Color (1.0f, 0.33f, 0.01f),   // Red for Pro
			constants               = new Color (1.0f, 0.33f, 0.01f),
			strings                 = new Color (0.85f, 0.15f, 0.85f),  // Pink for Pro
			builtInLiterals         = new Color (1.0f, 0.33f, 0.01f),   // Red for Pro
			operators               = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			
			referenceTypes          = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			valueTypes              = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			interfaceTypes          = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			enumTypes               = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			delegateTypes           = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			builtInTypes            = new Color (1.0f, 0.33f, 0.01f),   // Red for Pro
			
			namespaces              = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			methods                 = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			fields                  = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			properties              = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			events                  = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			
			parameters              = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			variables               = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			typeParameters          = new Color (0.9f, 0.15f, 0.05f),    // Dark Red for Pro
			enumMembers             = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			
			preprocessor            = new Color (1.0f, 0.33f, 0.01f),
			defineSymbols           = new Color (1.0f, 0.33f, 0.01f),
			inactiveCode            = new Color (0.20f, 0.60f, 0.0f),   // Green
			comments                = new Color (0.20f, 0.60f, 0.0f),   // Green
			xmlDocs                 = new Color (0.20f, 0.60f, 0.0f),   // Green
			xmlDocsTags             = new Color (0.20f, 0.60f, 0.0f),   // Green
			
			lineNumbers             = new Color (0.25f, 0.20f, 0.14f),  // Tan, Very Dark
			lineNumbersHighlight    = new Color (1.0f, 0.80f, 0.56f),   // Tan, Light
			lineNumbersBackground   = new Color (0.50f, 0.40f, 0.28f),  // Tan, Dark
			fold                    = new Color (0.20f, 0.60f, 0.0f),   // Green
			
			activeSelection			= new Color (0.30f, 0.40f, 0.48f, 0.7f),
			passiveSelection		= new Color (0.30f, 0.40f, 0.48f, 0.4f),
			searchResults           = new Color (0.50f, 0.45f, 0.14f, 0.5f),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color(0.20f, 0.18f, 0.14f),
			currentLineInactive     = new Color(0.25f, 0.20f, 0.14f),
			
			referenceHighlight      = (Color)TangoColors.skyblue2 * 0.3f + (Color)TangoColors.aluminium6 * 0.7f,
			referenceModifyHighlight = (Color)TangoColors.scarletred1 * 0.3f + (Color)TangoColors.aluminium6 * 0.7f,
			
			tooltipBackground       = new Color(0.25f, 0.20f, 0.14f),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = new Color(1.0f, 0.80f, 0.56f),
			
			listPopupBackground     = new Color(0.20f, 0.18f, 0.14f),
		};
	}
}
