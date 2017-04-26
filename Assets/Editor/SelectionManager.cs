using UnityEngine;

public static class SelectionManager {
	
	// persistent vars that are changed on input
	private static SDEComponentType _selectedComponentType;
	
	// per-event vars used to check for changes
	private static bool componentSelectedOnEvent;
	private static bool eventSelectionStarted;
	
	public static SDEComponentType SelectedComponentType() {
		return _selectedComponentType;
	}
	
	public static void SelectComponent(SDEComponentType type) {
		_selectedComponentType = type;
		componentSelectedOnEvent = true;
	}
	
	public static bool IsComponentSelectedOnEvent() {
		if (eventSelectionStarted) {
			return componentSelectedOnEvent;
		} else {
			throw new UnityException("Tried to check if ComponentSelectedAlready outside of selection event processing!");
		}
	}
	
	public static void StartSelectionEventProcessing(Event e) {
		if (e.type == EventType.MouseDown && e.button == 0) { 
			eventSelectionStarted = true;
			
			componentSelectedOnEvent = false;
			_selectedComponentType = SDEComponentType.Nothing;
		}
	}
	
	public static void EndSelectionEventProcessing(Event e) {
		if (e.type == EventType.MouseDown && e.button == 0) {
			Debug.Log(_selectedComponentType);
			eventSelectionStarted = false;
			
			componentSelectedOnEvent = false;
		}
	}
}
