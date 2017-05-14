using System;
using UnityEngine;
using UnityEditor;

public enum SDEComponentType {Nothing, Node, ConnectionPoint, TextArea, Label}

/*
  SDEComponent is the parent class of all components spawned in the StoryDialogEditor.
  
  This handles all of the selection, positioning, and click frame definitions of a specific
  component.
*/
public abstract class SDEComponent : ScriptableObject {
	
	// determines what component type it is (Node, ConnectionPoint, etc.)
	public SDEComponentType componentType;
	
	// if this is null, then this isn't bound to a container.
	public SDEContainer container;
	
	// if this is null, then this a root component.
	public SDEComponent parent;
	
	// this defines the position and size of the component.
	public Rect rect;
	
	// this overrides the element Rect size for click/interaction detection.
	// if it's null, then just use rect.
	public Rect clickRect;
	
	// denotes the width and height difference between rect and clickRect
	public float widthPad = 0f;
	public float heightPad = 0f;
	public bool padded = false;
	
	// all Components are selectable, and have a default and selected
	// style at minimum. More styles can be given to individual components.
	private bool _isSelected;
	
	public Action<SDEComponent> OnSelect;
	public Action<SDEComponent> OnDeselect;
	
	public GUIStyle style;
	public GUIStyle defaultStyle;
	public GUIStyle selectedStyle;
	
	public SDEComponent() {}
	
	public void Init(SDEComponentType componentType, SDEComponent parent, Rect rect, GUIStyle style, GUIStyle defaultStyle, GUIStyle selectedStyle, SDEContainer container=null) {
		this.componentType = componentType;
		this.parent = parent;
		this.container = container;
		this.rect = rect;
		
		// default click rect to be the same as the rect, but only specific component types
		// will continuously update the clickRect position.
		this.clickRect = rect;
		this.style = style;
		this.defaultStyle = defaultStyle;
		this.selectedStyle = selectedStyle;
	}
	
	public abstract void Draw();
	
	/*
	   ProcessEvent handles Events run over this Component.
		
	   this is virtual, so that Component level arguments passed around can
	   still process the event chain from the derived class.
	*/
	public virtual void ProcessEvent(Event e) {
		switch(e.type) {
		case EventType.MouseDown:
		// handle selection clicks
			if (e.button == 0) {
				if (!SelectionManager.IsComponentSelectedOnEvent() && 
				((padded && clickRect.Contains(e.mousePosition)) || rect.Contains(e.mousePosition))) {
					// select the component.
					if (!Selected) {
						Selected = true;
					} else {
						// make sure it still registers with the Selection Manager
						SelectionManager.SelectComponent(this);
					}
				} else {
					// deselect the component.
					if (Selected) {
						Selected = false;
					}
				}
			}
			break;
		}
	}
	
	/*
	  Selected getter/setter allows the selection status
	  of an SDEComponent from being directly mutable.
	*/
	public bool Selected {
		get { return _isSelected; }
		set {
			if (value) {
				Select();
			} else {
				Deselect();
			}
		}
	}
	
	/*
	  Select() marks the SDEComponent as selected, and loads the
	  selected GUIStyle before marking the GUI changed.
	*/
	private void Select() {
		// stop the selection event from propogating further.
		SelectionManager.SelectComponent(this);
		
		_isSelected = true;
		style = selectedStyle;
		GUI.changed = true;
		CallOnSelect();
	}
	
	/*
	  Select() deselects the SDEComponent, and loads the
	  default GUIStyle before marking the GUI changed.
	*/
	private void Deselect() {
		SelectionManager.Deselect(this);
		
		_isSelected = false;
		style = defaultStyle;
		GUI.changed = true;
		CallOnDeselect();
	}
	
	private void CallOnSelect() {
		if (OnSelect != null) {
			OnSelect(this);
		}
	}
	
	private void CallOnDeselect() {
		if (OnDeselect != null) {
			OnDeselect(this);
		} 
	}
	
	/*
	  defines a new click boundary around the GUI rect by taking the original
	  width and height, and adding the modifier.
	*/
	public void ExtendClickBound(Vector2 modifier) {
		widthPad = modifier.x;
		heightPad = modifier.y;
		padded = true;
		
		clickRect.width = rect.width + 2 * widthPad;
		clickRect.height = rect.height + 2 * heightPad;
		clickRect.x = rect.x - widthPad;
		clickRect.y = rect.y - heightPad;
	}
	
}
