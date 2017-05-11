using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  NodeManager serves as an intermediate between individual Nodes and 
  the StoryDialogEditor.
  
  This allows the organization of Action<Node> to be removed from the 
  StoryDialogEditor itself, without cloning a copy of the main editor
  into every single Node object.
*/
public static class NodeManager {
	
	public static StoryDialogEditor mainEditor;
	
	// all Nodes use these styles
	public static GUIStyle nodeDefault;
	public static GUIStyle nodeSelected;
	
	// defines Node dimensions
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 27;
	public const int INTERRUPT_WIDTH = 100;
	public const int INTERRUPT_HEIGHT = 27;
	
	
	/*
	  DrawNodes() draws all the mainEditor.nodes in the StoryDialogEditor window.
	*/
	public static void DrawNodes() {
		if (mainEditor.nodes != null) {
			for (int i = 0; i < mainEditor.nodes.Count; i++) {
				mainEditor.nodes[i].Draw();
			}
		}
	}
	
	/*
	  ProcessEvents() goes through all the mainEditor.nodes and processes their events.
	*/
	public static void ProcessEvents(Event e) {
		if (mainEditor.nodes != null) {
			// processed backwards because mainEditor.nodes on the top are rendered on top
			for (int i = mainEditor.nodes.Count - 1; i >= 0; i--) {
				mainEditor.nodes[i].ProcessEvent(e);
			}
		}
	}
	
	/*
	  RemoveNode() removes the given Node from the global list of mainEditor.nodes, and 
	  destroys any connections it was part of.
	*/
	public static void RemoveNode(Node node) {
		Undo.RecordObject(mainEditor, "removing node and associated connections...");
		
		// build the list of Connections to remove
		List<Connection> connectionsToRemove = ConnectionManager.GetConnections(node.inPoint);
		
		// remove all the connections from the global list of connections.
		for (int i = 0; i < connectionsToRemove.Count; i++) {
			mainEditor.connections.Remove(connectionsToRemove[i]);
		}
		
		// free the reference for GC
		connectionsToRemove = null;
		
		// remove the node from the global node list and the SelectionManager
		mainEditor.nodes.Remove(node);
		SelectionManager.Deselect(node);
		
		Undo.FlushUndoRecordObjects();
	}
	
	/*
	  AddNoteAt() creates a new Node at the given mouse position.
	*/
	public static void AddNodeAt(Vector2 nodePosition, NodeType type) {
		Undo.RecordObject(mainEditor, "adding node at...");
		
		if (mainEditor.nodes == null) {
			mainEditor.nodes = new List<Node>();
		}
		
		// add node as close to center as possible while staying on grid
		nodePosition.x -= (NODE_WIDTH/2) - (NODE_WIDTH/2) % StoryDialogEditor.GRID_SIZE;
		nodePosition.y -= (NODE_HEIGHT/2) - (NODE_HEIGHT/2) % StoryDialogEditor.GRID_SIZE;
		
		float width;
		float height;
		switch(type) {
		case NodeType.Interrupt:
			width = INTERRUPT_WIDTH;
			height = INTERRUPT_HEIGHT;
			break;
		default:
			width = NODE_WIDTH;
			height = NODE_HEIGHT;
			break;
		}
		
		Node newNode = ScriptableObject.CreateInstance<Node>();
		newNode.Init(
			nodePosition, width, height, 
			nodeDefault, nodeSelected, 
			RemoveNode);
		
		mainEditor.nodes.Add(newNode);
		
		Undo.FlushUndoRecordObjects();
	}
}
