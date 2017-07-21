using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleMenu : ScriptableObject {
	
	// represents the currently selected item index.
	// -1 means nothing selected
	public int selectedIndex;
	
	protected bool expanded;
	
	public Rect rect;
	public Rect toggleRect;
	
	// scroll view vars
	protected Rect outerViewRect;
	protected Rect innerViewRect;
	protected Vector2 scrollPos;
	
	public GUIStyle toggleStyle;
	public GUIStyle toggleUpStyle;
	public GUIStyle toggleDownStyle;
	
	public const int ITEM_HEIGHT = 20;
	public const int ITEM_OFFSET = 22;
	public const int MAX_TEXT_LENGTH = 16;
	
	public virtual void Init() {
		selectedIndex = -1;
		expanded = false;
		rect = new Rect(0, 0, 140, ITEM_HEIGHT);
		toggleRect = new Rect(0, 0, 16, 16);
		outerViewRect = new Rect(0, 0, rect.width+40, 300);
		innerViewRect = new Rect(0, 0, rect.width, 0);
		
		toggleUpStyle = SDEStyles.toggleUpDefault;
		toggleDownStyle = SDEStyles.toggleDownDefault;
		toggleStyle = toggleDownStyle;
	}
	
	public virtual void Draw() {
		toggleRect.x = rect.x - 13;
		toggleRect.y = rect.y + 2;
		outerViewRect.x = rect.x - 20;
		outerViewRect.y = rect.y + 20;
	}
	
	public virtual void ProcessEvent(Event e) {
		switch(e.type) {
		case EventType.MouseDown:
			if (!toggleRect.Contains(e.mousePosition) && !rect.Contains(e.mousePosition) && !outerViewRect.Contains(e.mousePosition)) {
				Close();
			}
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
}