using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryDialogueEditor : EditorWindow {
	
	private Vector2 offset;
	private Vector2 drag;
	private Rect windowRect;
	
	[MenuItem("Window/Story & Dialogue Editor")]
	public static void OpenWindow() {
		StoryDialogueEditor window = GetWindow<StoryDialogueEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	private void OnEnable() {
		// initialize component managers
		NodeManager.mainEditor = this;
		ConnectionManager.mainEditor = this;
		
		
		// instantiate GUI styles
		NodeManager.defaultNodeStyle = StyleManager.LoadStyle(Style.NodeDefault);
		NodeManager.selectedNodeStyle = StyleManager.LoadStyle(Style.NodeSelected);
		
		ConnectionManager.defaultControlPointStyle = StyleManager.LoadStyle(Style.ControlPointDefault);
		ConnectionManager.selectedControlPointStyle = StyleManager.LoadStyle(Style.ControlPointSelected);
	}
	
	private void OnGUI() {
		// draw bg color first
		GUI.color = new Color(0.3f, 0.3f, 0.3f, 1);
		windowRect.Set(0, 0, position.width, position.height);
		GUI.DrawTexture(windowRect, EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
		
		// draw grid over
		DrawGrid(50, 0.2f, Color.gray);
		DrawGrid(200, 0.4f, Color.gray);
		
		// draw nodes on top of background
		NodeManager.DrawNodes();
		
		// draw the connections between nodes
		ConnectionManager.DrawConnections();
		
		// draw the current connection as it's being selected
		ConnectionManager.DrawConnectionHandle(Event.current);
		
		// process events on nodes, than over the entire editor
		NodeManager.ProcessEvents(Event.current);
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

	private void ProcessEvents(Event e) {
		drag = Vector2.zero;
		
		switch (e.type) {
		case EventType.MouseDown:
			if (e.button == 0 && ClickManager.IsDoubleClick((float)EditorApplication.timeSinceStartup)) {
				NodeManager.OnClickAddNode(e.mousePosition);
			} else if(e.button == 1) {
				ProcessContextMenu(e.mousePosition);
			}
			break;
		
		case EventType.MouseDrag:
			if (e.button == 0) {
				OnDrag(e.delta);
			}
			break;
		
		// listen for key commands
		case EventType.KeyDown:
			// 'C' center on node positions
			if (e.keyCode == KeyCode.C) {
				// calculate current average
				Vector2 avgPosition = new Vector2();
				for (int i = 0; i < NodeManager.nodes.Count; i++) {
					avgPosition += NodeManager.nodes[i].rect.center;
				}
				avgPosition /= NodeManager.nodes.Count;
				
				// reshift everything by this new average, including window size
				OnDrag(-avgPosition + (position.size/2));
			}
			break;
		}
	}
	
	private void ProcessContextMenu(Vector2 mousePosition) {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Node"), false, ()=>NodeManager.OnClickAddNode(mousePosition));
		genericMenu.ShowAsContext();
	}
	
	private void OnDrag(Vector2 delta) {
		drag = delta;
		
		if (NodeManager.nodes != null) {
			for (int i = 0; i < NodeManager.nodes.Count; i++) {
				NodeManager.nodes[i].Drag(delta);
			}
		}
		
		GUI.changed = true;
	}
}
