using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

/*
  XMLManager is responsible for saving/loading XML data for game engine use.
*/
public static class XMLManager {
	
	public static StoryDialogEditor mainEditor;
	
	private static int NIDCounter = 0;
	
	public static bool SaveXML(string path) {
		// TODO: implement this
		return false;
	}
	
	private static void SavePrePass(Dictionary<Node, int> nodeMap) {
		// assign every node a Node ID (NID)
		for (int i = 0; i < mainEditor.nodes.Count; i++) {
			nodeMap[mainEditor.nodes[i]] = GenerateNID();
		}
		
		
	}
	
	private static int GenerateNID() {
		NIDCounter++;
		return NIDCounter-1;
	}
}

// ----------------------------------------------------------------------------------------------- //
// ---------------------------------------- ENTRY CLASSES ---------------------------------------- //
// ----------------------------------------------------------------------------------------------- //

[System.Serializable]
public class StoryNodeEntry {
	public List<string> localFlags;
	public List<NodeEntry> nodeEntries;
}

[System.Serializable]
public class NodeEntry {
	public int NID;
	public NodeType nodeType;
	
	public ChildEntry child;
	
	public List<int> outPointNIDs;
	public List<int> outPointPosNIDs;
	public List<int> outPointNegNIDs;
	public string flag;
}

[System.Serializable]
public class ChildEntry {
	public ChildEntry child;
	public List<int> outPointNIDs;
	public string text;
}