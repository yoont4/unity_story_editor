using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Node : SDEComponent {
	
	// the display title of the node
	public string title;
	
	public bool isDragged;
	
	// the in/out connection points
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	
	// the dialog associated with the node
	public TextArea dialogArea;
	
	// the delegate for handling node removal
	public Action<Node> OnRemoveNode;
	
	public Node(
		Vector2 position, float width, float height, 
		GUIStyle defaultStyle, GUIStyle selectedStyle,
		Action<Node> OnRemoveNode) :
	base (
	SDEComponentType.Node, null, 
	new Rect(position.x, position.y, width, height), 
	defaultStyle,
	defaultStyle, 
	selectedStyle)
	{
		this.inPoint = new ConnectionPoint(this, ConnectionPointType.In);
		this.outPoint = new ConnectionPoint(this, ConnectionPointType.Out);
		this.dialogArea = new TextArea(this, "");
		this.OnRemoveNode = OnRemoveNode;
			
	}
	
	/*
	  Drag() shifts the position of the node.
	*/
	public void Drag(Vector2 delta) {
		rect.position += delta;
	}
	
	/*
	  Draw() draws the Node in the window, and all child components.
	*/
	public void Draw() {
		inPoint.Draw();
		outPoint.Draw();
		dialogArea.Draw();
		GUI.Box(rect, title, style);
	}
	
	/*
	  Processes Events running through the component.
	
	  note: Processes child events first.
	*/
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
		
		// process control point events first
		inPoint.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		dialogArea.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle dragging
			if (e.button == 0 && rect.Contains(e.mousePosition)) {
				isDragged = true;
			}
			
			// handle context menu
			if (e.button == 1 && Selected && rect.Contains(e.mousePosition)) {
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
				GUI.changed = true;
			}
			break;
		}
	}
	
	
	/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
		genericMenu.ShowAsContext();
	}
	
	/*
	  OnClickRemoveNode() activates the OnRemoveNode actions for this Node
	*/
	private void OnClickRemoveNode() {
		if (OnRemoveNode != null) {
			OnRemoveNode(this);
			
			// if a Node is selected, it is this one, so we need to Deselect it in the SelectionManager
			if (SelectionManager.SelectedComponentType() == SDEComponentType.Node) {
				SelectionManager.ClearSelection();
			}
		}
	}
}
