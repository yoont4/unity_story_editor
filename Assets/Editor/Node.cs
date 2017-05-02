using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum NodeType {Dialog, Decision, LocalFlag, GlobalFlag}

/*
  Nodes are the master SDEComponent type in the StoryDialogueEditor, and serve
  as the anchor/parent of all subcomponents.
*/
public class Node : SDEComponent {
	
	// the display title of the node
	public string title;
	
	// the specific type of the node
	public NodeType nodeType;
	
	public bool isDragged;
	
	// the in/out connection points
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	
	// the dialog associated with the node
	public TextArea dialogArea;
	
	// the delegate for handling node removal
	private Action<Node> OnRemoveNode;
	
	// variables to maintain mouse offset and grid positioning on Move() 
	private Vector2 offset;
	private Vector2 gridOffset;
	
	// used to check if Node was dragged or not for Undo stack
	private bool dragged = false;
	
	
	public Node() {}
	
	public void Init(
		Vector2 position, float width, float height, 
		GUIStyle defaultStyle, GUIStyle selectedStyle,
		Action<Node> OnRemoveNode) 
	{
		Init(SDEComponentType.Node, null, 
			new Rect(position.x, position.y, width, height), 
			defaultStyle,
			defaultStyle, 
			selectedStyle);
		
		this.inPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.inPoint.Init(this, ConnectionPointType.In);
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this, ConnectionPointType.Out);
		this.dialogArea = ScriptableObject.CreateInstance<TextArea>();
		this.dialogArea.Init(this, "");
		this.OnRemoveNode = OnRemoveNode;
	}
	
	/*
	  Drag() shifts the position of the node by a delta.
	
	  Used to handle pans and view transforms. Use Move() to handle
	  individual Node transforms to keep it on the grid.
	*/
	public void Drag(Vector2 delta) {
		rect.position += delta;
	}
	
	/*
	  Move() moves a Node to the destination on a gridlock.
	
	  Always use this for Node specific movements to maintain snapped positions.
	*/
	public void Move(Vector2 destination) {
		destination -= offset;
		destination -= new Vector2(destination.x % StoryDialogueEditor.GRID_SIZE, destination.y % StoryDialogueEditor.GRID_SIZE);
		destination += gridOffset;
		rect.position = destination;
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
			// handle the start of a drag
			if (SelectionManager.SelectedComponent() == this &&
				e.button == 0 && rect.Contains(e.mousePosition)) {
				HandleDragStart(e);
			}
			
			// handle context menu
			if (e.button == 1 && Selected && rect.Contains(e.mousePosition)) {
				ProcessContextMenu();
				e.Use();
			}
			break;
		
		case EventType.MouseUp:
			HandleDragEnd();
			break;
		
		case EventType.MouseDrag:
			// handle Node moving
			if (e.button == 0 && isDragged) {
				HandleDrag(e);
			}
			break;
		}
	}
	
	/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Node"), false, CallOnRemoveNode);
		genericMenu.ShowAsContext();
	}
	
	/*
	  CallOnRemoveNode() activates the OnRemoveNode actions for this Node
	*/
	private void CallOnRemoveNode() {
		if (OnRemoveNode != null) {
			OnRemoveNode(this);
		} else {
			throw new UnityException("Tried to call OnRemoveNode when null!");
		}
	}
	
	private void HandleDragStart(Event e) {
		Undo.RegisterFullObjectHierarchyUndo(this, "Node moved...");
		
		isDragged = true;
		offset = e.mousePosition - rect.position;
		gridOffset = new Vector2(rect.position.x % StoryDialogueEditor.GRID_SIZE, rect.position.y % StoryDialogueEditor.GRID_SIZE);
	}
	
	private void HandleDrag(Event e) {
		// only register a Node being dragged once
		if (!dragged) {
			dragged = true;
		}
		
		Move(e.mousePosition);
		e.Use();
		GUI.changed = true;
	}
	
	private void HandleDragEnd() {
		// if the object was actually moved, register the undo
		// otherwise, revert the stored undo.
		if (dragged) {
			Undo.FlushUndoRecordObjects();
		} else if (isDragged) {
			Undo.RevertAllInCurrentGroup();
		}
		
		isDragged = false;
		dragged = false;
	}
}
