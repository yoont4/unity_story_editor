using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : ScriptableObject {
	
	// holds the list of labels that are used
	public List<TextArea> labels;
	
	// used to keep track of labels and help with updating, checking for changes, etc.
	public Dictionary<String, TextArea> labelMap;
	
	// TODO: figure out if this is necessary.
	// originally planned to use this to update Nodes when labels change, but the Nodes should
	// be in charge of updating themselves. LocalVariable Nodes should just have a reference to the 
	// TextArea itself, and if null, print the broken link text ("---"). That would minimize coupling
	public Dictionary<String, List<Node>> nodeMap;
	
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
	public const int MAX_TEXT_LENGTH = 16;
	
	// this value is set when the TextArea is highlighted, to ensure we know what text was modified
	private string selectedLabelStartText;
	
	public DropdownMenu() {}
	public void Init() {
		labels = new List<TextArea>();
		labelMap = new Dictionary<String, TextArea>();
		nodeMap = new Dictionary<string, List<Node>>();
		
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
				RemoveLabel(deleteIndex);
			}
		} 
	}
	
	public void ProcessEvent(Event e) {
		// process the TextAreas
		if (expanded) {
			for (int i = 0; i < labels.Count; i++) {
				labels[i].ProcessEvent(e);
			}
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
		// TODO: figure out why this breaks initializtion
		//HistoryManager.RecordDropdown(this);
		
		if (labelMap.ContainsKey(label)) {
			Debug.Log("Dropdown already contains: " + label);
			return;
		} 
		
		TextArea newText = CreateTextArea(label);
		labels.Add(newText);
		labelMap.Add(label, newText);
		nodeMap.Add(label, new List<Node>());
	}
	
	private void RemoveLabel(int index) {
		HistoryManager.RecordDropdown(this);
		
		// drop the position of the labels above the deleted one
		for (int i = index; i < labels.Count; i++) {
			labels[i].rect.y -= LABEL_OFFSET;
			labels[i].clickRect.y -= LABEL_OFFSET;
		}
		
		CallOnDelete(labels[index].text);
		labelMap.Remove(labels[index].text);
		nodeMap.Remove(labels[index].text);
		labels.RemoveAt(index);
		
	}
	
	private void ContainsLabel(string label) {
		// TODO: implement this
	}
	
	private void SortLabels() {
		// TODO: implement this
	}
	
	private TextArea CreateTextArea(string text) {
		// TODO: figure out why this breaks initializtion
		//HistoryManager.RecordDropdown(this);
		
		TextArea textArea = ScriptableObject.CreateInstance<TextArea>();
		textArea.Init(text, rect.width, LABEL_HEIGHT, labels.Count * LABEL_OFFSET);
		
		textArea.maxLength = MAX_TEXT_LENGTH;
		textArea.textAreaStyle = SDEStyles.textAreaSmallDefault;
		
		// set callbacks
		textArea.OnTextAreaSelect += SetLabelStartText;
		textArea.OnTextAreaDeselect += RevertIfDuplicated;
		
		return textArea;
	}
	
	// TODO: decide if this will ever be needed or if self-updating local variable Nodes
	// handles all of the use cases of this.
	private void SetLabelStartText(TextArea dropdownItem) {
		selectedLabelStartText = dropdownItem.text;
	}
	
	private void RevertIfDuplicated(TextArea dropdownItem) {
		// TODO: implement this
		// if there is already a duplicate dropdown item, then revert to the original value.
	}
	
	private void CallOnDelete(string text) {
		if (OnDelete != null) {
			OnDelete(text);
		}
	}
}