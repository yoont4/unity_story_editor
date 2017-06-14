using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : ToggleMenu {
	
	// holds the list of labels that are used
	public List<TextArea> labels;
	
	// used to keep track of edits
	private Dictionary<TextArea, string> preEditMap;
	
	// Dictionary<TextArea, List<Node>> nodeMap: originally planned to use this to update Nodes when 
	// labels change, but the Nodes should be in charge of updating themselves. LocalVariable Nodes 
	// should just have a reference to the TextArea itself, and if null, print the broken link text ("---"). 
	// That would minimize coupling while keeping it cohesive
	
	// this value is set when the TextArea is highlighted, to ensure we know what text was modified
	private string selectedItemStartText;
	private TextArea selectedItem;
	
	
	public DropdownMenu() {}
	public override void Init() {
		labels = new List<TextArea>();
		preEditMap = new Dictionary<TextArea, string>();
		
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
	
	public override void Draw() {
		base.Draw();
		
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
			
			// calculate where the bottom of the last visible element of the scroll view is
			float yPos = outerViewRect.y;
			
			// get the number of elements
			float yMod = Mathf.Min(((float)labels.Count*(float)LABEL_OFFSET)-scrollPos.y, outerViewRect.height);
			if (yMod < 0) {
				yMod = 0;
			}
			
			yPos += yMod;
			
			// draw the add new label button
			if (GUI.Button(new Rect(outerViewRect.x+20, yPos, rect.width, 20), "+item", SDEStyles.textButtonDefault)) {
				// TODO: implement this
			}
			
			if (deleteIndex >= 0) {
				RemoveLabel(deleteIndex);
			}
		} 
	}
	
	public override void ProcessEvent(Event e) {
		// process the TextAreas
		if (expanded) {
			for (int i = 0; i < labels.Count; i++) {
				labels[i].ProcessEvent(e);
			}
		}
		
		// TODO: implement the rest (figure out if necessary)
		switch(e.type) {
		case EventType.MouseDown:
			break;
		}
	}
	
	private void AddLabel(string label) {
		// TODO: figure out why this breaks initializtion
		//HistoryManager.RecordDropdown(this);
		
		for (int i = 0; i < labels.Count; i++) {
			if (label == labels[i].text) {
				Debug.Log("Dropdown already contains: " + label);
				return;	
			}
		}
		
		TextArea newText = CreateTextArea(label);
		labels.Add(newText);
	}
	
	private void RemoveLabel(int index) {
		HistoryManager.RecordDropdown(this);
		
		// drop the position of the labels above the deleted one
		for (int i = index; i < labels.Count; i++) {
			labels[i].rect.y -= LABEL_OFFSET;
			labels[i].clickRect.y -= LABEL_OFFSET;
		}
		
		preEditMap.Remove(labels[index]);
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
	
	/*
	  SetLabelStartText() is a callback function used with RevertIfDuplicated() to set the 
	  pre-edit value of a TextArea.
	*/
	private void SetLabelStartText(TextArea dropdownItem) {
		preEditMap[dropdownItem] = dropdownItem.text;
	}
	
	/*
	  RevertIfDuplicated() is a callback function, used to revert a TextArea's text value to
	  its pre-edit value if the post-edit value is a duplicate.
	*/
	private void RevertIfDuplicated(TextArea dropdownItem) {
		string text = dropdownItem.text;
		
		// run through all the labels for matches
		for (int i = 0; i < labels.Count; i++) {
			// check if the dropdown text matches with anything but itself
			if (text == labels[i].text && dropdownItem != labels[i]) {
				dropdownItem.text = preEditMap[dropdownItem];
				return;
			}
		}
	}
}