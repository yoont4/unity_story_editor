using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Node {

	public Rect rect;
	public string title;
	
	public bool isDragged;
	public bool isSelected;
	
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	
	public GUIStyle style;
	public GUIStyle defaultNodeStyle;
	public GUIStyle selectedNodeStyle;
	
	public Action<Node> OnRemoveNode;
	
	public Node(
		Vector2 position, float width, float height, 
		GUIStyle defaultStyle, GUIStyle selectedStyle,
		Action<Node> OnClickRemoveNode)
	{
		rect = new Rect(position.x, position.y, width, height);
		style = defaultStyle;
		defaultNodeStyle = defaultStyle;
		selectedNodeStyle = selectedStyle;
		inPoint = new ConnectionPoint(this, ConnectionPointType.In);
		outPoint = new ConnectionPoint(this, ConnectionPointType.Out);
		OnRemoveNode = OnClickRemoveNode;
			
	}
	
	public void Drag(Vector2 delta) {
		rect.position += delta;
	}
	
	public void Draw() {
		inPoint.Draw();
		outPoint.Draw();
		GUI.Box(rect, title, style);
	}
	
	public bool ProcessEvent(Event e) {
		// process control point events first
		inPoint.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle selection
			if (e.button == 0) {
				if (rect.Contains(e.mousePosition) && !NodeManager.nodeSelected) {
					// prevent overlapping lower-ordered nodes from being selected
					NodeManager.nodeSelected = true;
					
					isDragged = true;
					Select();
				} else {
					Deselect();
				}
			}
			
			// handle contect menu
			if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)) {
				ProcessContextMenu();
				e.Use();
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
	
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
		genericMenu.ShowAsContext();
	}
	
	public void Select() {
		isSelected = true;
		style = selectedNodeStyle;
		GUI.changed = true;
	}
	
	public void Deselect() {
		isSelected = false;
		style = defaultNodeStyle;
		GUI.changed = true;
	}
	
	
	
	private void OnClickRemoveNode() {
		if (OnRemoveNode != null) {
			OnRemoveNode(this);
		}
	}
	
	
	
}
