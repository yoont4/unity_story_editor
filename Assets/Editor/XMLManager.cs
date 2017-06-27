using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public static class XMLManager {
	
	public static int GIDCounter = 0;
	public static StoryNodeEntry storyNode;
	
	public static void SaveItems() {
		storyNode = new StoryNodeEntry();
		List<NodeEntry> nodes = new List<NodeEntry>();
		List<string> flags = new List<string>();
		
		// TEST CODE vvv
		NodeEntry tempNode;
		for (int i = 0; i < 10; i++) {
			tempNode = new NodeEntry();
			tempNode.rect = new RectEntry(i, i, i, i);
			tempNode.widthPad = i;
			tempNode.heightPad = i;
			tempNode.scrollViewOffset = new Vector2(i, i);
			tempNode.parentGID = 0;
			tempNode.nodeType = NodeType.Nothing;
			tempNode.inPointGIDs = null;
			tempNode.outPointGIDs = null;
			tempNode.splitterPositiveGIDs = null;
			tempNode.splitterNegativeGIDs = null;
			tempNode.childContainerGID = 0;
			
			nodes.Add(tempNode);
			flags.Add("test_string" + i);
		}
		
		storyNode.nodes = nodes;
		storyNode.localFlags = flags;
		// TEST CODE ^^^
		
		XmlSerializer serializer = new XmlSerializer(typeof(StoryNodeEntry));
		FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/XML/test.xml", FileMode.Create);
		serializer.Serialize(stream, storyNode);
		stream.Close();
	}
}

// the master story entry
[System.Serializable]
public class StoryNodeEntry {
	public List<string> localFlags;
	public List<NodeEntry> nodes;
}

// keeps track of position, type, and parent/child references
[System.Serializable]
public class SDEComponentEntry {
	public int GID;
	
	public RectEntry rect;
	
	public float widthPad;
	public float heightPad;
	public Vector2 scrollViewOffset;
	
	public int parentGID;
}

[System.Serializable]
public class SDEContainerEntry {
	public int GID;
	
	public SDEContainerType containerType;
	
	public RectEntry rect;
	
	public int parentGID;
	public int childGID;
	public int parentNodeGID;
	
	public List<int> outPointGIDs;
	
	public string text;
}

[System.Serializable]
public class NodeEntry : SDEComponentEntry {
	public NodeType nodeType;
	
	public ConnectionPointEntry inPointGIDs;
	public ConnectionPointEntry outPointGIDs;
	
	public ConnectionPointEntry splitterPositiveGIDs;
	public ConnectionPointEntry splitterNegativeGIDs;
	
	public int childContainerGID;
}

[System.Serializable]
public class ConnectionPointEntry {
	public int GID;
	
	public List<int> connectionGIDs;
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
	public float width;
	public float height;
}
