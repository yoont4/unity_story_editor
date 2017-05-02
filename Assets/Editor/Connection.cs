using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  Connections represent the connection between 2 control points.
*/
public class Connection : ScriptableObject {
	
	// bezier curve draw vars
	public const float TANGENT_DIST = 50f;
	public const float WIDTH = 2f;
	
	// the connection points the connection is tied to
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	
	// the Action that defines what happens on removal
	private Action<Connection> OnRemoveConnection;
	
	public Connection() {}
	
	public void Init(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnRemoveConnection) {
		this.inPoint = inPoint;
		this.outPoint = outPoint;
		this.OnRemoveConnection = OnRemoveConnection;
	}
	
	/*
	  Draw() draws the bezier curve between the connection's 2 connection points.
	*/
	public void Draw() {
		Handles.DrawBezier(
			inPoint.rect.center,
			outPoint.rect.center,
			inPoint.rect.center + Vector2.left * TANGENT_DIST,
			outPoint.rect.center - Vector2.left * TANGENT_DIST,
			Color.white,
			null,
			WIDTH );
		
		if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
			CallOnRemoveConnection();
		}
	}
	
	/*
	  CallOnRemoveConnection() activates the OnRemoveConnection actions for this Connection.
	*/
	private void CallOnRemoveConnection() {
		if (OnRemoveConnection != null) {
			OnRemoveConnection(this);
		}
	}
}
