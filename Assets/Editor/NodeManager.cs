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
	public static GUIStyle nodeInterruptDefault;
	public static GUIStyle nodeInterruptSelected;
	
	// defines Node dimensions
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 27;
	public const int INTERRUPT_WIDTH = 40;
	public const int INTERRUPT_HEIGHT = 20;
	
	
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
		RemoveNode(node, markHistory: true);
	}
	
	public static void RemoveNode(Node node, bool markHistory=true) {
		if (markHistory) {
			HistoryManager.RecordEditor();
		}
		
		// build the list of Connections to remove
		List<Connection> connectionsToRemove = new List<Connection>();
		connectionsToRemove.AddRange(node.inPoint.connections);
		
		// remove any associated Interrupt Nodes and Connections
		if (node.nodeType == NodeType.Dialog) {
			RemoveInterruptNodes(connectionsToRemove, node);
		} else {
			connectionsToRemove.AddRange(GetConnections(node));
		}
		
		// remove all the connections from the global list of connections.
		for (int i = 0; i < connectionsToRemove.Count; i++) {
			ConnectionManager.RemoveConnection(connectionsToRemove[i], markHistory: false);
		}
		
		
		// free the reference for GC
		connectionsToRemove = null;
		
		// remove the node from the global node list and the SelectionManager
		mainEditor.nodes.Remove(node);
		SelectionManager.Deselect(node);
	}
	
	/*
	  RemoveInterruptNodes() is a helper function for RemoveNode() that removes the associated
	  Interrupt Nodes and appends the Connections of the given Node.
	*/
	private static void RemoveInterruptNodes(List<Connection> connectionsToRemove, Node node) {
		// get all the associated connections to clear
		SDEContainer tempContainer = node.childContainer;
		List<Connection> tempConnections;
		List<Node> nodesToRemove = new List<Node>();
		Node tempNode;
		
		while (tempContainer != null) {
			// get all the connections of the Node's child DialogBoxes
			tempConnections = tempContainer.outPoint.connections;
			connectionsToRemove.AddRange(tempConnections);
			
			// get all the connected nodes
			for (int i = 0; i < tempConnections.Count; i++) {
				tempNode = (Node)tempConnections[i].inPoint.parent;
				if (tempNode.nodeType == NodeType.Interrupt) {
					// queue the Interrupt Node to be removed
					nodesToRemove.Add(tempNode);
					
					// add the default Interrupt Node output connections
					connectionsToRemove.AddRange(GetConnections(tempNode));
				}
			}
			
			tempContainer = tempContainer.child;
		}
		
		// remove the associated interrupt nodes
		for (int i = 0; i < nodesToRemove.Count; i++) {
			mainEditor.nodes.Remove(nodesToRemove[i]);
		}
		nodesToRemove = null;
	}
	
	private static List<Connection> GetConnections(Node node) {
		SDEContainer tempContainer = node.childContainer;
		List<Connection> connections = new List<Connection>();
		
		if (node.outPoint != null) {
			connections.AddRange(node.outPoint.connections);
		}
		
		while (tempContainer != null) {
			connections.AddRange(tempContainer.outPoint.connections);
			tempContainer = tempContainer.child;
		}
		
		return connections;
	}
	
	/*
	  AddNoteAt() creates a new Node at the given mouse position.
	*/
	public static Node AddNodeAt(Vector2 nodePosition, NodeType type, bool markHistory=true) {
		if (markHistory) {
			HistoryManager.RecordEditor();
		}
		
		if (mainEditor.nodes == null) {
			mainEditor.nodes = new List<Node>();
		}
		
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
		
		// add node as close to center as possible while staying on grid
		nodePosition.x -= (width/2) - (width/2) % StoryDialogEditor.GRID_SIZE;
		nodePosition.y -= (height/2) - (height/2) % StoryDialogEditor.GRID_SIZE;
		
		Node newNode = ScriptableObject.CreateInstance<Node>();
		newNode.Init(
			nodePosition, width, height, 
			nodeDefault, nodeSelected, 
			RemoveNode, type);
		
		mainEditor.nodes.Add(newNode);
		
		return newNode;
	}
}
