using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : ScriptableObject {
	
	// holds the list of labels that are used
	private List<string> labels;
	
	// maps a label to an index for fast lookups, additions, and removals
	private Dictionary<string, int> labelMap;
	
	public DropdownMenu() {}
	public void Init() {
		// TODO: implement this
	}
	
	public void Draw() {
		// TODO: implement this
	}
	
	public void ProcessEvent(Event e) {
		// TODO: implement this
	}
	
	private void AddLabel(string label) {
		// TODO: implement this
	}
	
	private void RemoveLabel(string label) {
		// TODO: implement this
	}
	
	private void ContainsLabel(string label) {
		// TODO: implement this
	}
	
	private void SortLabels() {
		// TODO: implement this
	}
	
	private void UpdateLabelMap() {
		labelMap.Clear();
		for (int i = 0; i < labels.Count; i++) {
			labelMap.Add(labels[i], i);
		}
	}
}