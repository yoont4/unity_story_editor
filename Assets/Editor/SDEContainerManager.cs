using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SDEContainerManager {
	public static StoryDialogEditor mainEditor;
	
	public static void RemoveContainer(SDEContainer container, bool removeConnections=true, bool markHistory=true) {
		if (markHistory) {
			HistoryManager.RecordEditor();
		}
		
		// stitch the parent to the child
		Node parentNode = container.parentNode;
		SDEContainer parent = container.parent;
		SDEContainer child = container.child;
		
		if (parentNode != null) {
			if (child != null) {
				// stitch the Node and the Container child together
				parentNode.childContainer = child;
				
				child.parentNode = parentNode;
				child.parent = null;
			} else {
				// TODO: figure out how to make this Container type dependent
				parentNode.childContainer = null;
				// tried to remove last Container of the Node!
				//throw new UnityException("Can't remove last Container of a Node!");
			}
		} else {
			if (parent != null && child != null) {
				// switch the parent and child Container together
				parent.child = child;
				child.parent = parent;
			} else if (parent != null && child == null) {
				// removing the last Container of the Node
				parent.child = null;
			} else {
				// something bad happened!
				throw new UnityException("Tried to Remove Container with erroneous parent/child!");
			}
		}
		
		// remove all associated connections
		if (removeConnections) {
			List<Connection> connectionsToRemove = container.outPoint.connections;
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				mainEditor.connections.Remove(connectionsToRemove[i]);
				ConnectionManager.RemoveConnectionHistory(connectionsToRemove[i]);
			}
			connectionsToRemove = null;
		}
		
		if (markHistory) {
			HistoryManager.Flush();
		}
	}
	
	public static void CleanLinks(SDEContainer container) {
		container.parent = null;
		container.parentNode = null;
		container.child = null;
	}
	
	public static void InsertParent(SDEContainer container, SDEContainer parent) {
		if (container.parentNode != null) {
			container.parentNode.childContainer = parent;
			parent.parentNode = container.parentNode;
			parent.parent = null;
		} else {
			container.parent.child = parent;
			parent.parent = container.parent;
			parent.parentNode = null;
		}
		container.parentNode = null;
		
		container.parent = parent;
		parent.child = container;
	}
	
	public static void InsertChild(SDEContainer container, SDEContainer child) {
		if (container.child != null) {
			container.child.parent = child;
			container.child.parentNode = null;
			child.child = container.child;
		}
		child.parentNode = null;
		
		container.child = child;
		child.parent = container;
		
	}
	
	public static void InsertChild(Node node, SDEContainer child) {
		if (node.childContainer != null) {
			node.childContainer.parent = child;
			node.childContainer.parentNode = null;
			child.child = node.childContainer;
		}
		child.parent = null;
		
		node.childContainer = child;
		child.parentNode = node;
	}
}
