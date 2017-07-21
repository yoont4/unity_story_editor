using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

// the master story entry
[System.Serializable]
public class EditorStoryNodeEntry {
	[XmlArray("lfs")]
	[XmlArrayItem("lf")]
	public List<string> localFlags;
	
	[XmlElement("n")]
	public List<EditorNodeEntry> nodes;
	
	[XmlElement("o")]
	public Vector2 offset;
}

[System.Serializable]
public class SDEContainerEntry {
	[XmlElement("c")]
	public SDEContainerEntry child;
	
	[XmlElement("op")]
	public ConnectionPointEntry outPoint;
	
	[XmlElement("t")]
	public string text;
}

[System.Serializable]
public class EditorNodeEntry {
	[XmlElement("nt")]
	public NodeType nodeType;
	
	[XmlElement("r")]
	public RectEntry rect;
	
	[XmlElement("wp")]
	public float wPad;
	[XmlElement("hp")]
	public float hPad;
	[XmlElement("sv")]
	public Vector2 SVOffset;
	
	[XmlElement("bl")]
	public bool bottomLevel;
	
	[XmlElement("ip")]
	public InPointEntry inPoint;
	[XmlElement("op")]
	public ConnectionPointEntry outPoint;
	
	[XmlElement("opp")]
	public ConnectionPointEntry outPos;
	[XmlElement("opn")]
	public ConnectionPointEntry outNeg;
	
	[XmlElement("cc")]
	public SDEContainerEntry childContainer;
	
	[XmlElement("sf")]
	public string selectedFlag;
	
	[XmlElement("gv")]
	public string globalVariableValue;
}

[System.Serializable]
public class ConnectionPointEntry {
	[XmlElement("id")]
	public int CPEID;
}

// NOTE: only "in" type ConnectionPoints need to save a list of linked ConnectionPoints, because
// they're the only kinds that *should* have multiple entries. Only ConnectionPoint is needed to create
// connections on Load() as well, guaranteeing no duplicate connections when running through all entries.
[System.Serializable]
public class InPointEntry : ConnectionPointEntry {
	[XmlArray("ids")]
	[XmlArrayItem("id")]
	public List<int> linkedCPEIDs;
}

[System.Serializable]
public class RectEntry {
	public RectEntry() {}
	public RectEntry(float x, float y, float width, float height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}
	
	public float x;
	public float y;
	
	[XmlElement("w")]
	public float width;
	[XmlElement("h")]
	public float height;
}