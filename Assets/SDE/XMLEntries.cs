using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

// the master story entry
[System.Serializable]
public class StoryNodeEntry {
	[XmlArray("lf")]
	[XmlArrayItem("s")]
	public List<string> localFlags;
	
	[XmlArray("ns")]
	[XmlArrayItem("n")]
	public List<NodeEntry> nodeEntries;
	
	[XmlElement("en")]
	public int entryNID;
	
	public StoryNodeEntry() {
		this.entryNID = -1;
	}
}

[System.Serializable]
public class NodeEntry {
	[XmlElement("id")]
	public int NID;
	
	[XmlElement("nt")]
	public NodeType nodeType;
	
	// only used by Set Local/Global Flag + Interrupt Nodes
	[XmlElement("op")]
	public int outPointNID;
	
	// only used by Check Local/Global Flag Node
	[XmlElement("opp")]
	public int outPointPosNID;
	
	// only used by Check Local/Global Flag Node
	[XmlElement("opn")]
	public int outPointNegNID;
	
	// only used by Check/Set Local/Global Flag/Variable Node
	[XmlElement("f")]
	public string flag;
	
	// only used by Set/Check Global Variable Node
	[XmlElement("v")]
	public string variableValue;
	
	[XmlArray("dis")]
	[XmlArrayItem("d")]
	public List<DialogEntry> dialogs;
	
	[XmlArray("des")]
	[XmlArrayItem("d")]
	public List<DecisionEntry> decisions;
	
	public NodeEntry() {
		this.outPointNID = -1;
		this.outPointPosNID = -1;
		this.outPointNegNID = -1;
	}
}

[System.Serializable]
public class DialogEntry {
	[XmlElement("op")]
	public int outPointNID;
	
	[XmlElement("t")]
	public string text;
	
	[XmlArray("fs")]
	[XmlArrayItem("fe")]
	public List<FlagEntry> flags;
	
	public DialogEntry() {
		this.outPointNID = -1;
	}
}

[System.Serializable]
public class DecisionEntry {
	[XmlElement("op")]
	public int outPointNID;
	
	[XmlElement("t")]
	public string text;
	
	public DecisionEntry() {
		this.outPointNID = -1;
	}
}

[System.Serializable]
public class FlagEntry {
	[XmlElement("f")]
	public string flag;
	
	[XmlElement("op")]
	public int outPointNID;
	
	public FlagEntry() {
		this.outPointNID = -1;
	}
}