using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogBox : SDEContainer {
	
	public TextArea dialogArea;
	public ConnectionPoint outPoint;
	
	public GUIStyle textAreaButtonStyle;
	
	public DialogBox() {}
	public void Init(SDEContainer parent, string text) {
		base.Init(parent);
		Init(text);
	}
	
	public void Init(Node parentNode, string text) {
		base.Init(parentNode);
		Init(text);
	}
	
	private void Init(string text) {
		this.dialogArea = ScriptableObject.CreateInstance<TextArea>();
		this.dialogArea.Init(this, text, NodeManager.NODE_WIDTH);
		
		// make the outpoint a child of the dialogArea, so it's bound to that field.
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this.dialogArea, ConnectionPointType.Out);
		
		// set the button styles
		this.textAreaButtonStyle = TextAreaManager.textAreaButtonStyle;
	}
	
	public override void Draw() {
		Rect refRect;
		if (parentNode != null) {
			refRect = parentNode.rect;
		} else {
			refRect = parent.rect;
		}
		
		// update container position
		rect.x = refRect.x;
		rect.y = refRect.y + refRect.height;
		
		// draw children
		dialogArea.Draw();
		outPoint.Draw();
		if (child != null) {
			child.Draw();
		}
		
		// update container size
		rect.width = dialogArea.clickRect.width + outPoint.rect.width;
		rect.height = dialogArea.clickRect.height;
		
		// draw remove button
		// don't draw the remove button if its the only child of a Node
		if (child != null || parent != null) {
			if (GUI.Button(new Rect(rect.x-11, rect.y + rect.height/2 - 6, 12, 12), "-", textAreaButtonStyle)) {
				Remove();
			}
		}
	}
	
	public override void ProcessEvent(Event e) {
		// process component events
		dialogArea.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		
		if (child != null) {
			child.ProcessEvent(e);
		}
		
		switch(e.type) {
		case EventType.MouseDown:
			// check for context menu
			if (e.button == 1 && rect.Contains(e.mousePosition)) {
				ProcessContextMenu();
				e.Use();
			}
			break;
			
		case EventType.KeyDown:
			// check for Tab & Shift+Tab cycling
			if (e.keyCode == KeyCode.Tab && dialogArea.Selected) {
				CycleFocus();
				e.Use();
			}
			break;
		}
	}
	
	/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Dialog"), false, Remove);
		genericMenu.ShowAsContext();
	}
	
	private void CycleFocus() {
		if (child != null) {
			Debug.Log("cycling");
			// transfer selection state
			dialogArea.Selected = false;
			((DialogBox)child).dialogArea.Selected = true;
	
			// pass keyboard control
			GUIUtility.keyboardControl = ((DialogBox)child).dialogArea.textID;
		} else if (parent != null) {
			// if at the bottom of the TextArea stack, jump back to the top
			SDEContainer newFocusedDialogBox = this;
			while(newFocusedDialogBox.parent != null) {
				newFocusedDialogBox = newFocusedDialogBox.parent;
			}
			
			// transfer selection state
			dialogArea.Selected = false;
			((DialogBox)newFocusedDialogBox).dialogArea.Selected = true;
			
			// pass keyboard control
			GUIUtility.keyboardControl = ((DialogBox)newFocusedDialogBox).dialogArea.textID;
		}
	}
	
	/*
	  Remove() is a wrapper for the DialogBoxManager's RemoveDialogBox function, so it
	  can be passed to the ContextMenu's menu function argument.
	*/
	private void Remove() {
		DialogBoxManager.RemoveDialogBox(this);
	}
}
