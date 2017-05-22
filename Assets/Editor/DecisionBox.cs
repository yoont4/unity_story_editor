using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionBox : DBox {
	public DecisionBox() {}
	public override void Init(SDEContainer parent, string text) {
		base.Init(parent, text);
		Init();
	}
	
	public override void Init(Node parentNode, string text) {
		base.Init(parentNode, text);
		Init();
	}
	
	private void Init() {
		// assign its container type
		this.containerType = SDEContainerType.DecisionBox;
	}
	
	public override void Draw() {
		base.Draw();
	}
	
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
	}
	
	public override void Remove() {
		// only remove if there are other dialog boxes
		if (parentNode != null && child == null) {
			Debug.Log("Can't remove the last DecisionBox!");
			return;
		}
		
		HistoryManager.RecordEditor();
		
		SDEContainerManager.RemoveContainer(this, markHistory: false);
	}
}
