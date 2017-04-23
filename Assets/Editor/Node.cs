using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Node {

	public Rect rect;
	public string title;
	
	public GUIStyle style;
	
	public Node(Vector2 position, float width, float height, GUIStyle nodeStyle) {
		rect = new Rect(position.x, position.y, width, height);
		style = nodeStyle;
	}
	
	public void Drag(Vector2 delta) {
		rect.position += delta;
	}
	
	public void Draw() {
		GUI.Box(rect, title, style);
	}
	
	public bool ProcessEvent(Event e) {
		return false;
	}
	
}
