using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  DialogInterrupts are the containers that handle conditional
  branching on interrupt types in the Story Dialog.
*/
public class DialogInterrupt : SDEContainer {
	
	public SDELabel label;
	
	public DialogInterrupt() {}
	
	public override void Init(SDEContainer parent) {
		base.Init(parent);
		Init();
	}
	
	public override void Init(Node parentNode) {
		base.Init(parentNode);
		Init();
	}
	
	private void Init() {
		this.label = ScriptableObject.CreateInstance<SDELabel>();
		this.label.Init(this, "test", NodeManager.INTERRUPT_WIDTH, NodeManager.INTERRUPT_HEIGHT);
		
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(label, ConnectionPointType.Out);
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
		
		// draw children
		label.Draw();
		outPoint.Draw();
		if (child != null) {
			child.Draw();
		}
		
		rect.width = label.rect.width + outPoint.rect.width;
		rect.height = label.rect.height;
	}
	
	public override void ProcessEvent(Event e) {
		outPoint.ProcessEvent(e);
		
		if (child != null) {
			child.ProcessEvent(e);
		}
	}
}
