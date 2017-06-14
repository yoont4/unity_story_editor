using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleMenu : ScriptableObject {
	
	// represents the currently selected label index.
	// -1 means nothing selected
	public int selectedIndex;
	
	protected bool expanded;
	
	public Rect rect;
	public Rect toggleRect;
	
	// scroll view vars
	protected Rect outerViewRect;
	protected Rect innerViewRect;
	protected Vector2 scrollPos;
	
	public GUIStyle boxStyle;
	public GUIStyle toggleStyle;
	public GUIStyle toggleUpStyle;
	public GUIStyle toggleDownStyle;
	
	public const int LABEL_HEIGHT = 20;
	public const int LABEL_OFFSET = 22;
	public const int MAX_TEXT_LENGTH = 16;
	
	public abstract void Init();
	
	public virtual void Draw() {
		toggleRect.x = rect.x - 16;
		toggleRect.y = rect.y + 2;
		outerViewRect.x = rect.x - 20;
		outerViewRect.y = rect.y + 20;
	}
	
	public abstract void ProcessEvent(Event e);
	
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