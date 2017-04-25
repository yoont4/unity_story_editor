using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint {
	
	public Rect rect;
	
	// parent Node reference
	public Node node;
	
	public GUIStyle style;
	public GUIStyle defaultControlPointStyle;
	public GUIStyle selectedControlPointStyle;
	public ConnectionPointType type;
	public Action<ConnectionPoint> OnClickConnectionPoint;
	
	public bool isSelected;
	
	public ConnectionPoint(Node node, ConnectionPointType type) {
		this.node = node;
		this.type = type;
		this.style = ConnectionManager.defaultControlPointStyle;
		this.defaultControlPointStyle = ConnectionManager.defaultControlPointStyle;
		this.selectedControlPointStyle = ConnectionManager.selectedControlPointStyle;
		
		if (this.type == ConnectionPointType.In) {
			this.OnClickConnectionPoint = ConnectionManager.OnClickInPoint;
		} else {
			this.OnClickConnectionPoint = ConnectionManager.OnClickOutPoint;
		}
		
		rect = new Rect(
			0, 0, 
			ConnectionManager.CONNECTIONPOINT_WIDTH,
			ConnectionManager.CONNECTIONPOINT_HEIGHT
		);
	}
	
	public void Draw() {
		// draw the connection point midway on the node
		rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
		
		switch (type) {
			case ConnectionPointType.In:
				rect.x = node.rect.x - rect.width + 3f;
				break;
			
			case ConnectionPointType.Out:
				rect.x = node.rect.x + node.rect.width - 3f;
				break;
		}
		
		if (GUI.Button(rect, "", style)) {
			if (OnClickConnectionPoint != null) {
				OnClickConnectionPoint(this);
			}
		}
	}
	
	public void ProcessEvent(Event e) {
		switch(e.type) {
		case EventType.MouseDown:
			if (e.button == 0) {
				// only check for when outside, because the 
				// GUI button uses the event, so selection
				// won't get processed here.
				if (!rect.Contains(e.mousePosition)) {
					StyleDeselect();
					ConnectionManager.ClearConnectionSelection();
				} 
			}
			break;
		}
	}
	
	public void StyleDeselect() {
		isSelected = false;
		style = defaultControlPointStyle;
		GUI.changed = true;
	}
	
	public void StyleSelect() {
		isSelected = true;
		style = selectedControlPointStyle;
		GUI.changed = true;
	}
	
}
