using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Node {

	public Rect rect;
	public string title;
	public bool isDragged;
	
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
		switch(e.type) {
			case EventType.MouseDown:
				if (e.button == 0) {
					if (rect.Contains(e.mousePosition)) {
						isDragged = true;
						GUI.changed = true;
					} else {
						GUI.changed = true;
					}
				}
				break;
			
			case EventType.MouseUp:
				isDragged = false;
				break;
			
			case EventType.MouseDrag:
				if (e.button == 0 && isDragged) {
					Drag(e.delta);
					e.Use();
					return true;
				}
				break;
		}
		
		return false;
	}
	
}
