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
	
	// This is used to avoid instantiating a float every Draw() call
	private float tangentOffset;
	
	private bool clickable;
	
	public Connection() {}
	
	public void Init(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnRemoveConnection, bool clickable) {
		this.inPoint = inPoint;
		this.outPoint = outPoint;
		this.OnRemoveConnection = OnRemoveConnection;
		
		this.clickable = clickable;
	}
	
	/*
	  Draw() draws the bezier curve between the connection's 2 connection points.
	*/
	public void Draw() {
		tangentOffset = Mathf.Min(Vector2.Distance(inPoint.rect.center, outPoint.rect.center)/2f, TANGENT_DIST);
		
		Handles.DrawBezier(
			inPoint.rect.center,
			outPoint.rect.center,
			inPoint.rect.center + Vector2.left * tangentOffset,
			outPoint.rect.center - Vector2.left * tangentOffset,
			Color.white,
			null,
			WIDTH );
		
		if(clickable) {
			if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
				CallOnRemoveConnection();
			}
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
