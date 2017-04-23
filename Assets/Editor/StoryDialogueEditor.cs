using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryEditor : EditorWindow {
	
	private List<Node> nodes;
	private GUIStyle nodeStyle;
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 50;
	
	private Vector2 offset;
	private Vector2 drag;
	
	[MenuItem("Window/Story & Dialogue Editor")]
	private static void OpenWindow() {
		StoryEditor window = GetWindow<StoryEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	private void OnEnable() {
		nodeStyle = new GUIStyle();
		nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		nodeStyle.border = new RectOffset(12, 12, 12, 12);
	}
	
	private void OnGUI() {
		// draw background first
		DrawGrid(20, 0.2f, Color.gray);
		DrawGrid(100, 0.4f, Color.gray);
		
		// draw nodes on top of background
		DrawNodes();
		
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
				new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
				new Vector3(gridSpacing * i, position.height, 0) + newOffset);
		}
		
		for (int i = 0; i < heightDivs; i++) {
			Handles.DrawLine(
				new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset,
				new Vector3(position.width, gridSpacing * i, 0) + newOffset);
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
		
		nodes.Add(new Node(nodePosition, NODE_WIDTH, NODE_HEIGHT, nodeStyle));
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
