using UnityEngine;

/*
  SelectionManager keeps track of the selected component in the editor, and 
  per-event selections.
*/
public static class SelectionManager {
	
	// persistent vars that are changed on input
	private static SDEComponent _selectedComponent;
	
	private static SDEComponent _selectedEventComponent;
	
	// per-event vars used to check for changes
	private static bool componentSelectedOnEvent;
	private static bool eventSelectionStarted;
	
	public static SDEComponent SelectedComponent() {
		return _selectedComponent;
	}
	
	/*
	  SelectedComponentType() returns the type of the selected component, or
	  SDEComponentType.Nothing if nothing is selected
	*/
	public static SDEComponentType SelectedComponentType() {
		if (_selectedComponent != null) {
			return _selectedComponent.componentType;
		} else {
			return SDEComponentType.Nothing;
		}
	}
	
	/*
	  Deselect() removes the given component from the current selection.
	
	  If the component is not part of the selection, or if there is
	  currently nothing selected, then nothing happens.
	*/
	public static void Deselect(SDEComponent component) {
		if (_selectedComponent == null) {
			return;
		}
		
		if (_selectedComponent.Equals(component)){
			_selectedComponent = null;
		}
	}
	
	/*
	  ClearSelection() resets the selection
	*/
	public static void ClearSelection() {
		_selectedComponent = null;
	}
	
	/*
	  SelectComponent() gives selection awareness of the given component.
	*/
	public static void SelectComponent(SDEComponent component) {
		_selectedComponent = component;
		_selectedEventComponent = component;
		componentSelectedOnEvent = true;
	}
	
	/*
	  Checks if a component was selected during an event processing queue.
	
	  Throws an exception if called outside of an event processing queue.
	*/
	public static bool IsComponentSelectedOnEvent() {
		if (eventSelectionStarted) {
			return componentSelectedOnEvent;
		} else {
			throw new UnityException("Tried to check if ComponentSelectedAlready outside of selection event processing!");
		}
	}
	
	/*
	  Used at the start of an event processing queue to check if anything
	  was selected within that queue.
	
	  For now, only handled on mouse down, as keyboard input does not influence
	  component selection.
	*/
	public static void StartSelectionEventProcessing(Event e) {
		if (e.type == EventType.MouseDown) { 
			eventSelectionStarted = true;
			
			componentSelectedOnEvent = false;
			_selectedEventComponent = null;
		}
	}
	
	public static void EndSelectionEventProcessing(Event e) {
		if (e.type == EventType.MouseDown) {
			eventSelectionStarted = false;
			
			componentSelectedOnEvent = false;
			
			if (_selectedEventComponent == null) {
				_selectedComponent = null;
			}
		}
	}
}
