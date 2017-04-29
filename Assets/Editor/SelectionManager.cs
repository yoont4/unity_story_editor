using UnityEngine;

public static class SelectionManager {
	
	// persistent vars that are changed on input
	private static SDEComponent _selectedComponent;
	
	// per-event vars used to check for changes
	private static bool componentSelectedOnEvent;
	private static bool eventSelectionStarted;
	
	public static SDEComponentType SelectedComponentType() {
		if (_selectedComponent != null) {
			return _selectedComponent.componentType;
		} else {
			return SDEComponentType.Nothing;
		}
	}
	
	public static void ClearSelection() {
		_selectedComponent = null;
	}
	
	public static void SelectComponent(SDEComponent component) {
		_selectedComponent = component;
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
			_selectedComponent = null;
		}
	}
	
	public static void EndSelectionEventProcessing(Event e) {
		if (e.type == EventType.MouseDown) {
			Debug.Log(SelectedComponentType());
			eventSelectionStarted = false;
			
			componentSelectedOnEvent = false;
		}
	}
}
