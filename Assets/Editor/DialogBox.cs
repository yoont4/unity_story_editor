using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogBox : SDEContainer {
	
	// components of the DialogBox
	public TextArea dialogArea;
	
	public GUIStyle textAreaButtonStyle;
	
	public DialogBox() {}
	public void Init(SDEContainer parent, string text) {
		base.Init(parent);
		Init(text);
	}
	
	public void Init(Node parentNode, string text) {
		base.Init(parentNode);
		Init(text);
	}
	
	private void Init(string text) {
		this.dialogArea = ScriptableObject.CreateInstance<TextArea>();
		this.dialogArea.Init(this, text, NodeManager.NODE_WIDTH);
		this.dialogArea.OnDeselect = UpdateInterrupts;
		
		// make the outpoint a child of the dialogArea, so it's bound to that field.
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this.dialogArea, ConnectionPointType.Out);
		
		// set the button styles
		this.textAreaButtonStyle = TextAreaManager.textAreaButtonStyle;
	}
	
	public override void Draw() {
		Rect refRect;
		if (parentNode != null) {
			refRect = parentNode.rect;
		} else {
			refRect = parent.rect;
		}
		
		// update container position
		rect.x = refRect.x;
		rect.y = refRect.y + refRect.height;
		
		// draw components
		dialogArea.Draw();
		outPoint.Draw();
		
		// update container size
		rect.width = dialogArea.clickRect.width + outPoint.rect.width;
		rect.height = dialogArea.clickRect.height;
		
		if (child != null) {
			child.Draw();
		}
		
		// draw remove button
		// don't draw the remove button if its the only child of a Node
		if (child != null || parent != null) {
			if (GUI.Button(new Rect(rect.x-11, rect.y + rect.height/2 - 6, 12, 12), "-", textAreaButtonStyle)) {
				Remove();
			}
		}
	}
	
	public override void ProcessEvent(Event e) {
		// process children first
		if (child != null) {
			child.ProcessEvent(e);
		}
		
		// process component events
		dialogArea.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// check for context menu
			if (e.button == 1 && rect.Contains(e.mousePosition)) {
				ProcessContextMenu();
				e.Use();
			}
			break;
			
		case EventType.KeyDown:
			// check for Tab & Shift+Tab cycling
						
			if (e.keyCode == KeyCode.Tab && dialogArea.Selected) {
				if (e.shift) {
					CycleFocusUp();
				} else {
					CycleFocusDown();
				}
				e.Use();
			}
			break;
		}
	}
	
	/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Dialog"), false, Remove);
		genericMenu.ShowAsContext();
	}
	
	private void CycleFocusDown() {
		Debug.Log("DialogBox: cycling down");
		SDEContainer newFocusedDialogBox = this;
		
		if (child != null) {
			newFocusedDialogBox = child;
		} else if (parent != null) {
			// if at the bottom of the TextArea stack, jump back to the top
			while(newFocusedDialogBox.parent != null) {
				newFocusedDialogBox = newFocusedDialogBox.parent;
			}
		}
		
		// transfer selection state
		dialogArea.Selected = false;
		((DialogBox)newFocusedDialogBox).dialogArea.Selected = true;

		// pass keyboard control
		GUIUtility.keyboardControl = ((DialogBox)newFocusedDialogBox).dialogArea.textID;
	}
	
	private void CycleFocusUp() {
		Debug.Log("DialogBox: cycling up");
		SDEContainer newFocusedDialogBox = this;
		
		if (parent != null) {
			newFocusedDialogBox = parent;
		} else if (child != null) {
			// if at the top of the DialogBox stack, jump back to the bottom
			while (newFocusedDialogBox.child != null) {
				newFocusedDialogBox = newFocusedDialogBox.child;
			}
		}
		
		// transfer Selection state
		dialogArea.Selected = false;
		((DialogBox)newFocusedDialogBox).dialogArea.Selected = true;
		
		// pass keyboard control
		GUIUtility.keyboardControl = ((DialogBox)newFocusedDialogBox).dialogArea.textID;
	}
	
	/*
	  UpdateInterrupts() heavily modifies the editor by updating connected InterruptNodes
	  and associated connections depending on the interrupt flags defined in the textArea.
	
	  NOTE: this performs write operations WITHOUT tracking history, because it should be
	  seamless updates that the user didn't input.
	*/
	private void UpdateInterrupts(SDEComponent textArea) {
		//HistoryManager.RecordEditor();		
		
		string text = ((TextArea)textArea).text;
		
		// parse the text for interrupts flags
		List<string> flags;
		try {
			flags = GetFlags(text);
		} catch (UnityException e) {
			Debug.Log(e.Message);
			return;
		}
		
		// find an Interrupt Node that's connected to this
		Node interruptNode = DialogBoxManager.GetInterruptNode(outPoint);
		if (interruptNode == null) {
			interruptNode = ConnectInterruptNode();
		} else {
			Debug.Log("found interrupt");
		}
		
		// update the Interrupt Node's bottom level status
		if (child == null) {
			interruptNode.SetBottomLevelInterrupt(true);
		} else {
			interruptNode.SetBottomLevelInterrupt(false);
		}
		
		// update the Interrupt Node
		DialogInterrupt interrupt = (DialogInterrupt)interruptNode.childContainer;
		List<DialogInterrupt> oldInterrupts = new List<DialogInterrupt>();
		
		// remove all the old Interrupt Nodes, but queue the interrupts that need to be added.
		while(interrupt != null) {
			if (flags.Contains(interrupt.label.text)) {
				oldInterrupts.Add(interrupt);
				SDEContainerManager.RemoveContainer(interrupt, removeConnections: false, markHistory: false);
			} else {
				SDEContainerManager.RemoveContainer(interrupt, removeConnections: true, markHistory: false);
			}
			
			interrupt = (DialogInterrupt)interrupt.child;
		}
		
		// rebuild the nodes
		bool appendNode = true;
		bool foundMatch = false;
		DialogInterrupt newInterrupt = null;
		for (int i = 0; i < flags.Count; i++) {
			// look for pre-existing nodes that match the flag
			for (int j = 0; j < oldInterrupts.Count; j++) {
				if (flags[i] == oldInterrupts[j].label.text) {
					newInterrupt = oldInterrupts[j];
					oldInterrupts.RemoveAt(j);
					foundMatch = true;
					break;
				}
			}
			
			if (!foundMatch) {
				newInterrupt = ScriptableObject.CreateInstance<DialogInterrupt>();
				newInterrupt.Init();
				newInterrupt.label.text = flags[i];
			}
			
			// guarantee that we are dealing with a new, unlinked Container.
			SDEContainerManager.CleanLinks(newInterrupt);
			
			if (appendNode) {
				SDEContainerManager.InsertChild(interruptNode, newInterrupt);
				appendNode = false;
			} else {
				SDEContainerManager.InsertChild(interrupt, newInterrupt);
			}
			
			interrupt = newInterrupt;
			foundMatch = false;
		}
		
		//HistoryManager.FlushEditor();
	}
	
	private List<string> GetFlags(string text) {
		// parse the text for interrupts flags
		List<string> flags = new List<string>();
		int flagStart = 0;
		int flagLength = 0;
		bool startFlag = false;
		char c;
		for (int i = 0; i < text.Length; i++) {
			c = text[i];
			switch (c) {
			case '<':
				if (startFlag) {
					throw new UnityException("Invalid Interrupt Flags!");
				}
				startFlag = true;
				flagStart = i+1;
				flagLength = 0;
				break;
			case '>':
				if (!startFlag) {
					throw new UnityException("Invalid Interrupt Flags!");
				}
				startFlag = false;
				if (flags.Contains(text.Substring(flagStart, flagLength))) {
					throw new UnityException("Duplicate Interrupt Flags!");
				}
				flags.Add(text.Substring(flagStart, flagLength));
				break;
			default:
				flagLength++;
				break;
			}
		}
		
		return flags;
	}
	
	/*
	  ConnectInterruptNode() creates/splices an Interrupt node to the DialogBox's
	  output ConnectionPoint.
	*/
	private Node ConnectInterruptNode() {
		// if no Interrupt Node is connected, check if there's a connection to
		// splice one between
		ConnectionPoint destinationPoint = null;
		List<Connection> connections = outPoint.connections;
		
		// TODO: only one connection can be paired with an output, when that is
		// refactored, fix this!
		if (connections.Count > 0) {
			destinationPoint = connections[0].inPoint;
		}
		
		// create a new Interrupt Node and connect them
		Vector2 nodeRect = new Vector2(rect.x+(rect.width*1.2f), rect.y+5f);
		Node interruptNode = NodeManager.AddNodeAt(nodeRect, NodeType.Interrupt, markHistory: true);
		
		ConnectionManager.selectedInPoint = interruptNode.inPoint;
		ConnectionManager.selectedOutPoint = outPoint;
		ConnectionManager.CreateConnection(false, markHistory: true);
		
		// do the splicing
		if (destinationPoint != null) {
			ConnectionManager.RemoveConnection(connections[0]);
			
			ConnectionManager.selectedInPoint = destinationPoint;
			ConnectionManager.selectedOutPoint = interruptNode.outPoint;
			ConnectionManager.CreateConnection(true, markHistory: true);
		}
		
		ConnectionManager.ClearConnectionSelection();
		
		return interruptNode;
	}
	
	/*
	  Remove() is a wrapper for the DialogBoxManager's RemoveDialogBox function, so it
	  can be passed to the ContextMenu's menu function argument.
	*/
	public void Remove() {
		HistoryManager.RecordEditor();
		
		Node interruptNode = DialogBoxManager.GetInterruptNode(outPoint);
		SDEContainerManager.RemoveContainer(this, markHistory: false);
		if (interruptNode != null) {
			Debug.Log("deleting interrupt");
			NodeManager.RemoveNode(interruptNode, markHistory: false);
		} 
		
		HistoryManager.FlushEditor();
	}
}
