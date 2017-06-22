using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  DropdownBoxes are like a combo box without the ability to type into it. A dropdown menu allows
  the user to choose an item from a predefined list of items.
  
  TODO: separate this off into a DropdownLocalFlagsBox class or something, because this has very specific
  item binding to an external list reference.
*/
public class DropdownBox : ToggleMenu {
	public List<string> items;
	private List<TextArea> flagListReference;
	
	public TextArea selectedItem;
	
	public DropdownBox(){}
	public override void Init() {
		base.Init();
		
		this.outerViewRect.height = 100;
		
		this.items = new List<string>();
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
			// TODO: this REALLY should be a unique subclass because of this binding behavior as well.
			innerViewRect.height = flagListReference.Count * ITEM_OFFSET;
			scrollPos = GUI.BeginScrollView(outerViewRect, scrollPos, innerViewRect);
			
			// TODO: this really should be a unique subclass since it has very specific binding behavior
			for (int i = 0; i < flagListReference.Count; i++) {
				if (GUI.Button(new Rect(20, i*ITEM_OFFSET, rect.width, ITEM_HEIGHT), flagListReference[i].text, SDEStyles.textButtonDefault)) {
					selectedItem = flagListReference[i];
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
	
	public void LinkFlags(List<TextArea> listReference) {
		flagListReference = listReference;
	}
}
