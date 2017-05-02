using UnityEngine;
using UnityEditor;

public enum SDEComponentType {Nothing, Node, ConnectionPoint, TextArea}

/*
  SDEComponent is the parent class of all components spawned in the StoryDialogueEditor.
  
  This handles all of the selection, positioning, and click frame definitions of a specific
  component.
*/
public class SDEComponent : ScriptableObject {	// TODO: FIGURE OUT HOW TO FUCKING UNDO
	
	// determines what component type it is (Node, ConnectionPoint, etc.)
	public SDEComponentType componentType;
	
	// if this is null, then this a parent component.
	public SDEComponent parent;
	
	// this defines the position and size of the component.
	public Rect rect;
	
	// this overrides the element Rect size for click/interaction detection.
	// if it's null, then just use rect.
	public Rect clickRect;
	
	// denotes the width and height difference between rect and clickRect
	public float widthPad;
	public float heightPad;
	
	// all Components are selectable, and have a default and selected
	// style at minimum. More styles can be given to individual components.
	private bool _isSelected;
	
	public GUIStyle style;
	public GUIStyle defaultStyle;
	public GUIStyle selectedStyle;
	
	public SDEComponent() {}
	
	public void Init(SDEComponentType componentType, SDEComponent parent, Rect rect, GUIStyle style, GUIStyle defaultStyle, GUIStyle selectedStyle) {
		this.componentType = componentType;
		this.parent = parent;
		this.rect = rect;
		this.style = style;
		this.defaultStyle = defaultStyle;
		this.selectedStyle = selectedStyle;
	}
	
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
				(clickRect.Contains(e.mousePosition) || rect.Contains(e.mousePosition))) {
					// select the component.
					Selected = true;
				} else {
					// deselect the component.
					Selected = false;
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
	}
	
	/*
	  Select() deselects the SDEComponent, and loads the
	  default GUIStyle before marking the GUI changed.
	*/
	private void Deselect() {
		_isSelected = false;
		style = defaultStyle;
		GUI.changed = true;
	}
	
	/*
	  defines a new click boundary around the GUI rect by taking the original
	  width and height, and adding the modifier.
	*/
	public void ExtendClickBound(Vector2 modifier) {
		widthPad = modifier.x;
		heightPad = modifier.y;
		
		clickRect.width = rect.width + 2 * widthPad;
		clickRect.height = rect.height + 2 * heightPad;
		clickRect.x = rect.x - widthPad;
		clickRect.y = rect.y - heightPad;
		
	}
	
}
