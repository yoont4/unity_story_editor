using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum NodeType {Nothing, Dialog, Decision, SetLocalFlag, SetGlobalFlag, CheckLocalFlag, CheckGlobalFlag, Interrupt}

/*
  Nodes are the master SDEComponent type in the StoryDialogEditor, and serve
  as the anchor/parent of all subcomponents.
*/
public class Node : SDEComponent {
	
	// the display title of the node
	public string title = "SELECT TYPE";
	
	// the specific type of the node
	public NodeType nodeType = NodeType.Nothing;
	
	// the child Container that starts the cascading Container display
	public SDEContainer childContainer;
	
	public bool isDragged;
	
	// the in/out connection points
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	
	// the action for handling node removal
	private Action<Node> OnRemoveNode;
	
	// the action for handling how to draw the child node.
	// NOTE: an action is used to replace the need for a switch statement.
	private Action OnDrawNodeChild;
	
	// variables to maintain mouse offset and grid positioning on Move() 
	private Vector2 offset;
	private Vector2 gridOffset;
	
	// Interrupt specific vars
	private bool bottomLevel = true;
	
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
		

		
		this.OnRemoveNode = OnRemoveNode;
		this.OnDrawNodeChild = DrawStartOptions;
	}
	
	public void Init(
		Vector2 position, float width, float height, 
		GUIStyle defaultStyle, GUIStyle selectedStyle,
		Action<Node> OnRemoveNode, NodeType type)
	{
		Init(position, width, height, defaultStyle, selectedStyle, OnRemoveNode);
		
		this.nodeType = type;
		switch(type) {
		case NodeType.Dialog:
			ToggleDialog();
			break;
		case NodeType.Decision:
			ToggleDecision();
			break;
		case NodeType.Interrupt:
			ToggleInterrupt();
			break;
		default:
			break;
		}
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
		destination -= new Vector2(destination.x % StoryDialogEditor.GRID_SIZE, destination.y % StoryDialogEditor.GRID_SIZE);
		destination += gridOffset;
		rect.position = destination;
	}
	
	/*
	  Draw() draws the Node in the window, and all child components.
	*/
	public override void Draw() {
		inPoint.Draw();
		
		CallOnDrawNodeChild();
		
		GUI.Box(rect, title, style);
	}
	
	/*
	  DrawStartOptions() draws the options for newly created Nodes
	*/
	public void DrawStartOptions() {
		// TODO: finalize the buttons and use the correct GUI styles.
		
		if (GUI.Button(new Rect(rect.x, rect.y + rect.height, 33, 24), "Text", SDEStyles.textButtonDefault)) {
			ToggleDialog();
		}
		
		if(GUI.Button(new Rect(rect.x+33, rect.y + rect.height, 33, 24), "Dec", SDEStyles.textButtonDefault)) {
			ToggleDecision();
		}
		
		if (GUI.Button(new Rect(rect.x+67, rect.y + rect.height, 33, 24), "SLV", SDEStyles.textButtonDefault)) {
			ToggleSetLocalFlag();
		}
		
		if (GUI.Button(new Rect(rect.x+100, rect.y + rect.height, 33, 24), "GLV", SDEStyles.textButtonDefault)) {
			ToggleCheckLocalFlag();
		}
		GUI.Button(new Rect(rect.x+134, rect.y + rect.height, 33, 24), "SGV", SDEStyles.textButtonDefault);
		GUI.Button(new Rect(rect.x+167, rect.y + rect.height, 33, 24), "GGV", SDEStyles.textButtonDefault);
	} 
	
	/*
	  DrawDialog() is used when the Node Type is Dialog, and draws a dialog entry menu.
	*/
	private void DrawDialog() {
		childContainer.Draw();
		
		// calculate the y position of the dialog buttons
		SDEContainer tempChild = childContainer;
		float buttonY = rect.y + rect.height + 2;
		while(true) {
			buttonY += tempChild.rect.height;
			
			if (tempChild.child == null) {
				break;
			}
			
			tempChild  = tempChild.child;
		}
		
		
		// only draw the remove TextArea button if there are multiple TextAreas
		if (tempChild.parentNode != this) {
			if (GUI.Button(new Rect(rect.xMax-33, buttonY, 16, 16), "-", SDEStyles.textButtonDefault)) {
				((DialogBox)tempChild).Remove();
			}
		}
		
		if (GUI.Button(new Rect(rect.xMax-16, buttonY, 16, 16), "+", SDEStyles.textButtonDefault)) {
			HistoryManager.RecordEditor();
			
			Debug.Log("Adding DialogBox");
			
			tempChild.child = ScriptableObject.CreateInstance<DialogBox>();
			((DialogBox)tempChild.child).Init(tempChild, "");
		}
	}
	
	private void DrawDecision() {
		childContainer.Draw();
		
		// calculate the y position of the dialog buttons
		SDEContainer tempChild = childContainer;
		float buttonY = rect.y + rect.height + 2;
		while(true) {
			buttonY += tempChild.rect.height;
			
			if (tempChild.child == null) {
				break;
			}
			
			tempChild  = tempChild.child;
		}
		
		// only draw the remove DecisionBox button if there are multiple DecisionBoxes
		if (tempChild.parentNode != this) {
			if (GUI.Button(new Rect(rect.xMax-33, buttonY, 16, 16), "-", SDEStyles.textButtonDefault)) {
				((DecisionBox)tempChild).Remove();
			}
		}
		
		if (GUI.Button(new Rect(rect.xMax-16, buttonY, 16, 16), "+", SDEStyles.textButtonDefault)) {
			HistoryManager.RecordEditor();
			
			Debug.Log("Adding DecisionBox");
			
			tempChild.child = ScriptableObject.CreateInstance<DecisionBox>();
			((DecisionBox)tempChild.child).Init(tempChild, "");
		}
	}
	
	private void DrawInterrupt() {
		if (childContainer != null) {
			childContainer.Draw();
		}
		
		if(outPoint != null && bottomLevel) {
			outPoint.Draw();
		}
	}
	
	private void DrawSetLocalFlag() {
		// TODO: implement this
	}
	
	private void DrawCheckLocalFlag() {
		// TODO: implement this
	}
	
	/*
	  Processes Events running through the component.
	
	  note: Processes child events first.
	*/
	public override void ProcessEvent(Event e) {
		// process control point events first
		inPoint.ProcessEvent(e);
		if (outPoint != null) {
			outPoint.ProcessEvent(e);
		}
		
		
		if (childContainer != null) {
			childContainer.ProcessEvent(e);
		}
		
		base.ProcessEvent(e);
		
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
			if (e.button == 0 && Selected) {
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
	
	private void ToggleDialog() {
		// create a child DialogBox
		this.childContainer = ScriptableObject.CreateInstance<DialogBox>();
		((DialogBox)this.childContainer).Init(this, "");
		
		
		nodeType = NodeType.Dialog;
		OnDrawNodeChild = DrawDialog;
		title = "DIALOG";
	}
	
	private void ToggleDecision() {
		// create a child DecisionBox
		this.childContainer = ScriptableObject.CreateInstance<DecisionBox>();
		((DecisionBox)this.childContainer).Init(this, "");
		
		nodeType = NodeType.Decision;
		OnDrawNodeChild = DrawDecision;
		title = "DECISION";
	}
	
	private void ToggleInterrupt() {
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		((ConnectionPoint)this.outPoint).Init(this, ConnectionPointType.Out);
		
		style = SDEStyles.nodeSmallDefault;
		defaultStyle = SDEStyles.nodeSmallDefault;
		selectedStyle = SDEStyles.nodeSmallSelected;
		
		nodeType = NodeType.Interrupt;
		OnDrawNodeChild = DrawInterrupt;
		title = "-->";
	}
	
	private void ToggleSetLocalFlag() {
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		((ConnectionPoint)this.outPoint).Init(this, ConnectionPointType.Out);
		
		style = SDEStyles.nodeSmallDefault;
		defaultStyle = SDEStyles.nodeSmallDefault;
		selectedStyle = SDEStyles.nodeSmallSelected;
		
		nodeType = NodeType.SetLocalFlag;
		OnDrawNodeChild = DrawSetLocalFlag;
		title = "SET LOCAL FLAG";
		
		rect.width = 140;
		rect.height = 26;
	}
	
	private void ToggleCheckLocalFlag() {
		style = SDEStyles.nodeSmallDefault;
		defaultStyle = SDEStyles.nodeSmallDefault;
		selectedStyle = SDEStyles.nodeSmallSelected;
		
		nodeType = NodeType.CheckLocalFlag;
		OnDrawNodeChild = DrawCheckLocalFlag;
		title = "CHECK LOCAL FLAG";
		
		rect.width = 140;
		rect.height = 26;
	}
	
	public void SetBottomLevelInterrupt(bool bottomLevel) {
		// mark this as a bottom level Interrupt Node
		this.bottomLevel = bottomLevel;
		
		// clear any connections that were there if no longer a bottom level
		if (!bottomLevel && outPoint != null) {
			List<Connection> connectionsToRemove = outPoint.connections;
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				ConnectionManager.RemoveConnection(connectionsToRemove[i], markHistory: true);
			}
		}
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
	
	/*
	  CallOnDrawNodeChild() activates the OnDrawNodeChild actions for this Node
	*/
	private void CallOnDrawNodeChild() {
		if (OnDrawNodeChild != null) {
			OnDrawNodeChild();
		} else {
			throw new UnityException("Tried to call OnDrawNodeChild when null!");
		}
	}
	
	private void HandleDragStart(Event e) {
		offset = e.mousePosition - rect.position;
		gridOffset = new Vector2(rect.position.x % StoryDialogEditor.GRID_SIZE, rect.position.y % StoryDialogEditor.GRID_SIZE);
	}
	
	private void HandleDrag(Event e) {
		// only register a Node being dragged once
		if (!isDragged) {
			HistoryManager.RecordEditor();
			
			isDragged = true;
		}
		
		Move(e.mousePosition);
		e.Use();
		GUI.changed = true;
	}
	
	private void HandleDragEnd() {
		isDragged = false;
	}
}
