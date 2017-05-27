using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : ScriptableObject {
	
	// holds the list of labels that are used
	private List<string> labels;
	
	// maps a label to an index for fast lookups, additions, and removals
	private Dictionary<string, int> labelMap;
	
	// represents the currently selected label index.
	// -1 means nothing selected
	public int selectedIndex;
	
	private bool expanded;
	
	public Rect rect;
	public Rect toggleRect;
	
	// scroll view vars
	private Rect outerViewRect;
	private Rect innerViewRect;
	private Vector2 scrollPos;
	
	public GUIStyle boxStyle;
	public GUIStyle toggleStyle;
	public GUIStyle toggleUpStyle;
	public GUIStyle toggleDownStyle;
	// TODO: implement these
	public GUIStyle verticalScrollStyle;
	public GUIStyle horizontalScrollStyle;
	
	public DropdownMenu() {}
	public void Init() {
		labels = new List<string>();
		labelMap = new Dictionary<string, int>();
		selectedIndex = -1;
		expanded = false;
		rect = new Rect(0, 0, 100, 20);
		toggleRect = new Rect(0, 0, 16, 16);
		outerViewRect = new Rect(0, 0, 105, 300);
		innerViewRect = new Rect(0, 0, 100, 1000);
		
		// TODO: refactor this shit (use DropdownMenuManager?)
		boxStyle = SDEStyles.nodeInterruptDefault;
		toggleUpStyle = SDEStyles.toggleUpDefault;
		toggleDownStyle = SDEStyles.toggleDownDefault;
		toggleStyle = toggleUpStyle;
		
		// vvvvvv test code vvvvvv 
		expanded = true;
		labels.Add("test");
		labels.Add("asd");
		labels.Add(":)");
		labels.Add("wow");
		labels.Add("test");
		labels.Add("asd");
		labels.Add(":)");
		labels.Add("wow");
		// ^^^^^^ test code ^^^^^^ 
	}
	
	public void Draw() {
		toggleRect.x = rect.x - 16;
		toggleRect.y = rect.y + 2;
		outerViewRect.x = rect.x;
		outerViewRect.y = rect.y + 20;
		
		GUI.Box(rect, "Local Variables", boxStyle);
		if (GUI.Button(toggleRect, "", toggleStyle)) {
			if (toggleStyle == toggleDownStyle) {
				Expand();
			} else {
				Close();
			}
		}
		
		if (expanded) {
			// start scroll view 
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect);
			for (int i = 0; i < labels.Count; i++) {
				// draw each label as a button
				GUI.Box(new Rect(0, i*20, rect.width, 20), labels[i], boxStyle);
			}
			GUI.EndScrollView();
		} 
	}
	
	public void ProcessEvent(Event e) {
		// TODO: implement this
	}
	
	public void Expand() {
		toggleStyle = toggleUpStyle;
		expanded = true;
	}
	
	public void Close() {
		toggleStyle = toggleDownStyle;
		expanded = false;
	}
	
	public void SetPosition(float x, float y) {
		rect.x = x;
		rect.y = y;
	}
	
	private void AddLabel(string label) {
		// TODO: implement this
	}
	
	private void RemoveLabel(string label) {
		// TODO: implement this
	}
	
	private void ContainsLabel(string label) {
		// TODO: implement this
	}
	
	private void SortLabels() {
		// TODO: implement this
	}
	
	// NOTE: probably not the most efficient way of updating it, but should be 
	// fast enough for sizes under 100 elements
	private void UpdateLabelMap() {
		labelMap.Clear();
		for (int i = 0; i < labels.Count; i++) {
			labelMap.Add(labels[i], i);
		}
	}
}