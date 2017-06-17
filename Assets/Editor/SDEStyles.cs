using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: figure out if this is a necessary design component
public enum Style {
	NodeDefault,
	NodeSelected,
	NodeInterruptDefault,
	NodeInterruptSelected,
	ConnectionPointDefault,
	ConnectionPointSelected,
	TextAreaDefault,
	TextBoxDefault,
	TextBoxSelected,
	TextAreaButtonDefault,
	LabelDefault,
	ToggleUpDefault,
	ToggleDownDefault,
}

/*
  SDEStyles is where all predefined styles are kept.
  
  Styles can be loaded in using the LoadStyle() method to choose 
  what GUIStyle they want applied to a GUI object.
*/
public static class SDEStyles {
	private static GUIStyle returnStyle;
	private static bool initialized = false;
	
	// borders
	private static RectOffset nodeBorder;
	private static RectOffset connectionPointBorder;
	private static RectOffset textAreaBorder;
	private static RectOffset toggleBorder;
	
	// padding
	private static RectOffset nodePadding;
	
	// file paths
	private const string NODE_DEFALT = "Assets/Editor/Resources/TestNodeBG.png";
	private const string NODE_HOVER = "Assets/Editor/Resources/TestNodeHoverBG.png";
	private const string NODE_SELECT = "Assets/Editor/Resources/TestNodeSelectedBG.png";
	
	private const string CONNECTIONPOINT_DEFAULT = "Assets/Editor/Resources/TestConnectionPointBG.png";
	private const string CONNECTIONPOINT_HOVER = "Assets/Editor/Resources/TestConnectionPointHoverBG.png";
	private const string CONNECTIONPOINT_SELECT = "Assets/Editor/Resources/TestConnectionPointSelectedBG.png";
	
	private const string TOGGLE_UP_DEFAULT = "Assets/Editor/Resources/TestToggleUpBG.png";
	private const string TOGGLE_UP_HOVER = "Assets/Editor/Resources/TestToggleUpHoverBG.png";
	private const string TOGGLE_DOWN_DEFAULT = "Assets/Editor/Resources/TestToggleDownBG.png";
	private const string TOGGLE_DOWN_HOVER = "Assets/Editor/Resources/TestToggleDownHoverBG.png";
	
	private const string OUTSTREAM_SPLITTER = "Assets/Editor/Resources/OutstreamSplitter.png";
	
	// custom GUI colors
	private static Color32 AlmostWhite;
	private static Color32 LightGray;
	
	// ----------- style references -----------
	// node styles
	public static GUIStyle nodeDefault;
	public static GUIStyle nodeSelected;
	public static GUIStyle nodeSmallDefault;
	public static GUIStyle nodeSmallSelected;
	// connection point styles
	public static GUIStyle connectionPointDefault;
	public static GUIStyle connectionPointSelected;
	// text area styles
	public static GUIStyle textAreaDefault;
	public static GUIStyle textAreaSmallDefault;
	// text box styles
	public static GUIStyle textBoxDefault;
	public static GUIStyle textBoxSelected;
	// text button styles
	public static GUIStyle textButtonDefault;
	// label styles
	public static GUIStyle labelDefault;
	// toggle styles
	public static GUIStyle toggleUpDefault;
	public static GUIStyle toggleDownDefault;
	// ----------- style references -----------
	
	// ---------- texture references ----------
	public static Texture2D outstreamSplitterTexture;
	// ---------- texture references ----------
	
