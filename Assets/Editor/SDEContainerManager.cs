using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SDEContainerManager {
	public static StoryDialogEditor mainEditor;
	
	public static void RemoveContainer(SDEContainer container) {
		// the parent, child, text area itself, and mainEditor all get modified
		if (container.parent != null) {
			Undo.RecordObject(container.parent, "removing text area...");	
		}
		if (container.parentNode != null) {
			Undo.RecordObject(container.parentNode, "removing text area...");	
		}
		if (container.child != null) {
			Undo.RecordObject(container.child, "removing text area...");
		}
		Undo.RecordObject(container, "removing text area...");
		Undo.RecordObject(mainEditor, "removing ConnectionPoint connections...");
		
		// stitch the parent to the child
		Node parentNode = container.parentNode;
		SDEContainer parent = container.parent;
		SDEContainer child = container.child;
		
		if (parentNode != null) {
			if (child != null) {
				// stitch the Node and the DialogBox child together
				parentNode.childContainer = child;
				
				child.parentNode = parentNode;
				child.parent = null;
			} else {
				// tried to remove last DialogBox of the Node!
				throw new UnityException("Can't remove last DialogBox of a Node!");
			}
		} else {
			if (parent != null && child != null) {
				// switch the parent and child DialogBox together
				parent.child = child;
				child.parent = parent;
			} else if (parent != null && child == null) {
				// removing the last DialogBox of the Node
				parent.child = null;
			} else {
				// something bad happened!
				throw new UnityException("Tried to RemoveDialogBox with erroneous parent/child!");
			}
		}
		
		// remove all associated connections
		List<Connection> connectionsToRemove = container.outPoint.connections;
		for (int i = 0; i < connectionsToRemove.Count; i++) {
			mainEditor.connections.Remove(connectionsToRemove[i]);
			ConnectionManager.RemoveConnectionHistory(connectionsToRemove[i]);
		}
		connectionsToRemove = null;
		
		Undo.FlushUndoRecordObjects();
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
			child.child = container.child;
		}
		child.parentNode = null;
		
		container.child = child;
		child.parent = container;
		
	}
	
	public static void InsertChild(Node node, SDEContainer child) {
		if (node.childContainer != null) {
			node.childContainer.parent = child;
			child.child = node.childContainer;
		}
		child.parent = null;
		
		node.childContainer = child;
		child.parentNode = node;
	}
}
