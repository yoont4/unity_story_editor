using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  ClickManager manages all processes regarding special click input.
*/
public static class ClickManager {
	
	public static float lastClickTime;
	public static SDEComponentType lastClickType;
	public static Vector2 lastClickPosition;
	
	// somewhere between 0.2 and 0.4 is good
	public static float doubleClickFrame = 0.2f;
	
	// tolerable distance between double clicks.
	public static float clickPositionFrame = 1f;
	
	/*
	  IsDoubleClick() takes the given time of a click, and determines 
	  if it was a double click or not. 
	
	  Timing defined by doubleClickFrame. Both clicks must be on the background
	  for it to register.
	*/
	public static bool IsDoubleClick(float clickTime, Vector2 position, SDEComponentType componentType) {
		bool ret = false;
		
		if (clickTime-lastClickTime < doubleClickFrame) {
			if (lastClickType == componentType &&
				SelectionManager.SelectedComponentType() == componentType &&
				Mathf.Abs((position.SqrMagnitude() - lastClickPosition.SqrMagnitude())) < clickPositionFrame) {
				ret = true;
			}
		} 
		
		// update click vars for next cycle
		lastClickTime = clickTime;
		lastClickType = SelectionManager.SelectedComponentType();
		lastClickPosition = position;
		
		return ret;
	}
}
