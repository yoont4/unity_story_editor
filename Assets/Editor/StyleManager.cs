using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Style {
	NodeDefault,
	NodeSelected,
	ControlPointDefault,
	ControlPointSelected,
	TextAreaDefault,
	TextBoxDefault,
	TextBoxSelected
}

/*
  StyleManager is where all predefined styles are kept.
  
  Styles can be loaded in using the LoadStyle() method to choose 
  what GUIStyle they want applied to a GUI object.
*/
public static class StyleManager {
	private static GUIStyle returnStyle;
	private static bool initialized = false;
	
	private static RectOffset nodeBorder;
	private static RectOffset controlPointBorder;
	private static RectOffset textAreaBorder;
	
	
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
		case Style.ControlPointDefault:
			returnStyle = ControlPointDefaultStyle();
			break;
		case Style.ControlPointSelected:
			returnStyle = ControlPointSelectedStyle();
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
		
		initialized = true;
	}
	
	private static GUIStyle NodeDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeBG.png") as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeHoverBG.png") as Texture2D;
		style.border = nodeBorder;
		return style;
	}
	
	private static GUIStyle NodeSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeSelectedBG.png") as Texture2D;
		style.border = nodeBorder;
		return style;
	}
	
	private static GUIStyle ControlPointDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointBG.png") as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointHoverBG.png") as Texture2D;
		style.border = controlPointBorder;
		return style;
	}
	
	private static GUIStyle ControlPointSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointSelectedBG.png") as Texture2D;
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
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeBG.png") as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeHoverBG.png") as Texture2D;
		style.border = textAreaBorder;
		return style;
	}
	
	private static GUIStyle TextBoxSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeSelectedBG.png") as Texture2D;
		style.border = textAreaBorder;
		return style;
	}
}
