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
	public class MDBrown
	{
		private static string _themeName = "MD Brown"; // MD Brown (courtesy of Little Angel)
		
		static MDBrown()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName);
		}
		
		public static Theme _colourTheme = new Theme
		{
			background              = Color.white,
			text                    = Color.black,
			hyperlinks              = Color.blue,                           // Didn't change
			
			keywords                = new Color (0.98f, 0.23f, 0.01f),		// Red
			constants               = new Color (1.0f, 0.14f, 1.0f),
			strings                 = new Color (1.0f, 0.14f, 1.0f),		// Pink
			builtInLiterals         = new Color (0.98f, 0.23f, 0.01f),		// Red
			operators               = Color.black,
			
			referenceTypes          = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			valueTypes              = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			interfaceTypes          = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			enumTypes               = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			delegateTypes           = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			builtInTypes            = new Color (0.98f, 0.23f, 0.01f),		// Red
			
			namespaces              = Color.black,
			methods                 = Color.black,
			fields                  = Color.black,
			properties              = Color.black,
			events                  = Color.black,
			
			parameters              = Color.black,
			variables               = Color.black,
			typeParameters          = new Color (0.58f, 0.04f, 0.0f),		// Dark Red
			enumMembers             = Color.black,
			
			preprocessor            = new Color32(0x33, 0x66, 0x99, 0xff),
			defineSymbols           = new Color32(0x33, 0x66, 0x99, 0xff),
			inactiveCode            = new Color (0.20f, 0.60f, 0.0f),		// Green
			comments                = new Color (0.20f, 0.60f, 0.0f),		// Green
			xmlDocs                 = new Color (0.20f, 0.60f, 0.0f),		// Green
			xmlDocsTags             = new Color (0.20f, 0.60f, 0.0f),		// Green
			
			lineNumbers             = new Color (0.50f, 0.40f, 0.28f),		// Tan, Dark
			lineNumbersHighlight    = new Color (0.25f, 0.20f, 0.14f),		// Tan, Very Dark
			lineNumbersBackground   = new Color (1.0f, 0.80f, 0.56f),		// Tan, Light
			fold                    = new Color (0.20f, 0.60f, 0.0f),		// Green
			
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			searchResults           = new Color32(0xff, 0xe2, 0xb9, 0xff),  // Didn't change
			
			trackSaved              = new Color32(98, 201, 98, 255),
			trackChanged            = new Color32(255, 243, 158, 255),
			trackReverted           = new Color32(236, 175, 50, 255),
			
			currentLine             = new Color32(253, 255, 153, 255),
			currentLineInactive     = new Color32(253, 255, 153, 192),
			
			tooltipBackground       = new Color32(253, 255, 153, 255),
			tooltipFrame            = new Color32(128, 128, 128, 255),
			tooltipText             = new Color32(22, 22, 22, 255),
			
			listPopupBackground		= Color.white,
		};
	}
}
