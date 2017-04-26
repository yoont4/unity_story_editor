using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint : SDEComponent {
	
	public ConnectionPointType connectionType;
	public Action<ConnectionPoint> OnClickConnectionPoint;
	
	public ConnectionPoint(Node node, ConnectionPointType connectionType) :
	base (
	SDEComponentType.ConnectionPoint, node, 
	new Rect(0, 0, ConnectionManager.CONNECTIONPOINT_WIDTH, ConnectionManager.CONNECTIONPOINT_HEIGHT), 
	ConnectionManager.defaultControlPointStyle, 
	ConnectionManager.defaultControlPointStyle, 
	ConnectionManager.selectedControlPointStyle) 
	{
		this.connectionType = connectionType;
		
		if (this.connectionType == ConnectionPointType.In) {
			this.OnClickConnectionPoint = ConnectionManager.OnClickInPoint;
		} else {
			this.OnClickConnectionPoint = ConnectionManager.OnClickOutPoint;
		}
	}
	
	public void Draw() {
		// draw the connection point midway on the parent
		rect.y = parent.rect.y + (parent.rect.height * 0.5f) - rect.height * 0.5f;
		
		switch (connectionType) {
			case ConnectionPointType.In:
				rect.x = parent.rect.x - rect.width + 3f;
				break;
			
			case ConnectionPointType.Out:
				rect.x = parent.rect.x + parent.rect.width - 3f;
				break;
		}
		
		GUI.Box(rect, "", style);
	}
	
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			if (e.button == 0) {
				if (rect.Contains(e.mousePosition)) {
					if (OnClickConnectionPoint != null) {
						OnClickConnectionPoint(this);
					} else {
						throw new UnityException("Tried to call OnClickConnectionPoint when null!");
					}
				}
			}
			break;
		}
	}
}
