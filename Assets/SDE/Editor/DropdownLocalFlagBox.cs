using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  DropdownBoxes are like a combo box without the ability to type into it. A dropdown menu allows
  the user to choose an item from a predefined list of items.
  
  LocalFlags are accessed through the StoryEditor's current local flag dropdown menu
*/
public class DropdownLocalFlagBox : ToggleMenu {
	private List<TextArea> flagListReference;
	
	public TextArea selectedItem;
	
	public DropdownLocalFlagBox(){}
	public override void Init() {
		base.Init();
		
		this.outerViewRect.height = 100;
		
		this.flagListReference = new List<TextArea>();
	}
	
	public override void Draw() {
		base.Draw();
		
		// draw the toggle button and selected value
		if (GUI.Button(toggleRect, "", toggleStyle) || 
			GUI.Button(rect, (selectedItem != null ? '['+selectedItem.text+']' : "---"), SDEStyles.textButtonDefault))
		{
			if (expanded) {
				Close();
			} else {
				Expand();
			}
		}
		
		
		if (expanded) {
			// start the scroll view
			innerViewRect.height = flagListReference.Count * ITEM_OFFSET;
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect);
			
			for (int i = 0; i < flagListReference.Count; i++) {
				if (GUI.Button(new Rect(20, i*ITEM_OFFSET, rect.width, ITEM_HEIGHT), flagListReference[i].text, SDEStyles.textButtonDefault)) {
					selectedItem = flagListReference[i];
					Close();
				}
			}
			GUI.EndScrollView();
		}
	}
	
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
	}
	
	public void LinkFlags(List<TextArea> listReference) {
		flagListReference = listReference;
	}
}
