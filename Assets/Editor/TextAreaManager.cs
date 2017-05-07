using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TextAreaManager{
	public static StoryDialogEditor mainEditor;
	
	public static GUIStyle textAreaStyle;
	public static GUIStyle textBoxDefault;
	public static GUIStyle textBoxSelected;
	
	public const float X_PAD = 5f;
	public const float Y_PAD = 5f;
	
	/*
	  RemoveTextArea() removes the given TextArea from its Node, and stitches
	  the parent and child components together, removing any old connections.
	*/
	public static void RemoveTextArea(TextArea textArea) {
		// the parent, child, text area itself, and mainEditor all get modified
		Undo.RecordObject(textArea.parent, "removing text area...");
		Undo.RecordObject(textArea, "removing text area...");
		Undo.RecordObject(textArea.child, "removing text area...");
		Undo.RecordObject(mainEditor, "removing ConnectionPoint connections...");
		
		// stitch the parent to the child
		SDEComponent parent = textArea.parent;
		SDEComponent child = textArea.child;
		if (parent != null && child != null) {
			parent.child = child;
			child.parent = parent;
		} else if(parent != null && parent.componentType != SDEComponentType.Node) {
			parent.child = null;
		} else {
			throw new UnityException("Tried to remove TextArea with erroneous parent/child!");
		}
		
		// remove all associated connections
		List<Connection> connectionsToRemove = ConnectionManager.GetConnections(textArea.outPoint);
		for (int i = 0; i < connectionsToRemove.Count; i++) {
			mainEditor.connections.Remove(connectionsToRemove[i]);
		}
		connectionsToRemove = null;
		
		// free the textArea itself
		textArea = null;
		
		Undo.FlushUndoRecordObjects();
	}
}
