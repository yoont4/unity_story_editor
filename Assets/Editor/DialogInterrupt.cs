using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  DialogInterrupts are the containers that handle conditional
  branching on interrupt types in the Story Dialog.
*/
public class DialogInterrupt : SDEContainer {
	
	public DialogInterrupt() {}
	
	public override void Init(SDEContainer parent) {
		base.Init(parent);
	}
	
	public override void Init(Node parentNode) {
		base.Init(parentNode);
	}
	
	public override void Draw() {
		Rect refRect;
		if (parentNode != null) {
			refRect = parentNode.rect;
		} else {
			refRect = parent.rect;
		}
		
		rect.x = refRect.x;
		rect.y = refRect.y + refRect.height;
		rect.width = refRect.width;
		rect.height = 20f;
		
		GUI.Box(rect, "test");
		//TODO: implement this
	}
	
	public override void ProcessEvent(Event e) {
		//TODO: implement this		
	}
}
