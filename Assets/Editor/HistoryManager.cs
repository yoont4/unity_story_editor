using UnityEngine;
using UnityEditor;

public static class HistoryManager {
	public static StoryDialogEditor mainEditor;
	
	public static void RecordEditor() {
		Undo.RegisterCompleteObjectUndo(mainEditor, "");
		
		// record all nodes
		SDEContainer tempContainer;
		if (mainEditor.nodes != null) {
			foreach (Node node in mainEditor.nodes) {
				Undo.RegisterCompleteObjectUndo(node, "");
				Undo.RecordObject(node.inPoint, "");
				if (node.outPoint != null) {
					Undo.RecordObject(node.outPoint, "");
				}
				
				// record all child containers of the node
				tempContainer = node.childContainer;
				while (tempContainer != null) {
					Undo.RegisterFullObjectHierarchyUndo(tempContainer, "");
					
					// TODO: this should be handled separately on all components that take input. It
					// should not happen in a global "RecordEditor" function, it should be unique to input
					// fields themselves. Probably attach as an event handler for on keypress or on change?
					// something to batch multiple key presses for sure.
					if (node.nodeType == NodeType.Dialog) {
						Undo.RegisterCompleteObjectUndo(((DialogBox)tempContainer).dialogArea, "");
					}
					
					Undo.RecordObject(tempContainer.outPoint, "");
					tempContainer = tempContainer.child;
				}
			}
		}
		
		// record all connections and connection points
		if (mainEditor.connections != null) {
			foreach(Connection connection in mainEditor.connections) {
				Undo.RecordObject(connection, "");
			}
		}
		
	}
	
	public static void FlushEditor() {
		Undo.FlushUndoRecordObjects();
	}
	
}
