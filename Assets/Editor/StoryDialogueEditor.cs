using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryEditor : EditorWindow {
	
	private List<Node> nodes;
	
	[MenuItem("Window/Story & Dialogue Editor")]
	private static void OpenWindow() {
		StoryEditor window = GetWindow<StoryEditor>();
		window.titleContent = new GUIContent("Story & Dialogue Editor");
	}
	
	private void OnGUI() {
		DrawNodes();
		
		ProcessEvents(Event.current);
		
		if (GUI.changed) Repaint();
	}
	
	private void DrawNodes() {
		if (nodes != null) {
			for (int i = 0; i < nodes.Count; i++) {
				nodes[i].Draw();
			}
		}
	}
	
	private void ProcessEvents(Event e) {
		
	}
}
