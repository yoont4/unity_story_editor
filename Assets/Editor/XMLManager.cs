using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;

public static class XMLManager {
	
	private static int CPEIDCounter = 0;
	
	public static StoryDialogEditor mainEditor;
	
	public static StoryNodeEntry storyEntry;
	
	
	public static void SaveItems() {
		if (mainEditor == null) {
			Debug.Log("Cannot Save: Story Editor reference unhooked!");
			return;
		}
		
		storyEntry = new StoryNodeEntry();
		List<NodeEntry> nodes = GenerateNodeEntries(mainEditor.nodes);
		List<string> flags = new List<string>();
		
		// assign nodes/flags
		storyEntry.nodes = nodes;
		storyEntry.localFlags = flags;
		
		// write to disk
		XmlSerializer serializer = new XmlSerializer(typeof(StoryNodeEntry));
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		using (StreamWriter stream = new StreamWriter(Application.dataPath + "/XML/test.xml", false, encoding)) {
			serializer.Serialize(stream, storyEntry);
		}
	}
	
	public static List<NodeEntry> GenerateNodeEntries(List<Node> nodes) {
		// node and connectionpoint maps to help with entry population later
		Dictionary<NodeEntry, Node> nodeMap = new Dictionary<NodeEntry, Node>();
		Dictionary<ConnectionPoint, int> connectionPointMap = new Dictionary<ConnectionPoint, int>();
		
		// start the entries and populate the CPEIDs
		List<NodeEntry> entries = PrepassNodeEntries(nodes, nodeMap, connectionPointMap);
		
		// record the node data into NodeEntries
		Node node;
		foreach(NodeEntry entry in entries) {
			node = nodeMap[entry];
			PopulateNodeEntry(entry, node, connectionPointMap);
		}
		
		return entries;
	}
	
	// goes through everything and assigns CPEIDs to every ConnectionPointEntry and populates container data
	public static List<NodeEntry> PrepassNodeEntries(List<Node> nodes, Dictionary<NodeEntry, Node> nodeMap, Dictionary<ConnectionPoint, int> connectionPointMap) {
		List<NodeEntry> entries = new List<NodeEntry>();
		NodeEntry tempNode;
		for(int i = 0; i < nodes.Count; i++) {
			tempNode = new NodeEntry();
			nodeMap.Add(tempNode, nodes[i]);
			
			tempNode.inPoint = new InPointEntry();
			tempNode.inPoint.CPEID = GenerateCPEID();
			connectionPointMap.Add(nodes[i].inPoint, tempNode.inPoint.CPEID);
			
			// assign CPEID to outpoints and splitter points based on NodeType
			NodeType type = nodes[i].nodeType;
			if ((type == NodeType.Interrupt ||
				type == NodeType.SetGlobalFlag ||
				type == NodeType.SetLocalFlag) &&
				nodes[i].outPoint != null) 
			{
				tempNode.outPoint = new ConnectionPointEntry();
				tempNode.outPoint.CPEID = GenerateCPEID();
				connectionPointMap.Add(nodes[i].outPoint, tempNode.outPoint.CPEID);
			}
			
			if ((type == NodeType.CheckGlobalFlag ||
				type == NodeType.CheckLocalFlag) &&
				nodes[i].splitter != null)
			{
				tempNode.outPos = new ConnectionPointEntry();
				tempNode.outNeg = new ConnectionPointEntry();
				tempNode.outPos.CPEID = GenerateCPEID();
				tempNode.outNeg.CPEID = GenerateCPEID();
				connectionPointMap.Add(nodes[i].splitter.positiveOutpoint, tempNode.outPos.CPEID);
				connectionPointMap.Add(nodes[i].splitter.negativeOutpoint, tempNode.outNeg.CPEID);
			}
			
			// check if it has child elements based on NodeType
			if ((type == NodeType.Interrupt ||
				type == NodeType.Dialog ||
				type == NodeType.Decision) &&
				nodes[i].childContainer != null)
			{
				// traverse through the child components and populate the CPEIDs
				SDEContainer container = nodes[i].childContainer;
				SDEContainerEntry tempContainer = new SDEContainerEntry();
				tempNode.childContainer = tempContainer;
				while (container != null) {
					tempContainer.outPoint = new ConnectionPointEntry();
					tempContainer.outPoint.CPEID = GenerateCPEID();
					connectionPointMap.Add(container.outPoint, tempContainer.outPoint.CPEID);
					
					// get container data
					switch(container.containerType) {
					case SDEContainerType.DecisionBox:
						tempContainer.text = ((DecisionBox)container).textArea.text;
						break;
					case SDEContainerType.DialogBox:
						tempContainer.text = ((DialogBox)container).textArea.text;
						break;
					case SDEContainerType.DialogInterrupt:
						tempContainer.text = ((DialogInterrupt)container).label.text;
						break;
					default:
						throw new UnityException("Malformed SDEContainerType on Save!");
					}
					
					container = container.child;
					
					if (container != null) {
						tempContainer.child = new SDEContainerEntry();
						tempContainer = tempContainer.child;
					}
				}
			}
			
			entries.Add(tempNode);
		}
		
		// reset the CPEID counter after assignments are done
		CPEIDCounter = 0;
		return entries;
	}
	
	public static void PopulateNodeEntry(NodeEntry entry, Node node, Dictionary<ConnectionPoint, int> connectionPointMap) {
		entry.rect = new RectEntry(node.rect.x, node.rect.y, node.rect.width, node.rect.height);
		entry.wPad = node.widthPad;
		entry.hPad = node.heightPad;
		entry.SVOffset = node.scrollViewOffset;
		entry.nodeType = node.nodeType;
		
		// assign inPoint links
		List<int> linkedCPEIDs = new List<int>();
		foreach (Connection connection in node.inPoint.connections) {
			linkedCPEIDs.Add(connectionPointMap[connection.outPoint]);
		}
		entry.inPoint.linkedCPEIDs = linkedCPEIDs;
	}
	
	public static int GenerateCPEID() {
		int temp = CPEIDCounter;
		CPEIDCounter++;
		return temp;
	}
}

// the master story entry
[System.Serializable]
public class StoryNodeEntry {
	public List<string> localFlags;
	public List<NodeEntry> nodes;
}

[System.Serializable]
public class SDEContainerEntry {
	public SDEContainerEntry child;
	
	public ConnectionPointEntry outPoint;
	
	public string text;
}

[System.Serializable]
public class NodeEntry {
	public NodeType nodeType;
	
	public RectEntry rect;
	
	public float wPad;
	public float hPad;
	public Vector2 SVOffset;
	
	public InPointEntry inPoint;
	public ConnectionPointEntry outPoint;
	
	public ConnectionPointEntry outPos;
	public ConnectionPointEntry outNeg;
	
	public SDEContainerEntry childContainer;
}

[System.Serializable]
public class ConnectionPointEntry {
	public int CPEID;
}

// NOTE: only "in" type ConnectionPoints need to save a list of linked ConnectionPoints, because
// they're the only kinds that *should* have multiple entries. Only ConnectionPoint is needed to create
// connections on Load() as well, guaranteeing no duplicate connections when running through all entries.
[System.Serializable]
public class InPointEntry : ConnectionPointEntry {
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
	public float width;
	public float height;
}
