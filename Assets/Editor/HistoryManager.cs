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
					Undo.RecordObject(tempContainer, "");
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
