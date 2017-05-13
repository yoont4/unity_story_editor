using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DialogBoxManager {
	
	public static StoryDialogEditor mainEditor;
	
	public static Node GetInterruptNode(ConnectionPoint point) {
		List<Connection> connections = point.connections;
		if (connections.Count > 0) {
			// look for which connection is tied to an Interrupt Node
			for (int i = 0; i < connections.Count; i++) {
				// safe to cast because only Nodes can have inputs
				if (((Node)connections[i].inPoint.parent).nodeType == NodeType.Interrupt) {
					return (Node)connections[i].inPoint.parent;
				}
			}
		} 
		
		// can't be connected to an Interrupted Node if there are no connections
		return null;
	}
}
