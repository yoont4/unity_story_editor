using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  NodeManager serves as an intermediate between individual Nodes and 
  the StoryDialogueEditor.
  
  This allows the organization of Action<Node> to be removed from the 
  StoryDialogueEditor itself, without cloning a copy of the main editor
  into every single Node object.
*/
public static class NodeManager {
	
	public static StoryDialogueEditor mainEditor;
	
	// the global list of connections
	public static List<Node> nodes;
	
	// all Nodes use these styles
	public static GUIStyle nodeDefault;
	public static GUIStyle nodeSelected;
	
	// defines Node dimensions
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 40;
	
	/*
	  DrawNodes() draws all the nodes in the StoryDialogueEditor window.
	*/
	public static void DrawNodes() {
		if (nodes != null) {
			for (int i = 0; i < nodes.Count; i++) {
				nodes[i].Draw();
			}
		}
	}
	
	/*
	  ProcessEvents() goes through all the nodes and processes their events.
	*/
	public static void ProcessEvents(Event e) {
		if (nodes != null) {
			// processed backwards because nodes on the top are rendered on top
			for (int i = nodes.Count - 1; i >= 0; i--) {
				nodes[i].ProcessEvent(e);
			}
		}
	}
	
	/*
	  RemoveNode() removes the given Node from the global list of nodes, and 
	  destroys any connections it was part of.
	*/
	public static void RemoveNode(Node node) {
		if (ConnectionManager.connections != null) {
			// build the list of Connections to remove
			List<Connection> connectionsToRemove = new List<Connection>();
			for (int i = 0; i < ConnectionManager.connections.Count; i++) {
				if (ConnectionManager.connections[i].inPoint == node.inPoint || ConnectionManager.connections[i].outPoint == node.outPoint) {
					connectionsToRemove.Add(ConnectionManager.connections[i]);
				}
			}
			
			// remove all the connections from the global list of connections.
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				ConnectionManager.connections.Remove(connectionsToRemove[i]);
			}
			
			// free the reference for GC
			connectionsToRemove = null;
		}
		
		// remove the node from the global node list and the SelectionManager
		nodes.Remove(node);
		SelectionManager.Deselect(node);
	}
	
	/*
	  AddNoteAt() creates a new Node at the given mouse position.
	*/
	public static void AddNodeAt(Vector2 mousePosition) {
		if (nodes == null) {
			nodes = new List<Node>();
		}
		
		// spawn the node so it is centered on the mouse
		Vector2 nodePosition = new Vector2(mousePosition.x - NODE_WIDTH/2, mousePosition.y - NODE_HEIGHT/2);
		
		nodes.Add(new Node(
			nodePosition, NODE_WIDTH, NODE_HEIGHT, 
			nodeDefault, nodeSelected, 
			RemoveNode)
		);
	}

}
