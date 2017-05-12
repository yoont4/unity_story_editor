using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDELabel : SDEComponent {
	
	public string text;
	
	public SDELabel() {}
	
	public void Init(SDEComponent parent, string text) {
		base.Init(SDEComponentType.Label, parent,
			new Rect(0, 0, NodeManager.INTERRUPT_WIDTH, NodeManager.INTERRUPT_HEIGHT), 
			SDELabelManager.labelStyle, null, null);
		
		Init(text);
	}
	
	public void Init(SDEContainer container, string text) {
		base.Init(SDEComponentType.Label, null,
			new Rect(0, 0, NodeManager.INTERRUPT_WIDTH, NodeManager.INTERRUPT_HEIGHT), 
			SDELabelManager.labelStyle, null, null, 
			container);
		
		Init(text);
	}
	
	private void Init(string text) {
		this.text = text;
	}
	
	public override void Draw() {
		
		if (container != null) {
			rect.x = container.rect.x;	
			rect.y = container.rect.y;
		}
		
		GUI.Box(rect, text, style);
	}
}
