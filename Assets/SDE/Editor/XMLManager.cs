using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

/*
  XMLManager is responsible for saving/loading XML data for game engine use.
  
  VALIDATION RULES:
  - All outgoing connections must be connected EXCEPT Set Flag Nodes and Dialog Nodes
*/
public static class XMLManager {
	
	public static StoryDialogEditor mainEditor;
	
	private static int NIDCounter = 0;
	
	public static bool SaveXML(string filename) {
		Debug.Log("exporting...");
		
		if (mainEditor == null || mainEditor.nodes == null || mainEditor.nodes.Count < 1) {
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
		
		try {
			Dictionary<Node, int> nodeMap = new Dictionary<Node, int>();
			
			// give all the nodes a NID before connecting them
			int entryNID = NodePrepass(nodeMap, mainEditor.nodes);
			
			// build the story entry
			StoryNodeEntry storyEntry = GenerateStoryEntry(nodeMap, mainEditor.nodes, entryNID);
			
			// write to disk
			XmlSerializer serializer = new XmlSerializer(typeof(StoryNodeEntry));
			using (StreamWriter stream = new StreamWriter(path, false, Encoding.ASCII)) {
				serializer.Serialize(stream, storyEntry);
			}
		} catch (UnityException e) {
			while (!EditorUtility.DisplayDialog("EXPORT FAILED", e.Message, "I will fix it like a good boi"));
			return false;
		}
		
		// success!
		Debug.Log("exported to: " + path);
		return true;
	}
	
	private static int NodePrepass(Dictionary<Node, int> nodeMap, List<Node> nodes) {
		int entryNID = -1;
		
		// if there is only 1 dialog node, with 1 dialog box (and 1 interrupt), it will always be the entry Node, even if it loops.
		// NOTE: set, check flags should never loop, and Decision nodes should never loop alone, so only Dialog 
		// Nodes should be able to be alone and loop.
		bool assigned = false;
		if (nodes.Count == 2) {
			// validate that the only 2 nodes are a Dialog and Interrupt Node
			if (nodes[0].nodeType == NodeType.Dialog && nodes[1].nodeType == NodeType.Interrupt) {
				nodeMap[nodes[0]] = GenerateNID();
				entryNID = nodeMap[nodes[0]];
				assigned = true;
			} else if (nodes[0].nodeType == NodeType.Interrupt && nodes[1].nodeType == NodeType.Dialog) {
				nodeMap[nodes[1]] = GenerateNID();
				entryNID = nodeMap[nodes[1]];
				assigned = true;
			}
		} 
		
		if (!assigned) {
			// used to check if there was a valid entry Node
			bool entryNIDFound = false;
			
			// assign every node a Node ID (NID)
			for (int i = 0; i < nodes.Count; i++) {
				
				// interrupt nodes are skipped, because their information is merged into dialog nodes
				if (nodes[i].nodeType != NodeType.Interrupt) {
					nodeMap[nodes[i]] = GenerateNID();
					
					// look for potential start Node
					if (nodes[i].inPoint.connections.Count < 1) {
						if (entryNID != -1) {
							throw new UnityException("EXPORT ERROR: There are multiple potential starting Nodes!");
						}
						entryNID = nodeMap[nodes[i]];
						entryNIDFound = true;
					}
				}
			}
			
			if (!entryNIDFound) {
				throw new UnityException("EXPORT ERROR: No valid starting Nodes!");
			}
		}
		
		
		return entryNID;
	}
	
	private static int GenerateNID() {
		NIDCounter++;
		return NIDCounter-1;
	}
	
	private static StoryNodeEntry GenerateStoryEntry(Dictionary<Node, int> nodeMap, List<Node> nodes, int entryNID) {
		StoryNodeEntry storyEntry = new StoryNodeEntry();
		
		storyEntry.nodeEntries = GenerateNodeEntries(nodeMap, nodes);
		storyEntry.entryNID = entryNID;
		
		List<string> flags = new List<string>();
		for (int i = 0; i < mainEditor.localFlagsMenu.items.Count; i++) {
			flags.Add(mainEditor.localFlagsMenu.items[i].text);
		}
		
		storyEntry.localFlags = flags;
		
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
				break;
				
			case NodeType.SetGlobalFlag:
				PopulateSetGlobalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				break;
				
			case NodeType.CheckGlobalFlag:
				PopulateCheckGlobalFlagEntry(tempNodeEntry, nodes[i], nodeMap);
				break;
				
			case NodeType.SetGlobalVariable:
				PopulateSetGlobalVariableEntry(tempNodeEntry, nodes[i], nodeMap);
				break;
				
			case NodeType.CheckGlobalVariable:
				PopulateCheckGlobalVariableEntry(tempNodeEntry, nodes[i], nodeMap);
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
		ValidateDialog(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.Dialog;
		nodeEntry.NID = nodeMap[node];
		
		nodeEntry.dialogs = new List<DialogEntry>();
		
		DialogEntry tempDialogEntry = new DialogEntry();
		SDEContainer tempChild = node.childContainer;
		if (tempChild != null) {
			nodeEntry.dialogs.Add(tempDialogEntry);
			
			while(tempChild != null) {
				// validate every DialogBox
				ValidateDialogBox((DialogBox)tempChild);
				
				// assign values if validated
				tempDialogEntry.text = ((DialogBox)tempChild).textArea.text;
				
				// get dialog flags
				tempDialogEntry.flags = new List<FlagEntry>();
				Node interruptNode = DialogBoxManager.GetInterruptNode(tempChild.outPoint);
				
				if (interruptNode == null) {
					throw new UnityException("EXPORT ERROR: Dialog Node is missing attached Interrupt Node!");
				}
				
				ValidateInterrupt(interruptNode);
				
				// set the default outpoint if there is one and if it's connected
				if (interruptNode.outPoint != null && interruptNode.outPoint.connections.Count > 0) {
					tempDialogEntry.outPointNID = nodeMap[(Node)interruptNode.outPoint.connections[0].inPoint.parent];
				}
				
				SDEContainer interrupt = interruptNode.childContainer;
				FlagEntry tempFlagEntry;
				while(interrupt != null) {
					ValidateDialogInterrupt((DialogInterrupt)interrupt);
					
					// build flag entry
					tempFlagEntry = new FlagEntry();
					tempFlagEntry.flag = ((DialogInterrupt)interrupt).label.text;
					tempFlagEntry.outPointNID = nodeMap[(Node)interrupt.outPoint.connections[0].inPoint.parent];
					
					// add the completed entry to the current dialog entry's flaglist
					tempDialogEntry.flags.Add(tempFlagEntry);
					
					// assign to continue traversal
					interrupt = interrupt.child;
				}
				
				// assign to continue traversal
				tempChild = tempChild.child;
				if (tempChild != null) {
					tempDialogEntry = new DialogEntry();
					nodeEntry.dialogs.Add(tempDialogEntry);
				}
			}
		}
	}
	
	private static void ValidateDialog(Node node) {
		if (node.childContainer == null) {
			throw new UnityException("EXPORT ERROR: DialogNode missing child!");
		}
	}
	
	private static void ValidateDialogBox(DialogBox dialog) {
		if (string.IsNullOrEmpty(dialog.textArea.text)) {
			throw new UnityException("EXPORT ERROR: DialogBox missing text!");
		}
		
		if (DialogBoxManager.GetInterruptNode(dialog.outPoint) == null) {
			throw new UnityException("EXPORT ERROR: DialogBox missing interrupt flag connection!");
		}
	}
	
	private static void ValidateInterrupt(Node node) {
		if (node.childContainer == null && node.outPoint == null) {
			throw new UnityException("EXPORT ERROR: InterruptNode has no path through! (Non bottom-level DialogBox is missing flags)");
		}
	}
	
	private static void ValidateDialogInterrupt(DialogInterrupt interrupt) {
		if (string.IsNullOrEmpty(interrupt.label.text)) {
			throw new UnityException("EXPORT ERROR: DialogInterrupt missing flag text!");
		}
		
		if (interrupt.outPoint.connections.Count < 1) {
			throw new UnityException("EXPORT ERROR: DialogInterrupt missing out-connections!");
		}
	}
	
	private static void PopulateDecisionEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateDecision(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.Decision;
		nodeEntry.NID = nodeMap[node];
		
		nodeEntry.decisions = new List<DecisionEntry>();
		
		DecisionEntry tempDecisionEntry = new DecisionEntry();
		SDEContainer tempChild = node.childContainer;
		if (tempChild != null) {
			nodeEntry.decisions.Add(tempDecisionEntry);
			
			while(tempChild != null) {
				// validate each DecisionBox
				ValidateDecisionBox((DecisionBox)tempChild);
				
				// assign child entry data if validated
				tempDecisionEntry.text = ((DecisionBox)tempChild).textArea.text;
				tempDecisionEntry.outPointNID = nodeMap[(Node)tempChild.outPoint.connections[0].inPoint.parent];
				
				tempChild = tempChild.child;
				if (tempChild != null) {
					tempDecisionEntry = new DecisionEntry();
					nodeEntry.decisions.Add(tempDecisionEntry);
				}
			}
		}
	}
	
	private static void ValidateDecision(Node node) {
		if (node.childContainer == null) {
			throw new UnityException("EXPORT ERROR: DecisionNode missing child!");
		}
	}
	
	private static void ValidateDecisionBox(DecisionBox decision) {
		if (decision.outPoint.connections.Count < 1) {
			throw new UnityException("EXPORT ERROR: DecisionBox missing out-connection!");
		}
		
		if (string.IsNullOrEmpty(decision.textArea.text)) {
			throw new UnityException("EXPORT ERROR: DecisionBox missing text!");
		}
	}
	
	private static void PopulateSetLocalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateSetLocalFlag(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.SetLocalFlag;
		nodeEntry.NID = nodeMap[node];
		
		// assign outpoint if there is one
		if (node.outPoint.connections.Count > 0) {
			nodeEntry.outPointNID = nodeMap[(Node)node.outPoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.localFlagDropdown.selectedItem.text;
	}
	
	private static void ValidateSetLocalFlag(Node node) {
		if (node.localFlagDropdown.selectedItem == null) {
			throw new UnityException("EXPORT ERROR: LocalFlagNode missing selected flag!");
		}
	}
	
	private static void PopulateCheckLocalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateCheckLocalFlag(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.CheckLocalFlag;
		nodeEntry.NID = nodeMap[node];
		
		// assign connected outpoints
		if (node.splitter.positiveOutpoint.connections.Count > 0) {
			nodeEntry.outPointPosNID = nodeMap[(Node)node.splitter.positiveOutpoint.connections[0].inPoint.parent];
		}
		if (node.splitter.negativeOutpoint.connections.Count > 0) {
			nodeEntry.outPointNegNID = nodeMap[(Node)node.splitter.negativeOutpoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.localFlagDropdown.selectedItem.text;
	}
	
	private static void ValidateCheckLocalFlag(Node node) {
		if (node.splitter.positiveOutpoint.connections.Count < 1 || node.splitter.negativeOutpoint.connections.Count < 1) {	
			throw new UnityException("EXPORT ERROR: LocalFlagNode must have at least 1 out-connection!");
		} 
		
		if (node.localFlagDropdown.selectedItem == null) {
			throw new UnityException("EXPORT ERROR: LocalFlagNode missing selected flag!");
		}
	}
	
	private static void PopulateSetGlobalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateSetGlobalFlag(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.SetGlobalFlag;
		nodeEntry.NID = nodeMap[node];
		
		// assign outpoint if there is one
		if (node.outPoint.connections.Count > 0) {
			nodeEntry.outPointNID = nodeMap[(Node)node.outPoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.globalItemDropdown.selectedItem;
	}
	
	private static void ValidateSetGlobalFlag(Node node) {
		if (string.IsNullOrEmpty(node.globalItemDropdown.selectedItem)) {
			throw new UnityException("EXPORT ERROR: GlobalFlagNode missing selected flag!");
		}
	}
	
	private static void PopulateCheckGlobalFlagEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateCheckGlobalFlag(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.CheckGlobalFlag;
		nodeEntry.NID = nodeMap[node];
		
		// assign connected outpoints
		if (node.splitter.positiveOutpoint.connections.Count > 0) {
			nodeEntry.outPointPosNID = nodeMap[(Node)node.splitter.positiveOutpoint.connections[0].inPoint.parent];
		}
		if (node.splitter.negativeOutpoint.connections.Count > 0) {
			nodeEntry.outPointNegNID = nodeMap[(Node)node.splitter.negativeOutpoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.globalItemDropdown.selectedItem;
	}
	
	private static void ValidateCheckGlobalFlag(Node node) {
		if (node.splitter.positiveOutpoint.connections.Count < 1 && node.splitter.negativeOutpoint.connections.Count < 1) {
			throw new UnityException("EXPORT ERROR: GlobalFlagNode must have at least 1 out-connection!");
		}
		
		if (string.IsNullOrEmpty(node.globalItemDropdown.selectedItem)) {
			throw new UnityException("EXPORT ERROR: GlobalFlagNode missing selected flag!");
		}
	}
	
	private static void PopulateSetGlobalVariableEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateSetGlobalVariable(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.SetGlobalVariable;
		nodeEntry.NID = nodeMap[node];
		
		// assign outpoint if there is one
		if (node.outPoint.connections.Count > 0) {
			nodeEntry.outPointNID = nodeMap[(Node)node.outPoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.globalItemDropdown.selectedItem;
		if (string.IsNullOrEmpty(node.globalVariableField.text)) {
			nodeEntry.variableValue = "+0";
		} else {
			nodeEntry.variableValue = node.globalVariableField.text;
		}
	}
	
	private static void ValidateSetGlobalVariable(Node node) {
		if (string.IsNullOrEmpty(node.globalItemDropdown.selectedItem)) {
			throw new UnityException("EXPORT ERROR: GlobalVariableNode missing selected flag!");
		}
		
		string regPattern = "^[+-=]?\\d{1,}$";
		if (!Regex.IsMatch(node.globalVariableField.text, regPattern)) {
			throw new UnityException("EXPORT ERROR: GlobalVariableNode has invalid variable value!");
		}
	}
	
	private static void PopulateCheckGlobalVariableEntry(NodeEntry nodeEntry, Node node, Dictionary<Node, int> nodeMap) {
		ValidateCheckGlobalVariable(node);
		
		// assign node entry data if validated
		nodeEntry.nodeType = NodeType.CheckGlobalVariable;
		nodeEntry.NID = nodeMap[node];
		
		// assign connected outpoints
		if (node.splitter.positiveOutpoint.connections.Count > 0) {
			nodeEntry.outPointPosNID = nodeMap[(Node)node.splitter.positiveOutpoint.connections[0].inPoint.parent];
		}
		if (node.splitter.negativeOutpoint.connections.Count > 0) {
			nodeEntry.outPointNegNID = nodeMap[(Node)node.splitter.negativeOutpoint.connections[0].inPoint.parent];
		}
		
		nodeEntry.flag = node.globalItemDropdown.selectedItem;
		if (string.IsNullOrEmpty(node.globalVariableField.text)) {
			nodeEntry.variableValue = "0";
		} else {
			nodeEntry.variableValue = node.globalVariableField.text;
		}
	}
	
	private static void ValidateCheckGlobalVariable(Node node) {
		if (node.splitter.positiveOutpoint.connections.Count < 1 && node.splitter.negativeOutpoint.connections.Count < 1) {
			throw new UnityException("EXPORT ERROR: GlobalVariableNode must have at least 1 out-connection!");
		}
		
		if (string.IsNullOrEmpty(node.globalItemDropdown.selectedItem)) {
			throw new UnityException("EXPORT ERROR: GlobalVariableNode missing selected variable!");
		}
		
		string regPattern = "^\\d{1,}$";
		if (!Regex.IsMatch(node.globalVariableField.text, regPattern)) {
			throw new UnityException("EXPORT ERROR: GlobalVariableNode has invalid variable value!");
		}
	}
}
