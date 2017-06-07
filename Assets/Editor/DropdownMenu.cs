using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : ScriptableObject {
	
	// holds the list of labels that are used
	private List<TextArea> labels;
	
	// used to keep track of labels and help with updating, checking for changes, etc.
	private Dictionary<String, TextArea> labelMap;
	
	// represents the currently selected label index.
	// -1 means nothing selected
	public int selectedIndex;
	
	private bool expanded;
	
	public Rect rect;
	public Rect toggleRect;
	
	// can be hooked up to update story nodes on delete, etc.
	public Action<string> OnDelete;
	
	// scroll view vars
	private Rect outerViewRect;
	private Rect innerViewRect;
	private Vector2 scrollPos;
	
	public GUIStyle boxStyle;
	public GUIStyle toggleStyle;
	public GUIStyle toggleUpStyle;
	public GUIStyle toggleDownStyle;
	
	public const int LABEL_HEIGHT = 20;
	public const int LABEL_OFFSET = 22;
	
	public DropdownMenu() {}
	public void Init() {
		labels = new List<TextArea>();
		labelMap = new Dictionary<String, TextArea>();
		
		selectedIndex = -1;
		expanded = false;
		rect = new Rect(0, 0, 140, LABEL_HEIGHT);
		toggleRect = new Rect(0, 0, 16, 16);
		outerViewRect = new Rect(0, 0, rect.width+40, 300);
		innerViewRect = new Rect(0, 0, 100, 1000);
		
		// TODO: refactor this shit (use DropdownMenuManager?)
		boxStyle = SDEStyles.nodeInterruptDefault;
		toggleUpStyle = SDEStyles.toggleUpDefault;
		toggleDownStyle = SDEStyles.toggleDownDefault;
		toggleStyle = toggleUpStyle;
		
		// vvvvvv test code vvvvvv 
		expanded = true;
		AddLabel("test");
		AddLabel("123");
		AddLabel(":)");
		AddLabel("what");
		// ^^^^^^ test code ^^^^^^ 
	}
	
	public void Draw() {
		toggleRect.x = rect.x - 16;
		toggleRect.y = rect.y + 2;
		outerViewRect.x = rect.x - 20;
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
			int deleteIndex = -1;
			
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect);
			for (int i = 0; i < labels.Count; i++) {
				// draw each label and update scroll view
				labels[i].scrollViewOffset = outerViewRect.position - scrollPos;
				labels[i].Draw();
				
				// draw each label's remove button
				if (GUI.Button(new Rect(6, i*LABEL_OFFSET+4, 12, 12), "-", SDEStyles.textButtonDefault)) {
					// show dialog to confirm
					deleteIndex = i;
				}
			}
			GUI.EndScrollView();
			
			if (deleteIndex >= 0) {
				CallOnDelete(labels[deleteIndex].text);
				labels.RemoveAt(deleteIndex);
			}
		} 
	}
	
	public void ProcessEvent(Event e) {
		// run all the TextAreas
		for (int i = 0; i < labels.Count; i++) {
			labels[i].ProcessEvent(e);
		}
		
		// TODO: implement the rest
		switch(e.type) {
		case EventType.MouseDown:
			break;
		}
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
		if (labelMap.ContainsKey(label)) {
			Debug.Log("Dropdown already contains: " + label);
			return;
		} 
		
		TextArea newText = CreateTextArea(label);
		labels.Add(newText);
		labelMap.Add(label, newText);
	}
	
	private void RemoveLabel(string label) {
		labels.Remove(labelMap[label]);
		labelMap.Remove(label);
	}
	
	private void ContainsLabel(string label) {
		// TODO: implement this
	}
	
	private void SortLabels() {
		// TODO: implement this
	}
	
	private TextArea CreateTextArea(string text) {
		TextArea textArea = ScriptableObject.CreateInstance<TextArea>();
		textArea.Init(text, rect.width, LABEL_HEIGHT, labels.Count * LABEL_OFFSET);
		textArea.maxLength = 16;
		textArea.textAreaStyle = SDEStyles.textAreaSmallDefault;
		
		return textArea;
	}
	
	private void CallOnDelete(string text) {
		if (OnDelete != null) {
			OnDelete(text);
		}
	}
}