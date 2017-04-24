using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryEditor : EditorWindow {
	
	private List<Node> nodes;
	private List<Connection> connections;
	
	private GUIStyle nodeStyle;
	private GUIStyle selectedNodeStyle;
	private GUIStyle inPointStyle;
	private GUIStyle outPointStyle;
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 50;
	
	private ConnectionPoint selectedInPoint;
	private ConnectionPoint selectedOutPoint;
	
	private Vector2 offset;
	private Vector2 drag;
	private Rect windowRect;
	
	[MenuItem("Window/Story & Dialogue Editor")]
	private static void OpenWindow() {
		StoryEditor window = GetWindow<StoryEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	private void OnEnable() {
		nodeStyle = new GUIStyle();
		nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		nodeStyle.border = new RectOffset(12, 12, 12, 12);
		
		selectedNodeStyle = new GUIStyle();
		selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
		selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
		
		inPointStyle = new GUIStyle();
		inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
		inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
		inPointStyle.border = new RectOffset(4, 4, 12, 12);
		
		outPointStyle = new GUIStyle();
		outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
		outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
		inPointStyle.border = new RectOffset(4, 4, 12, 12);
	}
	
	private void OnGUI() {
		// draw bg color first
		GUI.color = new Color(0.3f, 0.3f, 0.3f, 1);
		windowRect.Set(0, 0, position.width, position.height);
		GUI.DrawTexture(windowRect, EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
		
		// draw grid over
		DrawGrid(20, 0.2f, Color.gray);
		DrawGrid(100, 0.4f, Color.gray);
		
		// draw nodes on top of background
		DrawNodes();
		
		// draw the connections between nodes
		DrawConnections();
		
		// draw the current connection as it's being selected
		DrawCurrentConnection(Event.current);
		
		ProcessNodeEvents(Event.current);
		ProcessEvents(Event.current);
		
		if (GUI.changed) Repaint();
	}
	
	private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
		int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
		int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
		
		Handles.BeginGUI();
		Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
		
		// parallax effect
		offset += drag * 0.5f;
		Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
		
		for (int i = 0; i < widthDivs; i++) {
			Handles.DrawLine(
				new Vector3(gridSpacing * i + newOffset.x, 0, 0),
				new Vector3(gridSpacing * i + newOffset.x, position.height, 0));
		}
		
		for (int i = 0; i < heightDivs; i++) {
			Handles.DrawLine(
				new Vector3(0, gridSpacing * i + newOffset.y, 0),
				new Vector3(position.width, gridSpacing * i + newOffset.y, 0));
		}
		
		Handles.color = Color.white;
		Handles.EndGUI();
	}
	
	private void DrawNodes() {
		if (nodes != null) {
			for (int i = 0; i < nodes.Count; i++) {
				nodes[i].Draw();
			}
		}
	}
	
	private void DrawConnections() {
		if (connections != null) {
			for (int i = 0; i < connections.Count; i++) {
				connections[i].Draw();
			}
		}
	}
	
	private void DrawCurrentConnection(Event e) {
		if (e.type == EventType.mouseDown && e.button == 1) {
			ClearConnectionSelection();
			e.Use();
			return;
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
	
	private void ProcessEvents(Event e) {
		drag = Vector2.zero;
		
		switch (e.type) {
			case EventType.MouseDown:
				if (e.button == 0 && ClickManager.IsDoubleClick((float)EditorApplication.timeSinceStartup)) {
					OnClickAddNode(e.mousePosition);
				} else if(e.button == 1) {
					ProcessContextMenu(e.mousePosition);
				}
				break;
			
			case EventType.MouseDrag:
				if (e.button == 0) {
					OnDrag(e.delta);
				}
				break;
		}
	}
	
	private void ProcessNodeEvents(Event e) {
		if (nodes != null) {
			// processed backwards because nodes on the top are rendered on top
			for (int i = nodes.Count - 1; i >= 0; i--) {
				bool guiChanged = nodes[i].ProcessEvent(e);
				if (guiChanged) {
					GUI.changed = true;
				}
			}
		}
	}
	
	private void ProcessContextMenu(Vector2 mousePosition) {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Node"), false, ()=>OnClickAddNode(mousePosition));
		genericMenu.ShowAsContext();
	}
	
	private void OnClickAddNode(Vector2 mousePosition) {
		if (nodes == null) {
			nodes = new List<Node>();
		}
		
		Vector2 nodePosition = new Vector2(mousePosition.x - NODE_WIDTH/2, mousePosition.y - NODE_HEIGHT/2);
		
		nodes.Add(new Node(
			nodePosition, NODE_WIDTH, NODE_HEIGHT, 
			nodeStyle, selectedNodeStyle, 
			inPointStyle, outPointStyle,
			OnClickInPoint, OnClickOutPoint,
			OnClickRemoveNode)
		);
	}
	
	private void OnClickRemoveNode(Node node) {
		if (connections != null) {
			List<Connection> connectionsToRemove = new List<Connection>();
			for (int i = 0; i < connections.Count; i++) {
				if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint) {
					connectionsToRemove.Add(connections[i]);
				}
			}
			
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				connections.Remove(connectionsToRemove[i]);
			}
			
			connectionsToRemove = null;
		}
		
		nodes.Remove(node);
	}
	
	private void OnClickInPoint(ConnectionPoint inPoint) {
		selectedInPoint = inPoint;
		OnClickPoint(inPoint);
	}
	
	private void OnClickOutPoint(ConnectionPoint outPoint) {
		selectedOutPoint = outPoint;
		OnClickPoint(outPoint);
	}
	
	// helper function for OnClick[In/Out]Point
	private void OnClickPoint(ConnectionPoint point) {
		if (selectedOutPoint != null && selectedInPoint != null) {
			if (selectedOutPoint.node != selectedInPoint.node) {
				CreateConnection();
			} 
			ClearConnectionSelection();
		}
	}
	
	private void OnClickRemoveConnection(Connection connection) {
		connections.Remove(connection);
	}
	
	private void CreateConnection() {
		if (connections  == null) {
			connections = new List<Connection>();
		}
		
		connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
	}
	
	private void ClearConnectionSelection() {
		selectedInPoint = null;
		selectedOutPoint = null;
	}
	
	private void OnDrag(Vector2 delta) {
		drag = delta;
		
		if (nodes != null) {
			for (int i = 0; i < nodes.Count; i++) {
				nodes[i].Drag(delta);
			}
		}
		
		GUI.changed = true;
	}
}
