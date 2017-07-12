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
	
	private static void AssignNIDs(Dictionary<Node, int> nodeMap) {
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
	[XmlArray("lf")]
	[XmlArrayItem("s")]
	public List<string> localFlags;
	
	[XmlArray("ne")]
	[XmlArrayItem("n")]
	public List<NodeEntry> nodeEntries;
}

[System.Serializable]
public class NodeEntry {
	[XmlElement("id")]
	public int NID;
	
	[XmlElement("nt")]
	public NodeType nodeType;
	
	[XmlElement("c")]
	public ChildEntry child;
	
	[XmlElement("op")]
	public int outPointNID;
	
	[XmlElement("opp")]
	public int outPointPosNID;
	
	[XmlElement("opn")]
	public int outPointNegNID;
	
	[XmlElement("f")]
	public string flag;
}

[System.Serializable]
public class ChildEntry {
	[XmlElement("c")]
	public ChildEntry child;
	
	[XmlElement("op")]
	public int outPointNID;
	
	[XmlElement("t")]
	public string text;
}