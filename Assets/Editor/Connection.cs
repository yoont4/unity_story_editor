using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Connection {
	
	public const float TANGENT_DIST = 50f;
	public const float WIDTH = 2f;
	
	public ConnectionPoint inPoint;
	public ConnectionPoint outPoint;
	public Action<Connection> OnClickRemoveConnection;
	
	public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection) {
		this.inPoint = inPoint;
		this.outPoint = outPoint;
		this.OnClickRemoveConnection = OnClickRemoveConnection;
	}
	
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
			if (OnClickRemoveConnection != null) {
				OnClickRemoveConnection(this);
			}
		}
	}
}
