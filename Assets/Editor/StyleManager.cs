using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Style {
	NodeDefault,
	NodeSelected,
	ControlPointDefault,
	ControlPointSelected,
	Decision,
	Potato,
	Test
}

/*
  StyleManager is where all predefined styles are kept.
  
  Styles can be loaded in using the LoadStyle() method to choose 
  what GUIStyle they want applied to a GUI object.
*/
public static class StyleManager {
	private static GUIStyle returnStyle;
	
	public static GUIStyle LoadStyle(Style style) {
		returnStyle = null;
		
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
		case Style.Decision:
			break;
		case Style.Potato:
			break;
		case Style.Test:
			break;
		}
		
		return returnStyle;
	}
	
	private static GUIStyle NodeDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeBG.png") as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeOnBG.png") as Texture2D;
		style.border = new RectOffset(5, 5, 5, 5);
		return style;
	}
	
	private static GUIStyle NodeSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestNodeSelectedBG.png") as Texture2D;
		style.border = new RectOffset(5, 5, 5, 5);
		return style;
	}
	
	private static GUIStyle ControlPointDefaultStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointBG.png") as Texture2D;
		style.hover.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointOnBG.png") as Texture2D;
		style.border = new RectOffset(6, 5, 5, 5);
		return style;
	}
	
	private static GUIStyle ControlPointSelectedStyle() {
		GUIStyle style = new GUIStyle();
		style.normal.background = AssetDatabase.GetCachedIcon("Assets/Editor/Resources/TestControlPointSelectedBG.png") as Texture2D;
		style.border = new RectOffset(6, 5, 5, 5);
		return style;
	}
}
