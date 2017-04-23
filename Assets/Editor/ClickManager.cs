using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClickManager {
	
	public static float lastClickTime;
	
	// somewhere between 0.2 and 0.4 is good
	public static float doubleClickFrame = 0.2f;
	
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
