using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryDialogueEditor : EditorWindow {
	
	public const int GRID_SIZE = 10;
	
	private Vector2 offset;
	private Vector2 drag;
	private Rect windowRect;
	
	private bool lostFocus;
	private bool drawHelp;
	
	// Help menu constants
	private const float HELP_WIDTH = 200f;
	private const float HELP_HEIGHT = 80f;
	private const string HELP_TEXT = 
	"H: Hide/Show Help Menu\n" +
	"C: Center on all Nodes\n" +
	"D: Delete the selected Node\n";
	
	[MenuItem("Window/Story & Dialogue Editor")]
	public static void OpenWindow() {
		StoryDialogueEditor window = GetWindow<StoryDialogueEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	// TEST CODE: CLEARS THE CONSOLE
	static void ClearConsole () {
         // This simply does "LogEntries.Clear()" the long way:
		var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null,null);
	}
	
	private void OnEnable() {
		ClearConsole();
		
		// initialize component managers
		NodeManager.mainEditor = this;
		ConnectionManager.mainEditor = this;
		
		// load GUI styles
		StyleManager.Initialize();
		NodeManager.nodeDefault = StyleManager.LoadStyle(Style.NodeDefault);
		NodeManager.nodeSelected = StyleManager.LoadStyle(Style.NodeSelected);
		
		ConnectionManager.connectionPointDefault = StyleManager.LoadStyle(Style.ControlPointDefault);
		ConnectionManager.connectionPointSelected = StyleManager.LoadStyle(Style.ControlPointSelected);
		
		TextAreaManager.textAreaStyle = StyleManager.LoadStyle(Style.TextAreaDefault);
		TextAreaManager.textBoxDefault = StyleManager.LoadStyle(Style.TextBoxDefault);
		TextAreaManager.textBoxSelected = StyleManager.LoadStyle(Style.TextBoxSelected);
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
		DrawGrid(10, 0.2f, Color.gray);
		DrawGrid(200, 0.4f, Color.gray);
		
		// process events on nodes, than over the entire editor
		SelectionManager.StartSelectionEventProcessing(Event.current);
		NodeManager.ProcessEvents(Event.current);
		ProcessEvents(Event.current);
		SelectionManager.EndSelectionEventProcessing(Event.current);
		
		// draw nodes on top of background
		NodeManager.DrawNodes();
		// draw the connections between nodes
		ConnectionManager.DrawConnections();
		// draw the current connection as it's being selected
		ConnectionManager.DrawConnectionHandle(Event.current);
		
		// draw the help menu
		if (drawHelp) {
			DrawHelp();
		}
		
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
	
	/*
	  DrawHelp() displays the hotkeys and basic use of the StoryDialogueEditor.
	*/
	private void DrawHelp() {
		Rect helpRect = new Rect(position.xMax, position.yMax, HELP_WIDTH, HELP_HEIGHT);
		helpRect.x = position.width - HELP_WIDTH - 5f;
		helpRect.y = position.height - HELP_HEIGHT - 5f;
		GUI.TextArea(helpRect, HELP_TEXT, TextAreaManager.textAreaStyle);
	}

	private void ProcessEvents(Event e) {
		drag = Vector2.zero;
		
		switch (e.type) {
		// check for selection or context menu
		case EventType.MouseDown:
			if (e.button == 0 && ClickManager.IsDoubleClick((float)EditorApplication.timeSinceStartup)) {
				Vector2 creationOffset = new Vector2(
					e.mousePosition.x % GRID_SIZE - offset.x % GRID_SIZE,
					e.mousePosition.y % GRID_SIZE - offset.y % GRID_SIZE);
				NodeManager.AddNodeAt(e.mousePosition - creationOffset);
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
			if (NodeManager.nodes != null && NodeManager.nodes.Count > 0) { 
				Debug.Log("centering on nodes...");
				// calculate current average
				Vector2 avgPosition = new Vector2();
				for (int i = 0; i < NodeManager.nodes.Count; i++) {
					avgPosition += NodeManager.nodes[i].rect.center;
				}
				avgPosition /= NodeManager.nodes.Count;
				
				// reshift everything by this new average, including window size
				OnDrag(-avgPosition + (position.size/2));
			} else {
				Debug.Log("no nodes to center on");
			}
		}
		
			// 'D' delete everything in the editor window
		if (key == KeyCode.D) {
			if (NodeManager.nodes != null && SelectionManager.SelectedComponent() != null) {
				Debug.Log("deleting selected node...");
				SDEComponent component = SelectionManager.SelectedComponent();
				while (component != null) {
					if (component.componentType == SDEComponentType.Node) {
						// if a match is found, remove the Node and return
						NodeManager.RemoveNode((Node)component);
						return;
					}
					component = component.parent;
				}
				
				// if no match was found, that means the component had no Node parent!
				throw new UnityException("tried to delete SDEComponent with no parent Node!");
			} else {
				Debug.Log("Ignoring 'D'elete, no Node selected!");
			}
		}
		
		// 'H' show/hide the help box
		if (key == KeyCode.H) {
			if (drawHelp) {
				Debug.Log("Hiding Help menu");
				drawHelp = false;
			} else {
				Debug.Log("Displaying Help menu");
				drawHelp = true;
			}
		}
	}
	
	private void ProcessContextMenu(Vector2 mousePosition) {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Node"), false, ()=>NodeManager.AddNodeAt(mousePosition));
		genericMenu.ShowAsContext();
	}
	
	private void OnDrag(Vector2 delta) {
		if (FeatureManager.dragEnabled) {
			drag = delta;
			
			if (NodeManager.nodes != null) {
				for (int i = 0; i < NodeManager.nodes.Count; i++) {
					NodeManager.nodes[i].Drag(delta);
				}
			}
			
			GUI.changed = true;
		}
	}
}
