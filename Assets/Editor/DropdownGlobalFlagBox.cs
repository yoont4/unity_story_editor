using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  DropdownBoxes are like a combo box without the ability to type into it. A dropdown menu allows
  the user to choose an item from a predefined list of items.
  
  GlobalFlags are accessed through a global constant file
*/
public class DropdownGlobalFlagBox : ToggleMenu {
	private List<string> items;
	
	public string selectedItem = "---";
	
	public DropdownGlobalFlagBox(){}
	public override void Init() {
		base.Init();
		
		this.outerViewRect.height = 100;
		
		this.items = new List<string>();
	}
	
	public override void Draw() {
		base.Draw();
		
		// draw the toggle button and selected value
		if (GUI.Button(toggleRect, "", toggleStyle) || 
			GUI.Button(rect, '['+selectedItem+']', SDEStyles.textButtonDefault))
		{
			if (expanded) {
				Close();
			} else {
				Expand();
			}
		}
		
		
		if (expanded) {
			// start the scroll view
			innerViewRect.height = items.Count * ITEM_OFFSET;
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect);
			
			for (int i = 0; i < items.Count; i++) {
				if (GUI.Button(new Rect(20, i*ITEM_OFFSET, rect.width, ITEM_HEIGHT), items[i], SDEStyles.textButtonDefault)) {
					selectedItem = items[i];
					Close();
				}
			}
			GUI.EndScrollView();
		}
		// TODO: implement this
	}
	
	public override void ProcessEvent(Event e) {
		// TODO: implement this
	}
	
	public void LinkItems(List<string> itemList) {
		items = itemList;
	}
}
