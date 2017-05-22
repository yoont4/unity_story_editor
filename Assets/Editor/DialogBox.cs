using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogBox : DBox {
	
	public DialogBox() {}
	public override void Init(SDEContainer parent, string text) {
		base.Init(parent, text);
		Init();
	}
	
	public override void Init(Node parentNode, string text) {
		base.Init(parentNode, text);
		Init();
	}
	
	private void Init() {
		// Hook up updates and text undo stacking
		this.textArea.OnDeselect = UpdateInterrupts;
		this.textArea.OnSelect = UpdateInterrupts;
		
		// assign its container type
		this.containerType = SDEContainerType.DialogBox;
	}
	
	public override void Draw() {
		base.Draw();
	}
	
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
	}
	
	/*
	  UpdateInterrupts() heavily modifies the editor by updating connected InterruptNodes
	  and associated connections depending on the interrupt flags defined in the textArea.
	*/
	private void UpdateInterrupts(SDEComponent textComponent) { 
		HistoryManager.RecordEditor();		
		
		string text = ((TextArea)textComponent).text;
		
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
				if (flagLength > 0) {
					flags.Add(text.Substring(flagStart, flagLength));
				} else {
					int start = Mathf.Max(0, flagStart-10);
					Debug.Log("Empty flag at index " + flagStart + ": " + "\"..." + text.Substring(start, Mathf.Min(text.Length-start, 20)) + "...\"");
				}
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
		Node interruptNode = NodeManager.AddNodeAt(nodeRect, NodeType.Interrupt, markHistory: false);
		
		ConnectionManager.selectedInPoint = interruptNode.inPoint;
		ConnectionManager.selectedOutPoint = outPoint;
		ConnectionManager.CreateConnection(false, markHistory: false);
		
		// do the splicing
		if (destinationPoint != null) {
			ConnectionManager.RemoveConnection(connections[0]);
			
			ConnectionManager.selectedInPoint = destinationPoint;
			ConnectionManager.selectedOutPoint = interruptNode.outPoint;
			ConnectionManager.CreateConnection(true, markHistory: false);
		}
		
		ConnectionManager.ClearConnectionSelection();
		
		return interruptNode;
	}
	
	/*
	  Remove() removes the DialogBox and the accompanied InterruptNode
	*/
	public override void Remove() {
		// only remove if there are other dialog boxes
		if (parentNode != null && child == null) {
			Debug.Log("Can't remove the last DialogBox!");
			return;
		}
		
		HistoryManager.RecordEditor();
		
		Node interruptNode = DialogBoxManager.GetInterruptNode(outPoint);
		SDEContainerManager.RemoveContainer(this, markHistory: false);
		if (interruptNode != null) {
			Debug.Log("deleting interrupt");
			NodeManager.RemoveNode(interruptNode, markHistory: false);
		} 
	}
}
