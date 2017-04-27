using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ConnectionManager {
	
	public static StoryDialogueEditor mainEditor;
	public static List<Connection> connections;
	
	public static GUIStyle defaultControlPointStyle;
	public static GUIStyle selectedControlPointStyle;
	
	public static ConnectionPoint selectedInPoint;
	public static ConnectionPoint selectedOutPoint;
	
	public const float CONNECTIONPOINT_WIDTH = 16F;
	public const float CONNECTIONPOINT_HEIGHT = 16F;
	
	
	public static void OnClickInPoint(ConnectionPoint inPoint) {
		selectedInPoint = inPoint;
		OnClickPoint(inPoint);
	}
	
	public static void OnClickOutPoint(ConnectionPoint outPoint) {
		selectedOutPoint = outPoint;
		OnClickPoint(outPoint);
	}
	
	// helper function for OnClick[In/Out]Point
	public static void OnClickPoint(ConnectionPoint point) {
		point.Selected = true;
		
		if (selectedOutPoint != null && selectedInPoint != null) {
			if (selectedOutPoint.parent != selectedInPoint.parent) {
				CreateConnection();
			} 
			ClearConnectionSelection();
		}
	}
	
	public static void ClearConnectionSelection() {
		if (selectedInPoint != null) {
			selectedInPoint.Selected = false;
			selectedInPoint = null;
		}
		
		if (selectedOutPoint != null) {
			selectedOutPoint.Selected = false;
			selectedOutPoint = null;
		}
	}
	
	public static void DrawConnectionHandle(Event e) {
		if (SelectionManager.SelectedComponentType() != SDEComponentType.ConnectionPoint) {
			ClearConnectionSelection();
		}
		
		if (selectedInPoint != null && selectedOutPoint == null) {
			Handles.DrawBezier(
				selectedInPoint.rect.center,
				e.mousePosition,
				selectedInPoint.rect.center + Vector2.left * 50f,
				e.mousePosition - Vector2.left * 50f,
				Color.white,
				null,
				2f);
		}
		
		if (selectedInPoint == null && selectedOutPoint != null) {
			Handles.DrawBezier(
				selectedOutPoint.rect.center,
				e.mousePosition,
				selectedOutPoint.rect.center - Vector2.left * 50f,
				e.mousePosition + Vector2.left * 50f,
				Color.white,
				null,
				2f);
		}
		
		GUI.changed = true;
	}
	
	public static void DrawConnections() {
		if (connections != null) {
			for (int i = 0; i < connections.Count; i++) {
				connections[i].Draw();
			}
		}
	}
	
	public static void CreateConnection() {
		if (connections  == null) {
			connections = new List<Connection>();
		}
		
		connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
	}
	
	public static void OnClickRemoveConnection(Connection connection) {
		connections.Remove(connection);
	}
}
