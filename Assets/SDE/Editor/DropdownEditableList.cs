using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownEditableList : ToggleMenu {
	
	// holds the list of items that are used
	public List<TextArea> items;
	
	// used to keep track of edits
	private Dictionary<TextArea, string> preEditMap;
	
	// Dictionary<TextArea, List<Node>> nodeMap: originally planned to use this to update Nodes when 
	// items change, but the Nodes should be in charge of updating themselves. LocalVariable Nodes 
	// should just have a reference to the TextArea itself, and if null, print the broken link text ("---"). 
	// That would minimize coupling while keeping it cohesive
	
	// this value is set when the TextArea is highlighted, to ensure we know what text was modified
	private string selectedItemStartText;
	private TextArea selectedItem;
	
	
	public DropdownEditableList() {}
	public override void Init() {
		base.Init();
		
		items = new List<TextArea>();
		preEditMap = new Dictionary<TextArea, string>();
	}
	
	public override void Draw() {
		base.Draw();
		
		// draw the togle button and the label
		if (GUI.Button(toggleRect, "", toggleStyle) || 
			GUI.Button(rect, "Local Flags", SDEStyles.textButtonDefault))
		{
			if (expanded) {
				Close();
			} else {
				Expand();
			}
		}
		
		if (expanded) {
			// start scroll view 
			int deleteIndex = -1;
			
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect, false, false);
			for (int i = 0; i < items.Count; i++) {
				// draw each item and update scroll view
				items[i].scrollViewOffset = outerViewRect.position - scrollPos;
				items[i].Draw();
				
				// draw each item's remove button
				if (GUI.Button(new Rect(6, i*ITEM_OFFSET+4, 12, 12), "-", SDEStyles.textButtonDefault)) {
					// show dialog to confirm
					deleteIndex = i;
				}
			}
			GUI.EndScrollView();
			
			// calculate where the bottom of the last visible element of the scroll view is
			float yPos = outerViewRect.y;
			
			// get the number of elements
			float yMod = Mathf.Min(((float)items.Count*(float)ITEM_OFFSET)-scrollPos.y, outerViewRect.height);
			if (yMod < 0) {
				yMod = 0;
			}
			
			yPos += yMod;
			
			// draw the add new item button
			if (GUI.Button(new Rect(outerViewRect.x+20, yPos, rect.width, 20), "+item", SDEStyles.textButtonDefault)) {
				int i = 0;
				string newLabel = "new var " + i;
				while (!AddItem(newLabel)) {
					i++;
					newLabel = "new var " + i;
				}
			}
			
			if (deleteIndex >= 0) {
				RemoveItem(deleteIndex);
			}
		} 
	}
	
	public override void ProcessEvent(Event e) {
		// process the TextAreas
		if (expanded) {
			for (int i = 0; i < items.Count; i++) {
				items[i].ProcessEvent(e);
			}
		}
	}
	
	public bool AddItem(string item, bool markHistory=true) {
		if (markHistory) {
			HistoryManager.RecordDropdown(this);
		}
		
		for (int i = 0; i < items.Count; i++) {
			if (item == items[i].text) {
				Debug.Log("Dropdown already contains: " + item);
				return false;	
			}
		}
		
		TextArea newText = CreateTextArea(item);
		items.Add(newText);
		
		// expand the size of the inside space
		innerViewRect.height = items.Count * ITEM_OFFSET;
		
		return true;
	}
	
	public void RemoveItem(int index, bool markHistory=true) {
		if (markHistory) {
			HistoryManager.RecordDropdown(this);
		}
		
		// drop the position of the items above the deleted one
		for (int i = index; i < items.Count; i++) {
			items[i].rect.y -= ITEM_OFFSET;
			items[i].clickRect.y -= ITEM_OFFSET;
		}
		
		preEditMap.Remove(items[index]);
		items.RemoveAt(index);
		
		// shrink the size of the scrollview space
		innerViewRect.height = items.Count * ITEM_OFFSET;
	}
	
	private TextArea CreateTextArea(string text) {
		TextArea textArea = ScriptableObject.CreateInstance<TextArea>();
		textArea.Init(text, rect.width, ITEM_HEIGHT, items.Count * ITEM_OFFSET);
		
		textArea.maxLength = MAX_TEXT_LENGTH;
		textArea.textAreaStyle = SDEStyles.textAreaSmallDefault;
		
		// set callbacks
		textArea.OnTextAreaSelect += SetLabelStartText;
		textArea.OnTextAreaDeselect += RevertIfDuplicated;
		
		return textArea;
	}
	
	public TextArea GetTextArea(string text) {
		for (int i = 0; i < items.Count; i++) {
			if (items[i].text == text) {
				return items[i];
			}
		}
		return null;
		
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
		
		// run through all the items for matches
		for (int i = 0; i < items.Count; i++) {
			// check if the dropdown text matches with anything but itself
			if (text == items[i].text && dropdownItem != items[i]) {
				dropdownItem.text = preEditMap[dropdownItem];
				return;
			}
		}
	}
}