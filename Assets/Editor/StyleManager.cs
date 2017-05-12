using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
	LabelDefault
}

/*
  StyleManager is where all predefined styles are kept.
  
  Styles can be loaded in using the LoadStyle() method to choose 
  what GUIStyle they want applied to a GUI object.
*/
public static class StyleManager {
	private static GUIStyle returnStyle;
	private static bool initialized = false;
	
	// borders
	private static RectOffset nodeBorder;
	private static RectOffset controlPointBorder;
	private static RectOffset textAreaBorder;
	
	// padding
	private static RectOffset nodePadding;
	
	// file paths
	private const string NODE_DEFALT = "Assets/Editor/Resources/TestNodeBG.png";
	private const string NODE_HOVER = "Assets/Editor/Resources/TestNodeHoverBG.png";
	private const string NODE_SELECT = "Assets/Editor/Resources/TestNodeSelectedBG.png";
	
	private const string CONNECTIONPOINT_DEFAULT = "Assets/Editor/Resources/TestConnectionPointBG.png";
	private const string CONNECTIONPOINT_HOVER = "Assets/Editor/Resources/TestConnectionPointHoverBG.png";
	private const string CONNECTIONPOINT_SELECT = "Assets/Editor/Resources/TestConnectionPointSelectedBG.png";
	
	
	
	
	public static GUIStyle LoadStyle(Style style) {
		returnStyle = null;
		
		if (!initialized) {
			Debug.Log("Tried to LoadStyle before intializing. Initializing now...");
			Initialize();
		}
		
		switch(style) {
		case Style.NodeDefault:
			returnStyle = NodeDefaultStyle();
			break;
		case Style.NodeSelected:
			returnStyle = NodeSelectedStyle();
			break;
		case Style.NodeInterruptDefault:
			returnStyle = NodeInterruptDefaultStyle(); 
			break;
		case Style.NodeInterruptSelected:
			returnStyle = NodeInterruptSelectedStyle();
			break;
		case Style.ConnectionPointDefault:
			returnStyle = ConnectionPointDefaultStyle(); 
			break;
		case Style.ConnectionPointSelected:
			returnStyle = ConnectionPointSelectedStyle();
			break;
		case Style.TextAreaDefault:
			returnStyle = TextAreaDefaultStyle();
			break;
		case Style.TextBoxDefault:
			returnStyle = TextBoxDefaultStyle();
			break;
		case Style.TextBoxSelected:
			returnStyle = TextBoxSelectedStyle();
			break;
		case Style.TextAreaButtonDefault:
			returnStyle = TextAreaButtonDefaultStyle();
			break;
		case Style.LabelDefault:
			returnStyle = LabelDefaultStyle();
			break;
		}
		
		return returnStyle;
	}
	
	/*
	  Initializes the variables used to create styles
	*/
	public static void Initialize() {
		nodeBorder = new RectOffset(5, 5, 5, 5);
		controlPointBorder = new RectOffset(6, 5, 5, 5);
		textAreaBorder = nodeBorder;
		
		nodePadding = new RectOffset(5, 5, 5, 5);
		
		initialized = true;
	}
	
	// ------------------------------------------------------------------------------------------ //
	// ------------------------------------ COMPONENT STYLES ------------------------------------ //
	// ------------------------------------------------------------------------------------------ //
	
	private static GUIStyle NodeDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_DEFALT) as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon(NODE_HOVER) as Texture2D;
		style.border = nodeBorder;
		style.padding = nodePadding;
		
		style.normal.textColor = Color.white;
		style.hover.textColor = Color.white;
		style.fontSize = 14;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle NodeSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_SELECT) as Texture2D;
		style.border = nodeBorder;
		style.padding = nodePadding;
		
		style.normal.textColor = Color.white;
		style.fontSize = 14;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle NodeInterruptDefaultStyle() {
		GUIStyle style = NodeDefaultStyle();
		style.fontSize = 10;
		return style;
	}
	
	private static GUIStyle NodeInterruptSelectedStyle() {
		GUIStyle style = NodeSelectedStyle();
		style.fontSize = 10;
		return style;
	}
	
	private static GUIStyle ConnectionPointDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(CONNECTIONPOINT_DEFAULT) as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon(CONNECTIONPOINT_HOVER) as Texture2D;
		style.border = controlPointBorder;
		return style;
	}
	
	private static GUIStyle ConnectionPointSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(CONNECTIONPOINT_SELECT) as Texture2D;
		style.border = controlPointBorder;
		return style;
	}
	
	private static GUIStyle TextAreaDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.focused.textColor = Color.white;
		style.wordWrap = true;
		return style;
	}
	
	private static GUIStyle TextBoxDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_DEFALT) as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon(NODE_HOVER) as Texture2D;
		style.border = textAreaBorder;
		return style;
	}
	
	private static GUIStyle TextBoxSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_SELECT) as Texture2D;
		style.border = textAreaBorder;
		return style;
	}
	
	private static GUIStyle TextAreaButtonDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_DEFALT) as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon(NODE_HOVER) as Texture2D;
		style.active.background = AssetDatabase.GetCachedIcon(NODE_SELECT) as Texture2D;
		style.border = nodeBorder;
		
		style.fontSize = 10;
		style.normal.textColor = Color.white;
		style.hover.textColor = Color.yellow;
		style.active.textColor = Color.green;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
	
	private static GUIStyle LabelDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon(NODE_DEFALT) as Texture2D;
		style.border = nodeBorder;
		
		style.fontSize = 10;
		style.normal.textColor = Color.white;
		style.alignment = TextAnchor.MiddleCenter;
		return style;
	}
}
