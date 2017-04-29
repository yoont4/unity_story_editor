using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  ClickManager manages all processes regarding special click input.
*/
public static class ClickManager {
	
	public static float lastClickTime;
	
	// somewhere between 0.2 and 0.4 is good
	public static float doubleClickFrame = 0.2f;
	
	/*
	  IsDoubleClick() takes the given time of a click, and determines 
	  if it was a double click or not. 
	
	  Timing defined by doubleClickFrame.
	*/
	public static bool IsDoubleClick(float clickTime) {
		if (clickTime-lastClickTime < doubleClickFrame) {
			lastClickTime = clickTime;
			return true;
		} else {
			lastClickTime = clickTime;
			return false;
		}
	}
}
