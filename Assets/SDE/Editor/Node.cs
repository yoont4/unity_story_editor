using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
	
	// used for CheckLocal/Global modes
	public OutstreamSplitter splitter;
	
	// used for local flag Nodes
	public DropdownLocalFlagBox localFlagDropdown;
	
	// used for global flag AND variable Nodes
	public DropdownGlobalItemBox globalItemDropdown;
	
	// used for global variable Nodes to determine what value to check against/set
	public TextArea globalVariableField;
	
	// the action for handling node removal
	private Action<Node> OnRemoveNode;
	
	// variables to maintain mouse offset and grid positioning on Move() 
	private Vector2 offset;
	private Vector2 gridOffset;
	
	// Interrupt specific vars
	public bool bottomLevel = true;
	
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
			ToggleDialog(record:false);
			break;
			
		case NodeType.Decision:
			ToggleDecision(record:false);
			break;
			
		case NodeType.Interrupt:
			ToggleInterrupt();
			break;
			
		case NodeType.SetLocalFlag:
			ToggleSetLocalFlag(record:false);
			break;
			
		case NodeType.CheckLocalFlag:
			ToggleCheckLocalFlag(record:false);
			break;
			
		case NodeType.SetGlobalFlag:
			ToggleSetGlobalFlag(record:false);
			break;
			
		case NodeType.CheckGlobalFlag:
			ToggleCheckGlobalFlag(record:false);
			break;
			
		case NodeType.SetGlobalVariable:
			ToggleSetGlobalVariable(record:false);
			break;
			
		case NodeType.CheckGlobalVariable:
			ToggleCheckGlobalVariable(record:false);
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
		
		switch (nodeType) {
		case NodeType.Nothing:
			DrawStartOptions();
			break;
			
		case NodeType.Dialog:
			DrawDialog();
			break;
			
		case NodeType.Decision:
			DrawDecision();
			break;
			
		case NodeType.SetLocalFlag:
			DrawSetLocalFlag();
			break;
			
		case NodeType.CheckLocalFlag:
			DrawCheckLocalFlag();
			break;
			
		case NodeType.SetGlobalFlag:
			DrawSetGlobalFlag();
			break;
			
		case NodeType.CheckGlobalFlag:
			DrawCheckGlobalFlag();
			break;
			
		case NodeType.SetGlobalVariable:
			DrawSetGlobalVariable();
			break;
			
		case NodeType.CheckGlobalVariable:
			DrawCheckGlobalVariable();
			break;
			
		case NodeType.Interrupt:
			DrawInterrupt();
			break;
		}
		
		GUI.Box(rect, title, style);
	}
	
	/*
	  DrawStartOptions() draws the options for newly created Nodes
	*/
	public void DrawStartOptions() {
		// TODO: these should be icons instead of text buttons
		
		if (GUI.Button(new Rect(rect.x, rect.y + rect.height, 25, 25), "Di", SDEStyles.textButtonDefault)) {
			ToggleDialog();
		}
		
		if (GUI.Button(new Rect(rect.x+25, rect.y + rect.height, 25, 25), "De", SDEStyles.textButtonDefault)) {
			ToggleDecision();
		}
		
		if (GUI.Button(new Rect(rect.x+50, rect.y + rect.height, 25, 25), "SL", SDEStyles.textButtonDefault)) {
			ToggleSetLocalFlag();
		}
		
		if (GUI.Button(new Rect(rect.x+75, rect.y + rect.height, 25, 25), "CL", SDEStyles.textButtonDefault)) {
			ToggleCheckLocalFlag();
		}
		
		if (GUI.Button(new Rect(rect.x+100, rect.y + rect.height, 25, 25), "SG", SDEStyles.textButtonDefault)) {
			ToggleSetGlobalFlag();
		}
		
		if (GUI.Button(new Rect(rect.x+125, rect.y + rect.height, 25, 25), "CG", SDEStyles.textButtonDefault)) {
			ToggleCheckGlobalFlag();
		}
		
		if (GUI.Button(new Rect(rect.x+150, rect.y + rect.height, 25, 25), "SV", SDEStyles.textButtonDefault)) {
			ToggleSetGlobalVariable();
		}
		
		if (GUI.Button(new Rect(rect.x+175, rect.y + rect.height, 25, 25), "CV", SDEStyles.textButtonDefault)) {
			ToggleCheckGlobalVariable();
		}
	} 
	
	/*
	  DrawDialog() is used when the Node Type is Dialog, and draws a dialog entry menu.
	*/
	private void DrawDialog() {
		childContainer.Draw();
		
		// calculate the y position of the dialog buttons
		SDEContainer tempChild = childContainer;
		float buttonY = rect.y + rect.height + 2;
		while (true) {
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
		while (true) {
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
		
		if (outPoint != null) {
			outPoint.Draw();
		}
	}
	
	private void DrawSetLocalFlag() {
		localFlagDropdown.SetPosition(rect.x, rect.y + rect.height);
		localFlagDropdown.Draw();
		
		outPoint.Draw();
	}
	
	private void DrawCheckLocalFlag() {
		splitter.SetPosition(rect.x+rect.width+1, rect.y+7);
		splitter.Draw();
		
		localFlagDropdown.SetPosition(rect.x, rect.y + rect.height);
		localFlagDropdown.Draw();
	}
	
	private void DrawSetGlobalFlag() {
		globalItemDropdown.SetPosition(rect.x, rect.y + rect.height);
		globalItemDropdown.Draw();
		
		outPoint.Draw();
	}
	
	private void DrawCheckGlobalFlag() {
		splitter.SetPosition(rect.x+rect.width+1, rect.y+7);
		splitter.Draw();
		
		globalItemDropdown.SetPosition(rect.x, rect.y + rect.height);
		globalItemDropdown.Draw();
	}
	
	private void DrawSetGlobalVariable() {
		globalItemDropdown.SetPosition(rect.x, rect.y + rect.height);
		globalItemDropdown.Draw();
		
		globalVariableField.Draw();
		
		outPoint.Draw();
	}
	
	private void DrawCheckGlobalVariable() {
		globalItemDropdown.SetPosition(rect.x, rect.y + rect.height);
		globalItemDropdown.Draw();
		
		globalVariableField.Draw();
		
		splitter.SetPosition(rect.x+rect.width+1, rect.y+7);
		splitter.Draw();
	}
	
	/*
	  Processes Events running through the component.
	
	  note: Processes child events first.
	*/
	public override void ProcessEvent(Event e) {
		// process control point events first
		inPoint.ProcessEvent(e);
		
		// process subcomponents if necessary
		if (outPoint != null) outPoint.ProcessEvent(e);
		if (childContainer != null) childContainer.ProcessEvent(e);
		if (splitter != null) splitter.ProcessEvent(e);
		if (localFlagDropdown != null) localFlagDropdown.ProcessEvent(e);
		if (globalItemDropdown != null) globalItemDropdown.ProcessEvent(e);
		if (globalVariableField != null) globalVariableField.ProcessEvent(e);
		
		base.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle the start of a drag
			if (SelectionManager.SelectedComponent() == this &&
				e.button == 0 && Contains(e.mousePosition)) {
				HandleDragStart(e);
			}
			
			// handle context menu
			if (e.button == 1 && Selected && Contains(e.mousePosition)) {
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
	
	private void ToggleDialog(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.Dialog;
		title = "DIALOG";
		
		rect.width = NodeManager.TEXT_NODE_WIDTH;
		rect.height = NodeManager.TEXT_NODE_HEIGHT;
		clickRect.width = NodeManager.TEXT_NODE_WIDTH;
		clickRect.height = NodeManager.TEXT_NODE_HEIGHT;
		
		// create a child DialogBox
		this.childContainer = ScriptableObject.CreateInstance<DialogBox>();
		((DialogBox)this.childContainer).Init(this, "");
		
	}
	
	private void ToggleDecision(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.Decision;
		title = "DECISION";
		
		rect.width = NodeManager.TEXT_NODE_WIDTH;
		rect.height = NodeManager.TEXT_NODE_HEIGHT;
		clickRect.width = NodeManager.TEXT_NODE_WIDTH;
		clickRect.height = NodeManager.TEXT_NODE_HEIGHT;
		
		// create a child DecisionBox
		this.childContainer = ScriptableObject.CreateInstance<DecisionBox>();
		((DecisionBox)this.childContainer).Init(this, "");
		
	}
	
	private void ToggleInterrupt() {
		nodeType = NodeType.Interrupt;
		title = "-->";
		
		style = SDEStyles.nodeSmallDefault;
		defaultStyle = SDEStyles.nodeSmallDefault;
		selectedStyle = SDEStyles.nodeSmallSelected;
		
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		outPoint.Init(this, ConnectionPointType.Out);
	}
	
	private void ToggleSetLocalFlag(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.SetLocalFlag;
		title = "SET LOCAL FLAG";
		
		style = new GUIStyle(SDEStyles.nodeSmallDefault);
		defaultStyle = new GUIStyle(SDEStyles.nodeSmallDefault);
		selectedStyle = new GUIStyle(SDEStyles.nodeSmallSelected);
		
		rect.width = NodeManager.FLAG_WIDTH;
		rect.height = NodeManager.FLAG_HEIGHT;
		clickRect.width = NodeManager.FLAG_WIDTH;
		clickRect.height = NodeManager.FLAG_HEIGHT;
		
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		outPoint.Init(this, ConnectionPointType.Out);
		
		localFlagDropdown = ScriptableObject.CreateInstance<DropdownLocalFlagBox>();
		localFlagDropdown.Init();
		
		// bind the dropdown menu to the main editor's local flag list
		localFlagDropdown.LinkFlags(NodeManager.mainEditor.localFlagsMenu.items);
	}
	
	private void ToggleCheckLocalFlag(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.CheckLocalFlag;
		title = "CHECK LOCAL FLAG";
		
		style = new GUIStyle(SDEStyles.nodeSmallDefault);
		defaultStyle = new GUIStyle(SDEStyles.nodeSmallDefault);
		selectedStyle = new GUIStyle(SDEStyles.nodeSmallSelected);
		
		rect.width = NodeManager.FLAG_WIDTH;
		rect.height = NodeManager.FLAG_HEIGHT;
		clickRect.width = NodeManager.FLAG_WIDTH;
		clickRect.height = NodeManager.FLAG_HEIGHT;
		
		localFlagDropdown = ScriptableObject.CreateInstance<DropdownLocalFlagBox>();
		localFlagDropdown.Init();
		
		splitter = ScriptableObject.CreateInstance<OutstreamSplitter>();
		splitter.Init(SplitterType.Flag);
		
		// bind the dropdown menu to the main editor's local flag list
		localFlagDropdown.LinkFlags(NodeManager.mainEditor.localFlagsMenu.items);
	}
	
	private void ToggleSetGlobalFlag(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.SetGlobalFlag;
		title = "SET GLOBAL FLAG";
		
		style = new GUIStyle(SDEStyles.nodeSmallDefault);
		defaultStyle = new GUIStyle(SDEStyles.nodeSmallDefault);
		selectedStyle = new GUIStyle(SDEStyles.nodeSmallSelected);
		
		rect.width = NodeManager.FLAG_WIDTH;
		rect.height = NodeManager.FLAG_HEIGHT;
		clickRect.width = NodeManager.FLAG_WIDTH;
		clickRect.height = NodeManager.FLAG_HEIGHT;
		
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		outPoint.Init(this, ConnectionPointType.Out);
		
		globalItemDropdown =  ScriptableObject.CreateInstance<DropdownGlobalItemBox>();
		globalItemDropdown.Init();
		
		// bind the dropdown to the global flag list
		globalItemDropdown.LoadItems(GlobalFlags.flags);
	}
	
	private void ToggleCheckGlobalFlag(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.CheckGlobalFlag;
		title = "CHECK GLOBAL FLAG";
		
		style = new GUIStyle(SDEStyles.nodeSmallDefault);
		defaultStyle = new GUIStyle(SDEStyles.nodeSmallDefault);
		selectedStyle = new GUIStyle(SDEStyles.nodeSmallSelected);
		
		rect.width = NodeManager.FLAG_WIDTH;
		rect.height = NodeManager.FLAG_HEIGHT;
		clickRect.width = NodeManager.FLAG_WIDTH;
		clickRect.height = NodeManager.FLAG_HEIGHT;
		
		globalItemDropdown = ScriptableObject.CreateInstance<DropdownGlobalItemBox>();
		globalItemDropdown.Init();
		
		// bind the dropdown to the global flag list
		globalItemDropdown.LoadItems(GlobalFlags.flags);
		
		splitter = ScriptableObject.CreateInstance<OutstreamSplitter>();
		splitter.Init(SplitterType.Flag);
	}
	
	private void ToggleSetGlobalVariable(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.SetGlobalVariable;
		title = "SET GLOBAL VARIABLE";
		
		ToggleGlobalVariable();
		
		outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		outPoint.Init(this, ConnectionPointType.Out);
	}
	
	private void ToggleCheckGlobalVariable(bool record=true) {
		if (record) {
			HistoryManager.RecordNode(this);
		}
		
		nodeType = NodeType.CheckGlobalVariable;
		title = "CHECK GLOBAL VARIABLE";
		
		ToggleGlobalVariable();
		
		splitter = ScriptableObject.CreateInstance<OutstreamSplitter>();
		splitter.Init(SplitterType.Variable);
	}
	
	// helper function of ToggleSetGlobalVariable() and ToggleCheckGlobalVariable()
	private void ToggleGlobalVariable() {
		style = new GUIStyle(SDEStyles.nodeSmallDefault);
		defaultStyle = new GUIStyle(SDEStyles.nodeSmallDefault);
		selectedStyle = new GUIStyle(SDEStyles.nodeSmallSelected);
		
		rect.width = NodeManager.VARIABLE_WIDTH;
		rect.height = NodeManager.VARIABLE_HEIGHT;
		clickRect.width = NodeManager.VARIABLE_WIDTH;;
		clickRect.height = NodeManager.VARIABLE_HEIGHT;
		
		globalItemDropdown = ScriptableObject.CreateInstance<DropdownGlobalItemBox>();
		globalItemDropdown.Init();
		
		// bind the dropdown to the global variables list
		globalItemDropdown.LoadItems(GlobalVariables.variables);
		
		// instantiate the check field
		globalVariableField = ScriptableObject.CreateInstance<TextArea>();
		globalVariableField.Init(this, "0", 50);
		globalVariableField.parentOffset = new Vector2(140, -1);
		globalVariableField.maxLength = 4;
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
			
			// clear outpoint reference
			outPoint.connections.Clear();
			outPoint = null;
		} else if (bottomLevel && outPoint == null) {
			outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
			outPoint.Init(this, ConnectionPointType.Out);
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
