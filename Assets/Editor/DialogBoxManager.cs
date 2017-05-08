using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DialogBoxManager {
	
	public static StoryDialogEditor mainEditor;
	
	/*
	  RemoveTextArea() removes the given TextArea from its Node, and stitches
	  the parent and child components together, removing any old connections.
	*/
	public static void RemoveDialogBox(DialogBox dialogBox) {
		// the parent, child, text area itself, and mainEditor all get modified
		if (dialogBox.parent != null) {
			Undo.RecordObject(dialogBox.parent, "removing text area...");	
		}
		if (dialogBox.parentNode != null) {
			Undo.RecordObject(dialogBox.parentNode, "removing text area...");	
		}
		if (dialogBox.child != null) {
			Undo.RecordObject(dialogBox.child, "removing text area...");
		}
		Undo.RecordObject(dialogBox, "removing text area...");
		Undo.RecordObject(mainEditor, "removing ConnectionPoint connections...");
		
		// TODO: implement this properly
		//// stitch the parent to the child
		//SDEComponent parent = dialogBox.parent;
		//SDEComponent child = dialogBox.child;
		//if (parent != null && child != null) {
		//	parent.child = child;
		//	child.parent = parent;
		//} else if(parent != null && parent.componentType != SDEComponentType.Node) {
		//	parent.child = null;
		//} else {
		//	throw new UnityException("Tried to remove TextArea with erroneous parent/child!");
		//}
		
		//// remove all associated connections
		//List<Connection> connectionsToRemove = ConnectionManager.GetConnections(dialogBox.outPoint);
		//for (int i = 0; i < connectionsToRemove.Count; i++) {
		//	mainEditor.connections.Remove(connectionsToRemove[i]);
		//}
		//connectionsToRemove = null;
		
		//// free the dialogBox itself
		//dialogBox = null;
		
		Undo.FlushUndoRecordObjects();
	}
}
