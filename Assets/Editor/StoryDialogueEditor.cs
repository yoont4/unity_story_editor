using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryDialogueEditor : EditorWindow {
	
	private Vector2 offset;
	private Vector2 drag;
	private Rect windowRect;
	
	private bool lostFocus;
	
	[MenuItem("Window/Story & Dialogue Editor")]
	public static void OpenWindow() {
		StoryDialogueEditor window = GetWindow<StoryDialogueEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	private void OnEnable() {
		// initialize component managers
		NodeManager.mainEditor = this;
		ConnectionManager.mainEditor = this;
		
		// TODO: rename all style variables to be format: <component><state>
		// i.e. defaultNodeStyle -> nodeDefault
		
		// load GUI styles
		NodeManager.defaultNodeStyle = StyleManager.LoadStyle(Style.NodeDefault);
		NodeManager.selectedNodeStyle = StyleManager.LoadStyle(Style.NodeSelected);
		
		ConnectionManager.defaultControlPointStyle = StyleManager.LoadStyle(Style.ControlPointDefault);
		ConnectionManager.selectedControlPointStyle = StyleManager.LoadStyle(Style.ControlPointSelected);
		
		TextAreaManager.textAreaStyle = StyleManager.LoadStyle(Style.TextAreaDefault);
		TextAreaManager.defaultTextBoxStyle = StyleManager.LoadStyle(Style.TextBoxDefault);
		TextAreaManager.selectedTextBoxStyle = StyleManager.LoadStyle(Style.TextBoxSelected);
	}
	
	private void OnGUI() {
		// draw bg color first
		GUI.color = new Color(0.3f, 0.3f, 0.3f, 1);
		windowRect.Set(0, 0, position.width, position.height);
		GUI.DrawTexture(windowRect, EditorGUIUtility.whiteTexture);
		
		// reset base color
		GUI.color = Color.white;
		
		// choose cursor color
		if (lostFocus) {
			GUI.skin.settings.cursorColor = Color.black;
		} else {
			GUI.skin.settings.cursorColor = Color.white;
		}
		
		// draw grid over
		DrawGrid(50, 0.2f, Color.gray);
		DrawGrid(200, 0.4f, Color.gray);
		
		// process events on nodes, than over the entire editor
		SelectionManager.StartSelectionEventProcessing(Event.current);
		NodeManager.ProcessEvents(Event.current);
		ProcessEvents(Event.current);
		SelectionManager.EndSelectionEventProcessing(Event.current);
		
		// draw the current connection as it's being selected
		ConnectionManager.DrawConnectionHandle(Event.current);
		// draw nodes on top of background
		NodeManager.DrawNodes();
		// draw the connections between nodes
		ConnectionManager.DrawConnections();
		
		if (GUI.changed) Repaint();
	}
	
	// change the cursor color to white within the editor
	private void OnFocus() {
		lostFocus = false;
	}
	
	// reset the cursor color when outside the editor
	private void OnLostFocus() {
		lostFocus = true;
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
		// check for selection or context menu
		case EventType.MouseDown:
			if (e.button == 0 && ClickManager.IsDoubleClick((float)EditorApplication.timeSinceStartup)) {
				NodeManager.OnClickAddNode(e.mousePosition);
			} 
			
			if(e.button == 1 && SelectionManager.SelectedComponentType() == SDEComponentType.Nothing) {
				ProcessContextMenu(e.mousePosition);
			}
			break;
			
		// check for window dragging
		case EventType.MouseDrag:
			if (e.button == 0) {
				OnDrag(e.delta);
			}
			break;
		
		// listen for key commands
		case EventType.KeyDown:
			if (SelectionManager.SelectedComponentType() != SDEComponentType.TextArea) {
				ProcessKeyboardInput(e.keyCode);
			}
			break;
		}
	}
	
	private void ProcessKeyboardInput(KeyCode key) {
		// 'C' center on node positions
		if (key == KeyCode.C) {
			Debug.Log("centering on nodes...");
				// calculate current average
			Vector2 avgPosition = new Vector2();
			for (int i = 0; i < NodeManager.nodes.Count; i++) {
				avgPosition += NodeManager.nodes[i].rect.center;
			}
			avgPosition /= NodeManager.nodes.Count;
			
				// reshift everything by this new average, including window size
			OnDrag(-avgPosition + (position.size/2));
		}
		
			// 'D' delete everything in the editor window
		if (key == KeyCode.D) {
			Debug.Log("deleting nodes...");
			if (ConnectionManager.connections != null) {
				ConnectionManager.connections.Clear();
				ConnectionManager.connections = null;
			}
			
			if (NodeManager.nodes != null) {
				NodeManager.nodes.Clear();
				NodeManager.nodes = null;
			}
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
