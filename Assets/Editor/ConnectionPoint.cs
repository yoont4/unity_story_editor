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
	
	// if anchored, this ConnectionPoint doesn't implicitly update based on a parent
	public bool anchored = false;
	
	// determines whether this is an in or out ConnectionPoint
	public ConnectionPointType connectionType;
	
	// the Action that defines what happens when clicked
	private Action<ConnectionPoint> OnClickConnectionPoint;
	
	public List<Connection> connections;
	
	public ConnectionPoint() {}
	
	public void Init(SDEComponent parent, ConnectionPointType connectionType) {
		base.Init(SDEComponentType.ConnectionPoint, parent, 
			new Rect(0, 0, ConnectionManager.CONNECTIONPOINT_WIDTH, ConnectionManager.CONNECTIONPOINT_HEIGHT), 
			SDEStyles.connectionPointDefault, 
			SDEStyles.connectionPointDefault, 
			SDEStyles.connectionPointSelected);
		
		this.connectionType = connectionType;
		
		// determine what method to be called when clicked
		if (this.connectionType == ConnectionPointType.In) {
			this.OnClickConnectionPoint = ConnectionManager.OnClickInPoint;
		} else {
			this.OnClickConnectionPoint = ConnectionManager.OnClickOutPoint;
		}
		
		this.connections = new List<Connection>();
	}
	
	/*
	  Draw() draws the connection point relative to its parent Node.
	*/ 
	public override void Draw() {
		if (parent == null && !anchored) {
			throw new UnityException("Unanchored ConnectionPoint was drawn without a parent Node!");
		}
		
		// only inherit parent position if unanchored
		if (!anchored) {
			// the parent rect to reference the ConnectionPoint position from
			Rect refRect;
			if (parent.widthPad != 0f || parent.heightPad != 0f) {
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
			
		}
		
		GUI.Box(rect, "", style);
	}
	
	public void SetPosition(float x, float y) {
		rect.x = x;
		rect.y = y;
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
				if (Contains(e.mousePosition)) {
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
