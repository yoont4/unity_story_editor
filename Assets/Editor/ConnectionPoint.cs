using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionPointType { In, Out }

/*
  ConnectionPoints represent the in/out handles of a Node, allowing
  Nodes to connect to each other.
*/
public class ConnectionPoint : SDEComponent {
	
	// determines whether this is an in or out ConnectionPoint
	public ConnectionPointType connectionType;
	
	// the Action that defines what happens when clicked
	private Action<ConnectionPoint> OnClickConnectionPoint;
	
	public ConnectionPoint() {}
	
	public void Init(SDEComponent parent, ConnectionPointType connectionType) {
		base.Init(SDEComponentType.ConnectionPoint, parent, 
			new Rect(0, 0, ConnectionManager.CONNECTIONPOINT_WIDTH, ConnectionManager.CONNECTIONPOINT_HEIGHT), 
			ConnectionManager.connectionPointDefault, 
			ConnectionManager.connectionPointDefault, 
			ConnectionManager.connectionPointSelected);
		
		this.connectionType = connectionType;
		
		// determine what method to be called when clicked
		if (this.connectionType == ConnectionPointType.In) {
			this.OnClickConnectionPoint = ConnectionManager.OnClickInPoint;
		} else {
			this.OnClickConnectionPoint = ConnectionManager.OnClickOutPoint;
		}
	}
	
	/*
	  Draw() draws the connection point relative to its parent Node.
	*/ 
	public void Draw() {
		if (parent == null) {
			throw new UnityException("ConnectionPoint was drawn without a parent Node!");
		}
		
		// the parent rect to reference the ConnectionPoint position from
		Rect refRect;
		if (parent.clickRect.width > 0f) {
			refRect = parent.clickRect;
		} else {
			refRect = parent.rect;
		}
		
		// draw the ConnectionPoint midway to the parent
		rect.y = refRect.y + (refRect.height * 0.5f) - (rect.height * 0.5f);
		
		// draw either on the left or right of the parent, depending on 
		// the ConnectionPoint type.
		switch (connectionType) {
			case ConnectionPointType.In:
				rect.x = refRect.x - rect.width + 3f;
				break;
			
			case ConnectionPointType.Out:
				rect.x = refRect.x + refRect.width - 3f;
				break;
		}
		
		GUI.Box(rect, "", style);
	}
	
	/*
	  ProcessEvent() processes the events running through the component.
	*/
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle clicking
			if (e.button == 0) {
				if (rect.Contains(e.mousePosition)) {
					CallOnClickConnectionPoint();
				}
			}
			break;
		}
	}
	
	/*
	  CallOnClickConnectionPoint() activates the OnClickConnectionPoint actions
	  for this ConnectionPoint.
	*/
	public void CallOnClickConnectionPoint() {
		if (OnClickConnectionPoint != null) {
			OnClickConnectionPoint(this);
		} else {
			throw new UnityException("Tried to call OnClickConnectionPoint when null!");
		}
	}
}
