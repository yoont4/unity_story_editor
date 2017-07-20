using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  DBox is the parent class of both DialogBoxes and DecisionBoxes.
*/
public abstract class DBox : SDEContainer {
	public TextArea textArea;
	public GUIStyle textButtonStyle;
	
	public DBox() {}
	public virtual void Init(SDEContainer parent, string text) {
		base.Init(parent);
		Init(text);
	}
	
	public virtual void Init(Node parentNode, string text) {
		base.Init(parentNode);
		Init(text);
	}
	
	private void Init(string text) {
		this.textArea = ScriptableObject.CreateInstance<TextArea>();
		this.textArea.Init(this, text, NodeManager.TEXT_NODE_WIDTH);
		
		// make the outpoint a child of the textArea, so it's bound to that field.
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this.textArea, ConnectionPointType.Out);
		
		// set the button styles
		this.textButtonStyle = SDEStyles.textButtonDefault;
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
		
		// draw components
		textArea.Draw();
		outPoint.Draw();
		
		// update container size
		rect.width = textArea.clickRect.width + outPoint.rect.width;
		rect.height = textArea.clickRect.height;
		
		if (child != null) {
			child.Draw();
		}
		
		// draw remove button
		// don't draw the remove button if its the only child of a Node
		if (child != null || parent != null) {
			if (GUI.Button(new Rect(rect.x-11, rect.y + rect.height/2 - 6, 12, 12), "-", textButtonStyle)) {
				Remove();
			}
		}
	}
	
	public override void ProcessEvent(Event e) {
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
			if (e.keyCode == KeyCode.Tab && textArea.Selected) {
				if (e.shift) {
					CycleFocusUp();
				} else {
					CycleFocusDown();
				}
				e.Use();
			}
			break;
		}
		
		// process component events
		textArea.ProcessEvent(e);
		outPoint.ProcessEvent(e);
		
		// process children last
		if (child != null) {
			child.ProcessEvent(e);
		}
	}
		/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove Text Box"), false, Remove);
		genericMenu.ShowAsContext();
	}
	
	private void CycleFocusDown() {
		SDEContainer newFocusedDialogBox = this;
		
		if (child != null) {
			newFocusedDialogBox = child;
		} else if (parent != null) {
			// if at the bottom of the TextArea stack, jump back to the top
			while (newFocusedDialogBox.parent != null) {
				newFocusedDialogBox = newFocusedDialogBox.parent;
			}
		}
		
		ShiftFocus((DBox)newFocusedDialogBox);
	}
	
	private void CycleFocusUp() {
		SDEContainer newFocusedDialogBox = this;
		
		if (parent != null) {
			newFocusedDialogBox = parent;
		} else if (child != null) {
			// if at the top of the DialogBox stack, jump back to the bottom
			while (newFocusedDialogBox.child != null) {
				newFocusedDialogBox = newFocusedDialogBox.child;
			}
		}
		
		ShiftFocus((DBox)newFocusedDialogBox);
	}
	
	private void ShiftFocus(DBox newFocusedDialogBox) {
		if (newFocusedDialogBox != this) {
			// transfer Selection state
			textArea.Selected = false;
			((DBox)newFocusedDialogBox).textArea.Selected = true;
			
			// pass keyboard control
			GUIUtility.keyboardControl = ((DBox)newFocusedDialogBox).textArea.textID;
		} else {
			// if the new focused box is the same, just call the DialogBox's selection event handler
			textArea.Selected = true;
		}
	}
	
	public abstract void Remove();
	
}
