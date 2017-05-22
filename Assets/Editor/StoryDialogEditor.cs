using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryDialogEditor : EditorWindow {
	
	// The global list of nodes and connections. 
	// NOTE: these must be in a non-static class, as Undo
	// operations cannot record changes within static classes, because
	// they must extend the UnityEngine.Object type.
	public List<Node> nodes;
	public List<Connection> connections;
	
	// the command log history text
	public string commandHistory;
	
	public const int GRID_SIZE = 10;
	
	private Vector2 offset;
	private Vector2 drag;
	private Rect windowRect;
	
	private bool lostFocus;
	
	private bool drawHelp = true;
	private bool drawDebug = true;
	private double testTime = 0;
	private double t = 0;
	
	// Help menu constants
	private const float HELP_WIDTH = 180f;
	private const float HELP_HEIGHT = 80f;
	private const string HELP_TEXT = 
	"H: Hide/Show Help Menu\n" +
	"Q: Hide/Show Debug Info\n" +
	"C: Center on all Nodes\n" +
	"D: Delete the selected Node\n\n" +
	"R-Mouse: Context Menu";
	
	// Debug menu constants
	private const float DEBUG_WIDTH = 300f;
	private const float DEBUG_HEIGHT = 500f;
	private string debugText;
	
	[MenuItem("Window/Story & Dialog Editor")]
	public static void OpenWindow() {
		StoryDialogEditor window = GetWindow<StoryDialogEditor>();
		window.titleContent = new GUIContent("Story & Dialog Editor");
	}
	
	// TEST CODE: CLEARS THE CONSOLE
	private void ClearConsole () {
         // This simply does "LogEntries.Clear()" the long way:
		var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null,null);
	}
	
	/*
	  This wipes all global data from the scene.
	
	  Used only on script recompile.
	*/
	private void DestroyScene() {
		ClearConsole();
		nodes = null;
		connections = null;
	}
	
	private void OnEnable() {
		DestroyScene();
		
		// initialize component managers
		NodeManager.mainEditor = this;
		ConnectionManager.mainEditor = this;
		TextAreaManager.mainEditor = this;
		DialogBoxManager.mainEditor = this;
		SDEContainerManager.mainEditor = this;
		HistoryManager.mainEditor = this;
		
		// load GUI styles
		StyleManager.Initialize();
		NodeManager.nodeDefault = StyleManager.LoadStyle(Style.NodeDefault);
		NodeManager.nodeSelected = StyleManager.LoadStyle(Style.NodeSelected);
		NodeManager.nodeInterruptDefault = StyleManager.LoadStyle(Style.NodeInterruptDefault);
		NodeManager.nodeInterruptSelected = StyleManager.LoadStyle(Style.NodeInterruptSelected);
		
		ConnectionManager.connectionPointDefault = StyleManager.LoadStyle(Style.ConnectionPointDefault);
		ConnectionManager.connectionPointSelected = StyleManager.LoadStyle(Style.ConnectionPointSelected);
		
		TextAreaManager.textAreaStyle = StyleManager.LoadStyle(Style.TextAreaDefault);
		TextAreaManager.textAreaButtonStyle = StyleManager.LoadStyle(Style.TextAreaButtonDefault);
		TextAreaManager.textBoxDefault = StyleManager.LoadStyle(Style.TextBoxDefault);
		TextAreaManager.textBoxSelected = StyleManager.LoadStyle(Style.TextBoxSelected);
		
		SDELabelManager.labelStyle = StyleManager.LoadStyle(Style.LabelDefault);
	}
	
	

	
	private void OnGUI() {
		t = EditorApplication.timeSinceStartup;
		
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
		
		// Add to the Undo stack if anything changed
		HistoryManager.FlushIfDirty();
		
		// draw the connections between nodes
		ConnectionManager.DrawConnections();
		// draw nodes on top of background
		NodeManager.DrawNodes();
		// draw the current connection as it's being selected
		ConnectionManager.DrawConnectionHandle(Event.current);
		
		// draw additional information
		if (drawHelp) DrawHelp();
		if (drawDebug) DrawDebug();
		
		if (GUI.changed) Repaint();
		
		testTime = testTime * .9d + ((EditorApplication.timeSinceStartup - t)/10);
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
		
		for (int i = 0; i <= widthDivs; i++) {
			Handles.DrawLine(
				new Vector3(gridSpacing * i + newOffset.x, 0, 0),
				new Vector3(gridSpacing * i + newOffset.x, position.height, 0));
		}
		
		for (int i = 0; i <= heightDivs; i++) {
			Handles.DrawLine(
				new Vector3(0, gridSpacing * i + newOffset.y, 0),
				new Vector3(position.width, gridSpacing * i + newOffset.y, 0));
		}
		
		Handles.color = Color.white;
		Handles.EndGUI();
	}
	
	/*
	  DrawHelp() displays the hotkeys and basic use of the StoryDialogEditor.
	*/
	private void DrawHelp() {
		Rect helpRect = new Rect(0, 0, HELP_WIDTH, HELP_HEIGHT);
		helpRect.x = position.width - HELP_WIDTH - 5f;
		helpRect.y = position.height - HELP_HEIGHT - 5f;
		GUI.Box(helpRect, HELP_TEXT, TextAreaManager.textAreaStyle);
	}
	
	/*
	  DrawDebug() displays some information about the story editor for debug use
	*/
	private void DrawDebug() {
		Rect debugRect = new Rect(5f, 5f, DEBUG_WIDTH, DEBUG_HEIGHT);
		debugText = "";
		debugText += "Selected Component Type: " + SelectionManager.SelectedComponentType();
		debugText += "\nCurrent Keyboard Control ID: " + GUIUtility.keyboardControl;
		debugText += "\nNumber of Nodes: " + (nodes != null ? nodes.Count.ToString() : "null");
		debugText += "\nNumber of Connections: " + (connections != null ? connections.Count.ToString() : "null");
		debugText += "\nOnGUI Run Time: " + (testTime*1000).ToString("F3") + "ms";
		GUI.Box(debugRect, debugText, TextAreaManager.textAreaStyle);
	}

	private void ProcessEvents(Event e) {
		drag = Vector2.zero;
		
		switch (e.type) {
		// check for selection or context menu
		case EventType.MouseDown:
			if (e.button == 0 && ClickManager.IsDoubleClick((float)EditorApplication.timeSinceStartup, e.mousePosition, SDEComponentType.Nothing)) {
				Vector2 creationOffset = new Vector2(
					e.mousePosition.x % GRID_SIZE - offset.x % GRID_SIZE,
					e.mousePosition.y % GRID_SIZE - offset.y % GRID_SIZE);
				NodeManager.AddNodeAt(e.mousePosition - creationOffset, NodeType.Nothing);
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
			// NOTE: Unity editor is broken, so both a tab KeyCode AND a '\t' character
			// gets parsed on key press. The '\t' character is what the editor uses
			// internally for it's default tab cycling (which we want to override).
			if (e.character == '\t') {e.Use();} //eat the input			
			if (SelectionManager.SelectedComponentType() != SDEComponentType.TextArea) {
				ProcessKeyboardInput(e.keyCode);
			}
			break;
		}
	}
	
	private void ProcessKeyboardInput(KeyCode key) {
		// 'C' center on node positions
		if (key == KeyCode.C) {
			if (nodes != null && nodes.Count > 0) { 
				Debug.Log("centering on nodes...");
				// calculate current average
				Vector2 avgPosition = new Vector2();
				for (int i = 0; i < nodes.Count; i++) {
					avgPosition += nodes[i].rect.center;
				}
				avgPosition /= nodes.Count;
				
				// reshift everything by this new average, including window size
				OnDrag(-avgPosition + (position.size/2));
			} else {
				Debug.Log("no nodes to center on");
			}
		}
		
			// 'D' delete the selected node
		if (key == KeyCode.D) {
			if (nodes != null && SelectionManager.SelectedComponent() != null) {
				Debug.Log("deleting selected node...");
				SDEComponent component = SelectionManager.SelectedComponent();
				while (component != null) {
					if (component.componentType == SDEComponentType.Node) {
						// if a match is found, remove the Node if it's not an Interrupt and return
						if (((Node)component).nodeType != NodeType.Interrupt) {
							NodeManager.RemoveNode((Node)component);
						}
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
		
		// 'Q' show/hide debug information
		if (key == KeyCode.Q) {
			if (drawDebug) {
				Debug.Log("Hiding Debug info");
				drawDebug = false;
			} else {
				Debug.Log("Displaying Debug info");
				drawDebug = true;
			}
		}
	}
	
	private void ProcessContextMenu(Vector2 mousePosition) {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Node"), false, ()=>NodeManager.AddNodeAt(mousePosition, NodeType.Nothing));
		genericMenu.ShowAsContext();
	}
	
	private void OnDrag(Vector2 delta) {
		if (FeatureManager.dragEnabled) {
			drag = delta;
			
			if (nodes != null) {
				for (int i = 0; i < nodes.Count; i++) {
					nodes[i].Drag(delta);
				}
			}
			
			GUI.changed = true;
		}
	}
}
