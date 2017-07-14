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
	
	public static bool SaveXML(string filename) {
		Debug.Log("exporting...");
		
		if (mainEditor == null || mainEditor.nodes == null) {
			Debug.Log("EXPORT ERROR: Story Editor reference unhooked or empty story!");
			return false;
		}
		
		// open the file explorer save window if on a new file
		string path = EditorUtility.SaveFilePanel("Export Story Entry", "Assets", "entry", "xml");
		if (string.IsNullOrEmpty(path)) {
			Debug.Log("canceled save");
			return false;
		}
		
		// --- start building the entry ---
		Dictionary<Node, int> nodeMap = new Dictionary<Node, int>();
		
		// give all the nodes a NID before connecting them
		AssignNIDs(nodeMap);
		
		// build the story entry
		StoryNodeEntry storyEntry = GenerateStoryEntry(nodeMap, mainEditor.nodes);
		
		// write to disk
		XmlSerializer serializer = new XmlSerializer(typeof(StoryNodeEntry));
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		using (StreamWriter stream = new StreamWriter(path, false, encoding)) {
			serializer.Serialize(stream, storyEntry);
		}
		
		// success!
		Debug.Log("exported to: " + path);
		return true;
	}
	
	private static void AssignNIDs(Dictionary<Node, int> nodeMap) {
		// assign every node a Node ID (NID)
		for (int i = 0; i < mainEditor.nodes.Count; i++) {
			
			// interrupt nodes are skipped, because their information is merged into dialog nodes
			if (mainEditor.nodes[i].nodeType != NodeType.Interrupt) {
				nodeMap[mainEditor.nodes[i]] = GenerateNID();
			}
		}
	}
	
	private static int GenerateNID() {
		NIDCounter++;
		return NIDCounter-1;
	}
	
	private static StoryNodeEntry GenerateStoryEntry(Dictionary<Node, int> nodeMap, List<Node> nodes) {
		StoryNodeEntry storyEntry = new StoryNodeEntry();
		
		storyEntry.nodeEntries = GenerateNodeEntries(nodeMap, nodes);
		
		// TODO: implement the rest
		
		return storyEntry;
	}
	
	private static List<NodeEntry> GenerateNodeEntries(Dictionary<Node, int> nodeMap, List<Node> nodes) {
		List<NodeEntry> nodeEntries = new List<NodeEntry>();
		
		NodeEntry tempNodeEntry;
		for (int i = 0; i < nodes.Count; i++) {
			tempNodeEntry = new NodeEntry();
			
			switch (nodes[i].nodeType) {
			case NodeType.Nothing:
				// can't have unassigned Node types!
				throw new UnityException("EXPORT ERROR: cannot have unassigned Nodes when exporting!");
				
			case NodeType.Interrupt:
				// skip interrupt nodes, because they're used to populate Dialog Node Entries
				continue;
				
			case NodeType.Dialog:
				PopulateDialogEntry(tempNodeEntry, nodes[i], nodeMap);
				break;
				
			case NodeType.Decision:
				PopulateDecisionEntry(tempNodeEntry, nodes[i], nodeMap);
				
				break;
				
			case NodeType.SetLocalFlag:
				PopulateSetLocalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				
				break;
				
			case NodeType.CheckLocalFlag:
				PopulateCheckLocalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				tempNodeEntry.nodeType = NodeType.CheckLocalFlag;
				break;
				
			case NodeType.SetGlobalFlag:
				PopulateSetGlobalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				tempNodeEntry.nodeType = NodeType.SetGlobalFlag;
				break;
				
			case NodeType.CheckGlobalFlag:
				PopulateCheckGlobalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				tempNodeEntry.nodeType = NodeType.CheckGlobalFlag;
				break;
				
			default:
				break;
			}
			
			// add the completed NodeEntry
			nodeEntries.Add(tempNodeEntry);
		}
		
		return nodeEntries;
	}
	
	private static void PopulateDialogEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.Dialog;
		nodeEntry.NID = nodeMap[node];
		
		ChildEntry tempChildEntry = new ChildEntry();
		SDEContainer tempChild = node.childContainer;
		if (tempChild != null) {
			nodeEntry.child = tempChildEntry;
			
			while(tempChild != null) {
				tempChildEntry.text = ((DialogBox)tempChild).textArea.text;
				
				// get dialog flags
				tempChildEntry.flags = new List<FlagEntry>();
				Node interruptNode = DialogBoxManager.GetInterruptNode(tempChild.outPoint);
				
				// set the default outpoint if there is one
				if (interruptNode.outPoint != null) {
					tempChildEntry.outPointNID = nodeMap[(Node)interruptNode.outPoint.connections[0].inPoint.parent];
				}
				
				SDEContainer interrupt = interruptNode.childContainer;
				FlagEntry tempFlagEntry;
				while(interrupt != null) {
					// build flag entry
					tempFlagEntry = new FlagEntry();
					tempFlagEntry.flag = ((DialogInterrupt)interrupt).label.text;
					tempFlagEntry.outPointNID = nodeMap[(Node)interrupt.outPoint.connections[0].inPoint.parent];
					
					// add the completed entry to the current dialog entry's flaglist
					tempChildEntry.flags.Add(tempFlagEntry);
					
					// assign to continue traversal
					interrupt = interrupt.child;
				}
				
				// assign to continue traversal
				tempChild = tempChild.child;
				if (tempChild != null) {
					tempChildEntry.child = new ChildEntry();
					tempChildEntry = tempChildEntry.child;
				}
			}
		}
	}
	
	private static void PopulateDecisionEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.Decision;
		nodeEntry.NID = nodeMap[node];
		
	}
	
	private static void PopulateSetLocalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.SetLocalFlag;
		nodeEntry.NID = nodeMap[node];
	}
	
	private static void PopulateCheckLocalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.CheckLocalFlag;
		nodeEntry.NID = nodeMap[node];
	}
	private static void PopulateSetGlobalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.SetGlobalFlag;
		nodeEntry.NID = nodeMap[node];
	}
	private static void PopulateCheckGlobalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		nodeEntry.nodeType = NodeType.CheckGlobalFlag;
		nodeEntry.NID = nodeMap[node];
	}
}
