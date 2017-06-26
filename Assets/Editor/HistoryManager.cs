using UnityEngine;
using UnityEditor;

public static class HistoryManager {
	public static StoryDialogEditor mainEditor;
	public static bool needsFlush;
	public static int savedUndoGroup;
	
	// TODO: optimize this, because it's slow as shit
	public static void RecordEditor() {
		Undo.RegisterCompleteObjectUndo(mainEditor, "");
		
		// record all nodes
		if (mainEditor.nodes != null) {
			foreach (Node node in mainEditor.nodes) {
				RecordNodeHierarchy(node);
			}
		}
		
		// record all connections and connection points
		if (mainEditor.connections != null) {
			foreach(Connection connection in mainEditor.connections) {
				Undo.RecordObject(connection, "");
			}
		}
		
		needsFlush = true;
	}
	
	public static void RecordNode(Node node) {
		Undo.RegisterCompleteObjectUndo(node, "");
		Undo.RecordObject(node.inPoint, "");
		if (node.outPoint != null) {
			Undo.RecordObject(node.outPoint, "");
		}
		needsFlush = true;
	}
	
	public static void RecordNodeHierarchy(Node node) {
		RecordNode(node);
		
		SDEContainer tempContainer = node.childContainer;
		while (tempContainer != null) {
			RecordContainer(tempContainer);
			tempContainer = tempContainer.child;
		}
		needsFlush = true;
	}
	
	public static void RecordContainer(SDEContainer container) {
		// record the container values itself
		Undo.RegisterCompleteObjectUndo(container, "");
		
		switch(container.containerType) {
		case SDEContainerType.DialogBox:
			Undo.RegisterCompleteObjectUndo(((DialogBox)container).textArea, "");
			break;
			
		default:
			break;
		}
		
		// all containers have the outpoint
		Undo.RecordObject(container.outPoint, "");
		needsFlush = true;
	}
	
	public static void RecordCompleteComponent(SDEComponent component) {
		Undo.RegisterCompleteObjectUndo(component, "");
		needsFlush = true;
	}
	
	public static void RecordDropdown(DropdownEditableList menu) {
		Undo.RecordObject(menu, "");
		foreach (TextArea item in menu.items) {
			Undo.RecordObject(item, "");
		}
		
		needsFlush = true;
	}
	
	// NOTE: NewComponent() and DeleteComponent() seem to have weird behavior. 
	// i.e. not properly adding the new object to  the undo stack. Maybe 
	// these are really designed for only GameObjects?
	public static void NewComponent(Object component) {
		Undo.RegisterCreatedObjectUndo(component, "");
		needsFlush = true;
	}
	
	public static void DeleteComponent(Object component) {
		Undo.DestroyObjectImmediate(component);
		needsFlush = true;
	}
	
	public static void FlushIfDirty() {
		if (needsFlush) {
			Undo.FlushUndoRecordObjects();
			Undo.IncrementCurrentGroup();
		}
		
		needsFlush = false;
	}
}
