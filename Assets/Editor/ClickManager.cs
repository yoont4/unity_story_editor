using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  ClickManager manages all processes regarding special click input.
*/
public static class ClickManager {
	
	public static float lastClickTime;
	public static SDEComponentType lastClickType;
	
	// somewhere between 0.2 and 0.4 is good
	public static float doubleClickFrame = 0.2f;
	
	// when this hits 2, 2 consecutive proper clicks have been made
	private static int clickCount = 0;
	
	/*
	  IsDoubleClick() takes the given time of a click, and determines 
	  if it was a double click or not. 
	
	  Timing defined by doubleClickFrame. Both clicks must be on the background
	  for it to register.
	*/
	public static bool IsDoubleClick(float clickTime, SDEComponentType componentType) {
		bool ret = false;
		
		if (clickTime-lastClickTime < doubleClickFrame) {
			if (lastClickType == componentType &&
				SelectionManager.SelectedComponentType() == componentType) {
				ret = true;
			}
			
			lastClickTime = clickTime;
			lastClickType = SelectionManager.SelectedComponentType();
		} else {
			lastClickTime = clickTime;
			lastClickType = SelectionManager.SelectedComponentType();
		}
		
		return ret;
	}
}
