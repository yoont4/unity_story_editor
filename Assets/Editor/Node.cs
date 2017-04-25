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
	
	public TextArea dialogArea;
	
	public GUIStyle style;
	public GUIStyle defaultNodeStyle;
	public GUIStyle selectedNodeStyle;
	
	public Action<Node> OnRemoveNode;
	
	public Node(
		Vector2 position, float width, float height, 
		GUIStyle defaultStyle, GUIStyle selectedStyle,
		Action<Node> OnClickRemoveNode)
	{
		this.rect = new Rect(position.x, position.y, width, height);
		this.style = defaultStyle;
		this.defaultNodeStyle = defaultStyle;
		this.selectedNodeStyle = selectedStyle;
		this.inPoint = new ConnectionPoint(this, ConnectionPointType.In);
		this.outPoint = new ConnectionPoint(this, ConnectionPointType.Out);
		this.dialogArea = new TextArea(this, "");
		this.OnRemoveNode = OnClickRemoveNode;
			
	}
	
	public void Drag(Vector2 delta) {
		rect.position += delta;
	}
	
	public void Draw() {
		inPoint.Draw();
		outPoint.Draw();
		dialogArea.Draw();
		GUI.Box(rect, title, style);
	}
	
	public bool ProcessEvent(Event e) {
		// process control point events first
		inPoint.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		dialogArea.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle selection
			if (e.button == 0) {
				if (rect.Contains(e.mousePosition) && NodeManager.selectedNode == null) {
					// prevent overlapping lower-ordered nodes from being selected
					NodeManager.selectedNode = this;
					
					isDragged = true;
					StyleSelect();
				} else {
					StyleDeselect();
				}
			}
			
			// handle context menu
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
	
	// TODO: figure out how to manage StyleSelect() vs a logical Select() in
	// this class (Node) and the ConnectionPoint class. It will bubble into 
	// other classes eventually.
	
	// maybe use a SelectionManager class? or have all selections be parsed
	// through it's own EventProcessing stack? I dunno, gotta keep it performant
	// but it needs to be legible. Don't ignore this for too long!
	
	// Changes Node style to selection.
	// NOTE: this does NOT change the NodeManager's selectedNode field
	public void StyleSelect() {
		isSelected = true;
		style = selectedNodeStyle;
		GUI.changed = true;
	}
	
	public void StyleDeselect() {
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
