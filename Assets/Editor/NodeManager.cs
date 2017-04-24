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
	public static List<Node> nodes;
	
	public static GUIStyle defaultNodeStyle;
	public static GUIStyle selectedNodeStyle;
	
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 100;
	
	// this is used in conjunction with the Node ProcessEvents
	// stack, to prevent overlapping lower-order Nodes from being 
	// selected at the same time.
	public static Node selectedNode;
	
	public static void DrawNodes() {
		if (nodes != null) {
			for (int i = 0; i < nodes.Count; i++) {
				nodes[i].Draw();
			}
		}
	}
	
	public static void ProcessEvents(Event e) {
		if (nodes != null) {
			// reset for every click processing
			selectedNode = null;
			
			// processed backwards because nodes on the top are rendered on top
			for (int i = nodes.Count - 1; i >= 0; i--) {
				bool guiChanged = nodes[i].ProcessEvent(e);
				if (guiChanged) {
					GUI.changed = true;
				}
			}
		}
	}
	
	public static void OnClickRemoveNode(Node node) {
		if (ConnectionManager.connections != null) {
			List<Connection> connectionsToRemove = new List<Connection>();
			for (int i = 0; i < ConnectionManager.connections.Count; i++) {
				if (ConnectionManager.connections[i].inPoint == node.inPoint || ConnectionManager.connections[i].outPoint == node.outPoint) {
					connectionsToRemove.Add(ConnectionManager.connections[i]);
				}
			}
			
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				ConnectionManager.connections.Remove(connectionsToRemove[i]);
			}
			
			connectionsToRemove = null;
		}
		
		nodes.Remove(node);
	}
	
	public static void OnClickAddNode(Vector2 mousePosition) {
		if (nodes == null) {
			nodes = new List<Node>();
		}
		
		Vector2 nodePosition = new Vector2(mousePosition.x - NODE_WIDTH/2, mousePosition.y - NODE_HEIGHT/2);
		
		nodes.Add(new Node(
			nodePosition, NODE_WIDTH, NODE_HEIGHT, 
			defaultNodeStyle, selectedNodeStyle, 
			OnClickRemoveNode)
		);
	}

}
