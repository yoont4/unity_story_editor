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
	public class Xcode
	{
		private static string _themeName = "Xcode"; // Xcode (updated to Xcode 5 by inventor2010)
		
		static Xcode()
		{
			FGTextEditor.AddTheme(_colourTheme, _themeName); // Loaded by default
		}
		
		public static Theme _colourTheme = new Theme
		{
			background				= Xcode5Theme.Background,
			text					= Xcode5Theme.PlainText,
			hyperlinks				= Xcode5Theme.URLs,
			
			keywords				= Xcode5Theme.Keywords,
			constants               = Xcode5Theme.Numbers,
			strings					= Xcode5Theme.Strings,
			builtInLiterals         = Xcode5Theme.Keywords,
			operators               = Color.black,
			
			referenceTypes			= Xcode5Theme.ProjectTypeNames,
			valueTypes				= Xcode5Theme.OtherTypeNames,
			interfaceTypes			= Xcode5Theme.OtherTypeNames,
			enumTypes				= Xcode5Theme.OtherTypeNames,
			delegateTypes			= Xcode5Theme.OtherTypeNames,
			builtInTypes			= Xcode5Theme.Keywords,
			
			namespaces              = Xcode5Theme.Keywords,
			methods                 = Xcode5Theme.ProjectFunctionandMethodNames,
			fields                  = Color.black,
			properties              = Xcode5Theme.OtherInstanceVariablesandGlobals,
			events                  = Xcode5Theme.ProjectFunctionandMethodNames,
			
			parameters              = Color.black,
			variables               = Color.black,
			typeParameters          = new Color32(0x80, 0x46, 0xb0, 0xff),
			enumMembers             = Xcode5Theme.OtherConstants,
			
			preprocessor            = Xcode5Theme.ProjectPreprocessorMacros,
			defineSymbols           = Xcode5Theme.ProjectPreprocessorMacros,
			inactiveCode			= TangoColors.aluminium4,
			comments				= Xcode5Theme.Comments,
			xmlDocs					= new Color32(0x23, 0x97, 0x2d, 0xff),
			xmlDocsTags				= new Color32(0x23, 0x97, 0x2d, 0xff),
			
			lineNumbers				= Xcode5Theme.lineNumbers,
			lineNumbersHighlight	= Xcode5Theme.lineNumbers,
			lineNumbersBackground	= Xcode5Theme.lineNumberBackground,
			fold					= Xcode5Theme.lineNumberBoarder,
			
			activeSelection			= new Color32(164, 205, 255, 0xff),
			passiveSelection		= new Color32(212, 212, 212, 0x7f),
			searchResults			= new Color32(250, 241, 190, 255),
			
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			
			currentLine             = new Color32(213, 213, 241, 255),
			currentLineInactive     = new Color32(228, 228, 228, 255),
			
			referenceHighlight      = new Color32(0xe0, 0xff, 0xff, 0xff),
			referenceModifyHighlight = new Color32(0xff, 0xdd, 0xdd, 0xff),
			
			tooltipBackground       = new Color32(255, 254, 205, 255),
			tooltipFrame            = new Color32(210, 210, 210, 255),
			tooltipText             = new Color32(20, 15, 0, 255),
			
			listPopupBackground		= Xcode5Theme.Background,
		};
		
		//Xcode 5 theme (courtesy of inventor2010)
	//Xcode 5's actual colors and names for its default theme:
		private static class Xcode5Theme
		{
		//Changable Colors in Xcode:
			public static Color32 PlainText								= new Color32(0,0,0,255);
			public static Color32 Comments								= new Color32(0,116,0,255);
			public static Color32 DocumentationComments					= new Color32(0,116,0,255);
			public static Color32 DocumentationCommentKeywords			= new Color32(2,61,16,255);
			public static Color32 Strings								= new Color32(196,26,22,255);
			public static Color32 Characters							= new Color32(28,0,207,255);
			public static Color32 Numbers								= new Color32(28,0,207,255);
			public static Color32 Keywords								= new Color32(170,13,145,255);
			public static Color32 PreprocessorStatements				= new Color32(100,56,32,255);
			public static Color32 URLs									= new Color32(14,14,255,255);
			public static Color32 Attributes							= new Color32(131,108,40,255);
			public static Color32 ProjectClassNames						= new Color32(63,110,116,255);
			public static Color32 ProjectFunctionandMethodNames			= new Color32(38,71,75,255);
			public static Color32 ProjectConstants						= new Color32(38,71,75,255);
			public static Color32 ProjectTypeNames						= new Color32(63,110,116,255);
			public static Color32 ProjectInstanceVariablesandGlobals	= new Color32(63,110,116,255);
			public static Color32 ProjectPreprocessorMacros				= new Color32(100,56,32,255);
			public static Color32 OtherClassNames						= new Color32(92,38,153,255);
			public static Color32 OtherFunctionandMethodNames			= new Color32(46,13,110,255);
			public static Color32 OtherConstants						= new Color32(46,13,110,255);
			public static Color32 OtherTypeNames						= new Color32(92,38,153,255);
			public static Color32 OtherInstanceVariablesandGlobals		= new Color32(92,38,153,255);
			public static Color32 OtherPreprocessorMacros				= new Color32(100,56,32,255);
			
			public static Color32 Background							= new Color32(255,255,255,255);
			public static Color32 Selection								= new Color32(167,202,255,255);
			public static Color32 Cursor								= new Color32(0,0,0,255);
			public static Color32 Invisibles							= new Color32(127,127,127,255);
			
		//Found with "DigitalColor Meter"
			public static Color32 inactiveSelection						= new Color32(212,212,212,255);
			public static Color32 lineNumberBackground					= new Color32(247,247,247,255);
			public static Color32 lineNumbers							= new Color32(146,146,146,255);
			public static Color32 lineNumberBoarder						= new Color32(231,231,231,255);
			public static Color32 searchResults							= new Color32(250, 241, 190,255);
		}
	}
}