	/*
	  Initializes the variables used to create styles
	*/
	public static void Initialize() {
		// initialize texture borders
		nodeBorder = new RectOffset(5, 5, 5, 5);
		connectionPointBorder = new RectOffset(6, 5, 5, 5);
		textAreaBorder = nodeBorder;
		toggleBorder = nodeBorder;
		
		// initialize padding
		nodePadding = new RectOffset(5, 5, 5, 5);
		
		// initialize colors
		AlmostWhite = new Color32(245, 245, 245, 255);
		LightGray = new Color32(215, 215, 215, 255);
		
		// initialize styles
		nodeDefault = NodeDefaultStyle();
		nodeSelected = NodeSelectedStyle();
		nodeSmallDefault = NodeSmallDefaultStyle();
		nodeSmallSelected = NodeSmallSelectedStyle();
		
		connectionPointDefault = ConnectionPointDefaultStyle();
		connectionPointSelected = ConnectionPointSelectedStyle();
		
		textAreaDefault = TextAreaDefaultStyle();
		textAreaSmallDefault = TextAreaSmallDefaultStyle();
		
		textBoxDefault = TextBoxDefaultStyle();
		textBoxSelected = TextBoxSelectedStyle();
		
		textButtonDefault = TextButtonDefaultStyle();
		
		labelDefault = LabelDefaultStyle();
		
		toggleUpDefault = ToggleUpDefaultStyle();
		toggleDownDefault = ToggleDownDefaultStyle();
		
		// initialize textures
		outstreamSplitterTexture= AssetDatabase.LoadAssetAtPath<Texture2D>(OUTSTREAM_SPLITTER);
		
		initialized = true;
	}
	
	// ------------------------------------------------------------------------------------------ //
	// ------------------------------------ COMPONENT STYLES ------------------------------------ //
	// ------------------------------------------------------------------------------------------ //
	
	private static GUIStyle NodeDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_DEFALT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_HOVER);
		style.border = nodeBorder;
		style.padding = nodePadding;
		
		style.normal.textColor = AlmostWhite;
		style.hover.textColor = LightGray;
		style.fontSize = 14;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle NodeSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_SELECT);
		style.border = nodeBorder;
		style.padding = nodePadding;
		
		style.normal.textColor = Color.white;
		style.fontSize = 14;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle NodeSmallDefaultStyle() {
		GUIStyle style = NodeDefaultStyle();
		style.fontSize = 10;
		return style;
	}
	
	private static GUIStyle NodeSmallSelectedStyle() {
		GUIStyle style = NodeSelectedStyle();
		style.fontSize = 10;
		return style;
	}
	
	private static GUIStyle ConnectionPointDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(CONNECTIONPOINT_DEFAULT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(CONNECTIONPOINT_HOVER);
		style.border = connectionPointBorder;
		return style;
	}
	
	private static GUIStyle ConnectionPointSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(CONNECTIONPOINT_SELECT);
		style.border = connectionPointBorder;
		return style;
	}
	
	private static GUIStyle TextAreaDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = AlmostWhite;
		style.wordWrap = true;
		style.fontSize = 11;
		return style;
	}
	
	private static GUIStyle TextAreaSmallDefaultStyle() {
		GUIStyle style = TextAreaDefaultStyle();
		style.fontSize = 10;
		return style;
	}
	
	private static GUIStyle TextBoxDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_DEFALT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_HOVER);
		style.border = textAreaBorder;
		return style;
	}
	
	private static GUIStyle TextBoxSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_SELECT);
		style.border = textAreaBorder;
		return style;
	}
	
	private static GUIStyle TextButtonDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_DEFALT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_HOVER);
		style.active.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_SELECT);
		style.border = nodeBorder;
		
		style.fontSize = 10;
		style.normal.textColor = AlmostWhite;
		style.hover.textColor = LightGray;
		style.active.textColor = Color.white;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle LabelDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(NODE_DEFALT);
		style.border = nodeBorder;
		
		style.fontSize = 10;
		style.normal.textColor = Color.white;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle ToggleUpDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(TOGGLE_UP_DEFAULT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(TOGGLE_UP_HOVER);
		style.border = connectionPointBorder;
		
		style.normal.textColor = AlmostWhite;
		style.hover.textColor = LightGray;
		
		return style;
	}
	
	private static GUIStyle ToggleDownDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(TOGGLE_DOWN_DEFAULT);
		style.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>(TOGGLE_DOWN_HOVER);
		style.border = connectionPointBorder;
		
		style.normal.textColor = AlmostWhite;
		style.hover.textColor = LightGray;
		
		return style;
	}
}
