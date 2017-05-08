using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBox : SDEContainer {
	
	// TODO: hook these up
	public TextArea dialog;
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
		//this.dialog = ScriptableObject.CreateInstance<TextArea>();
		//this.dialog.Init(this, text);
		
		// make the outpoint a child of the dialog, so it's bound to that field.
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this.dialog, ConnectionPointType.Out);
		
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
		dialog.Draw();
		outPoint.Draw();
		
		// update container size
		rect.width = dialog.clickRect.width + outPoint.rect.width;
		rect.height = dialog.clickRect.height;
		
		// draw remove button
		// don't draw the remove button if its the only child of a Node
		if (child != null || parentNode != null) {
			if (GUI.Button(new Rect(rect.x-11, rect.y + rect.height/2 - 6, 12, 12), "-", textAreaButtonStyle)) {
				// TODO: implement this
				//Remove();
			}
		}
	}
	
	public override void ProcessEvent(Event e) {
		// TODO: implement this
	}
}
