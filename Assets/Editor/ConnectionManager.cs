using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  ConnectionManager serves as an intermediate between individual Connections
  and the StoryDialogueEditor.
  
  This allows the organization of Action<Connection> to be removed from the
  StoryDialogueEditor itself, without cloning a copy of the main editor into 
  every single Connection object.
*/
public static class ConnectionManager {
	
	public static StoryDialogueEditor mainEditor;
	
	// the global list of connections
	public static List<Connection> connections;
	
	// all ConnectionPoints use these styles
	public static GUIStyle defaultConnectionPointStyle;
	public static GUIStyle selectedConnectionPointStyle;
	
	// keeps track of the selected in/out points
	public static ConnectionPoint selectedInPoint;
	public static ConnectionPoint selectedOutPoint;
	
	// defines the ConnectionPoint dimensions
	public const float CONNECTIONPOINT_WIDTH = 16F;
	public const float CONNECTIONPOINT_HEIGHT = 16F;
	
	
	/*
	  ClickIn/OutPoint() is used to manage the selection status of
	  ConnectionPoints and whether a Connection should be spawned or not.
	*/
	public static void ClickInPoint(ConnectionPoint inPoint) {
		selectedInPoint = inPoint;
		OnClickPoint(inPoint);
	}
	
	public static void ClickOutPoint(ConnectionPoint outPoint) {
		selectedOutPoint = outPoint;
		OnClickPoint(outPoint);
	}
	
	// helper function for OnClick[In/Out]Point
	private static void OnClickPoint(ConnectionPoint point) {
		point.Selected = true;
		
		if (selectedOutPoint != null && selectedInPoint != null) {
			if (selectedOutPoint.parent != selectedInPoint.parent) {
				CreateConnection();
			} 
			ClearConnectionSelection();
		}
	}
	
	/*
	  ClearConnectionSelection() clears the currently selected ConnectionPoints
	  and the ConnectionPoints' selection status vars.
	*/
	public static void ClearConnectionSelection() {
		if (selectedInPoint != null) {
			SelectionManager.Deselect(selectedInPoint);
			selectedInPoint.Selected = false;
			selectedInPoint = null;
		}
		
		if (selectedOutPoint != null) {
			SelectionManager.Deselect(selectedOutPoint);
			selectedOutPoint.Selected = false;
			selectedOutPoint = null;
		}
	}
	
	/*
	  DrawConnectionHandle() draws a bezier curve from the currently selected 
	  ConnectionPoint to the cursor.
	*/
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
	
	/*
	  DrawConnections() draws all the currently defined Connections
	*/
	public static void DrawConnections() {
		if (connections != null) {
			for (int i = 0; i < connections.Count; i++) {
				connections[i].Draw();
			}
		}
	}
	
	/*
	  CreateConnection() forms a new Connection between the currently 
	  selected in/out ConnectionPoints.
	*/
	public static void CreateConnection() {
		if (connections  == null) {
			connections = new List<Connection>();
		}
		
		connections.Add(new Connection(selectedInPoint, selectedOutPoint, RemoveConnection));
	}
	
	/*
	  RemoveConnection() removes the given Connection from the global
	  connections list.
	*/
	public static void RemoveConnection(Connection connection) {
		connections.Remove(connection);
	}
}
