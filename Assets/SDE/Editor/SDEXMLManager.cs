using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

/*
  SDEXMLManager is responsible for saving/loading XML data for the SDE.
*/
public static class SDEXMLManager {
	
	private static int CPEIDCounter = 0;
	
	public static StoryDialogEditor mainEditor;
	
	// vvvvv LOAD STORY EDITOR vvvvv //
	
	public static void LoadItems(string path) {
		if (mainEditor == null) {
			// open a new window if it's unhooked
			Debug.Log("Window not found, opening new Story Dialog Editor window.");
			StoryDialogEditor.OpenWindow();
		} else if (HistoryManager.needsSave) {
			// create dialog entry to warn user that they have unsaved changes
			if (!EditorUtility.DisplayDialog("Load new entry", "Are you sure you want to open a new entry and close the current one?", "yes", "no")) {
				return;
			}
		}
		
		Debug.Log("attempting to load: " + path);
		
		EditorStoryNodeEntry storyEntry = new EditorStoryNodeEntry();
		
		XmlSerializer serializer = new XmlSerializer(typeof(EditorStoryNodeEntry));
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		using (StreamReader stream = new StreamReader(path, encoding)) {
			storyEntry = serializer.Deserialize(stream) as EditorStoryNodeEntry;
		}
		
		// destroy the scene before populating it
		mainEditor.DestroyScene();
		
		// set the editor's offset to match the saved entry
		mainEditor.offset = storyEntry.offset;
		
		// load flags
		foreach (string entry in storyEntry.localFlags) {
			mainEditor.localFlagsMenu.AddItem(entry, markHistory:false);
		}
		
		// CPEID maps to ConnectionPoint to generate connections later
		Dictionary<int, ConnectionPoint> connectionPointMap = new Dictionary<int, ConnectionPoint>();
		
		// initialize all the nodes and generate the connection map
		Dictionary<int, List<int>> connectionMap = InitializeNodes(storyEntry, connectionPointMap);
		
		// create all the connections
		foreach (int inCPEID in connectionMap.Keys) {
			ConnectionManager.selectedInPoint = connectionPointMap[inCPEID];
			
			bool deletable = true;
			if (((Node)ConnectionManager.selectedInPoint.parent).nodeType == NodeType.Interrupt) {
				deletable = false;
			}
			foreach (int outCPEID in connectionMap[inCPEID]) {
				ConnectionManager.selectedOutPoint = connectionPointMap[outCPEID];
				ConnectionManager.CreateConnection(deletable, markHistory:false);
			}
		}
		ConnectionManager.ClearConnectionSelection();
		
		Debug.Log("loaded: " + path);
		mainEditor.fileName = path;
	}
	
	/*
	  InitializeNodes() is a helper function for loading Story Dialog Editor data.
	
	  Returns the mapping of input connection points and their associated output connection points
	*/
	private static Dictionary<int, List<int>> InitializeNodes(EditorStoryNodeEntry storyEntry, Dictionary<int, ConnectionPoint> connectionPointMap) {
		// the map of input points and their associated output points
		Dictionary<int, List<int>> connectionMap = new Dictionary<int, List<int>>();
		
		// generate nodes and data from entries
		Node tempNode;
		foreach (EditorNodeEntry entry in storyEntry.nodes) {
			tempNode = NodeManager.AddNodeAt(new Vector2(entry.rect.x, entry.rect.y), entry.nodeType, markHistory:false, center:false);
			connectionPointMap[entry.inPoint.CPEID] = tempNode.inPoint;
			connectionMap[entry.inPoint.CPEID] = entry.inPoint.linkedCPEIDs;
			
			// add flag data if set local/global
			if (entry.nodeType == NodeType.SetLocalFlag || entry.nodeType == NodeType.CheckLocalFlag) {
				tempNode.localFlagDropdown.selectedItem = mainEditor.localFlagsMenu.GetTextArea(entry.selectedFlag);
			} else if (entry.nodeType == NodeType.SetGlobalFlag || entry.nodeType == NodeType.CheckGlobalFlag) {
				tempNode.globalItemDropdown.selectedItem = entry.selectedFlag;
			} else if (entry.nodeType == NodeType.SetGlobalVariable || entry.nodeType == NodeType.CheckGlobalVariable) {
				tempNode.globalItemDropdown.selectedItem = entry.selectedFlag;
				tempNode.globalVariableField.text = entry.globalVariableValue;
			}
			
			// map Node outpoint/splitter depending on NodeType
			if (entry.nodeType == NodeType.SetLocalFlag || 
				entry.nodeType == NodeType.SetGlobalFlag || 
				entry.nodeType == NodeType.SetGlobalVariable || 
				entry.nodeType == NodeType.Interrupt)
			{
				// add outpoint entry if available
				if (tempNode.outPoint != null && entry.outPoint != null) {
					connectionPointMap[entry.outPoint.CPEID] = tempNode.outPoint;
				}
			} else if (entry.nodeType == NodeType.CheckLocalFlag || 
				entry.nodeType == NodeType.CheckGlobalFlag || 
				entry.nodeType == NodeType.CheckGlobalVariable)
			{
				// add splitter entries if available
				if (tempNode.splitter != null && entry.outPos != null && entry.outNeg != null) {
					connectionPointMap[entry.outPos.CPEID] = tempNode.splitter.positiveOutpoint;
					connectionPointMap[entry.outNeg.CPEID] = tempNode.splitter.negativeOutpoint;
				}
			}
			
			// record child container outpoints depending on NodeType and populate fields
			if (entry.nodeType == NodeType.Dialog || entry.nodeType == NodeType.Decision) {
				DBox child = tempNode.childContainer as DBox;
				SDEContainerEntry childEntry = entry.childContainer;
				
				while (childEntry != null) {
					// set text and outpoint mapping for the child container
					child.textArea.text = childEntry.text;
					connectionPointMap[childEntry.outPoint.CPEID] = child.outPoint;
					
					childEntry = childEntry.child;
					
					// generate the child's child if there needs to be one
					if (childEntry != null) {
						switch (entry.nodeType) {
						case NodeType.Dialog:
							child.child = ScriptableObject.CreateInstance<DialogBox>();
							((DialogBox)child.child).Init(child, "");
							break;
						case NodeType.Decision:
							child.child = ScriptableObject.CreateInstance<DecisionBox>();
							((DecisionBox)child.child).Init(child, "");
							break;
						}
					}
					
					child = child.child as DBox;
				}
			} else if (entry.nodeType == NodeType.Interrupt) {
				tempNode.SetBottomLevelInterrupt(entry.bottomLevel);
				
				DialogInterrupt child;
				SDEContainerEntry childEntry = entry.childContainer;
				
				if (childEntry != null) {
					// create the first child of the parent node
					tempNode.childContainer = ScriptableObject.CreateInstance<DialogInterrupt>();
					tempNode.childContainer.Init(tempNode);
					((DialogInterrupt)tempNode.childContainer).label.text = childEntry.text;
					
					// record the connection point
					connectionPointMap[childEntry.outPoint.CPEID] = tempNode.childContainer.outPoint;
					
					child = tempNode.childContainer as DialogInterrupt;
					childEntry = childEntry.child;
					
					while (childEntry != null) {
						// generate the child interrupt and populate it
						child.child = ScriptableObject.CreateInstance<DialogInterrupt>();
						((DialogInterrupt)child.child).Init(child);
						((DialogInterrupt)child.child).label.text = childEntry.text;
						
						// record the connection point
						connectionPointMap[childEntry.outPoint.CPEID] = child.child.outPoint;
						
						child = child.child as DialogInterrupt;
						childEntry = childEntry.child;
					}
				}
			}
		}
		
		return connectionMap;
	}
	
	// ^^^^^ LOAD STORY EDITOR ^^^^^ //
	
	// vvvvv SAVE STORY EDITOR vvvvv //
	
	// returns if it completed the save or not
	public static bool SaveItems(bool saveAs) {
		// use event regardless of outcome to prevent unexpected event pass-through
		Event.current.Use();
		
		if (mainEditor == null) {
			Debug.Log("Cannot Save: Story Editor reference unhooked!");
			return false;
		}
		
		// open the file explorer save window if on a new file
		// otherwise, save to the current file
		string path;
		if (saveAs || (!saveAs && string.IsNullOrEmpty(mainEditor.fileName))) {
			path = EditorUtility.SaveFilePanel("Save Story Entry", ProjectPathManager.LastExportPath, "entry", "sdexml");
			mainEditor.fileName = path;
		} else {
			path = mainEditor.fileName;
		}
		
		if (string.IsNullOrEmpty(path)) {
			Debug.Log("canceled save");
			return false;
		}
		
		EditorStoryNodeEntry storyEntry = new EditorStoryNodeEntry();
		List<EditorNodeEntry> nodes = GenerateNodeEntries(mainEditor.nodes);
		List<string> flags = new List<string>();
		foreach (TextArea flag in mainEditor.localFlagsMenu.items) {
			flags.Add(flag.text);
		}
		
		// assign nodes/flags
		storyEntry.nodes = nodes;
		storyEntry.localFlags = flags;
		storyEntry.offset = mainEditor.offset;
		
		// write to disk
		XmlSerializer serializer = new XmlSerializer(typeof(EditorStoryNodeEntry));
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		using (StreamWriter stream = new StreamWriter(path, false, encoding)) {
			serializer.Serialize(stream, storyEntry);
		}
		
		ProjectPathManager.LastExportPath = path;
		return true;
	}
	
	public static List<EditorNodeEntry> GenerateNodeEntries(List<Node> nodes) {
		// node and connectionpoint maps to help with entry population later
		Dictionary<EditorNodeEntry, Node> nodeMap = new Dictionary<EditorNodeEntry, Node>();
		Dictionary<ConnectionPoint, int> connectionPointMap = new Dictionary<ConnectionPoint, int>();
		
		// start the entries and populate the CPEIDs
		List<EditorNodeEntry> entries = PrepassNodeEntries(nodes, nodeMap, connectionPointMap);
		
		// record the node data into NodeEntries
		Node node;
		foreach(EditorNodeEntry entry in entries) {
			node = nodeMap[entry];
			PopulateNodeEntry(entry, node, connectionPointMap);
		}
		
		return entries;
	}
	
	// goes through everything and assigns CPEIDs to every ConnectionPointEntry and populates container data
	public static List<EditorNodeEntry> PrepassNodeEntries(List<Node> nodes, Dictionary<EditorNodeEntry, Node> nodeMap, Dictionary<ConnectionPoint, int> connectionPointMap) {
		List<EditorNodeEntry> entries = new List<EditorNodeEntry>();
		EditorNodeEntry tempNode;
		for(int i = 0; i < nodes.Count; i++) {
			tempNode = new EditorNodeEntry();
			nodeMap[tempNode] = nodes[i];
			
			tempNode.inPoint = new InPointEntry();
			tempNode.inPoint.CPEID = GenerateCPEID();
			connectionPointMap[nodes[i].inPoint] = tempNode.inPoint.CPEID;
			
			// assign CPEID to outpoints and splitter points based on NodeType
			NodeType type = nodes[i].nodeType;
			if ((type == NodeType.Interrupt ||
				type == NodeType.SetGlobalFlag ||
				type == NodeType.SetLocalFlag ||
				type == NodeType.SetGlobalVariable) &&
				nodes[i].outPoint != null) 
			{
				tempNode.outPoint = new ConnectionPointEntry();
				tempNode.outPoint.CPEID = GenerateCPEID();
				connectionPointMap[nodes[i].outPoint] = tempNode.outPoint.CPEID;
			}
			
			if ((type == NodeType.CheckGlobalFlag ||
				type == NodeType.CheckLocalFlag ||
				type == NodeType.CheckGlobalVariable) &&
				nodes[i].splitter != null)
			{
				tempNode.outPos = new ConnectionPointEntry();
				tempNode.outNeg = new ConnectionPointEntry();
				tempNode.outPos.CPEID = GenerateCPEID();
				tempNode.outNeg.CPEID = GenerateCPEID();
				connectionPointMap[nodes[i].splitter.positiveOutpoint] = tempNode.outPos.CPEID;
				connectionPointMap[nodes[i].splitter.negativeOutpoint] = tempNode.outNeg.CPEID;
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
					connectionPointMap[container.outPoint] = tempContainer.outPoint.CPEID;
					
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
	
	public static void PopulateNodeEntry(EditorNodeEntry entry, Node node, Dictionary<ConnectionPoint, int> connectionPointMap) {
		entry.rect = new RectEntry(node.rect.x, node.rect.y, node.rect.width, node.rect.height);
		entry.wPad = node.widthPad;
		entry.hPad = node.heightPad;
		entry.SVOffset = node.scrollViewOffset;
		entry.nodeType = node.nodeType;
		entry.bottomLevel = node.bottomLevel;
		
		if (entry.nodeType == NodeType.SetLocalFlag || entry.nodeType == NodeType.CheckLocalFlag){
			// populate the selected item of the local flag entry
			if (node.localFlagDropdown.selectedItem != null) {
				entry.selectedFlag = node.localFlagDropdown.selectedItem.text;
			}
		} else if (entry.nodeType == NodeType.SetGlobalFlag || entry.nodeType == NodeType.CheckGlobalFlag) {
			// populate the selected item of the global flag entry
			entry.selectedFlag = node.globalItemDropdown.selectedItem;
		} else if (entry.nodeType == NodeType.SetGlobalVariable || entry.nodeType == NodeType.CheckGlobalVariable) {
			// populate the selected item and the global variable value of the global variable entry
			entry.selectedFlag = node.globalItemDropdown.selectedItem;
			entry.globalVariableValue = node.globalVariableField.text;
		}
		
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
	
	// ^^^^^ SAVE STORY EDITOR ^^^^^ //
	
	// vvvvv SDEXML UTILIY vvvvv //
	
	public static void OnProjectItemGUI(string item, Rect selectionRect) {
		if (!IsValidOnProjectItemGUIEvent(item, selectionRect)) {
			return;
		}
		
		// get selected item path
		string path = AssetDatabase.GUIDToAssetPath(item);
		if (!IsValidFile(path)) {
			return;
		}
		
		// if it's a valid item, use the event
		Event.current.Use();
		LoadItems(path);
	}
	
	// IsValidOnProjectItemGUIEvent() is a helper function to cull useless update calls from being
	// processed by OnProjectItemGUI().
	private static bool IsValidOnProjectItemGUIEvent(string item, Rect selectionRect) {
		if (string.IsNullOrEmpty(item)) {
			return false;
		}
		
		if (Event.current.isMouse) {
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2 || Event.current.button != 0) {
				return false;
			}
			
			if (selectionRect.height < 20f) {
				selectionRect.xMin = 0f;
			}
			
			if (!selectionRect.Contains(Event.current.mousePosition)) {
				return false;
			}
		} else {
			return false;
		}
		
		return true;
	}
	
	private static bool IsValidFile(string path) {
		if (path.EndsWith(".sdexml", System.StringComparison.OrdinalIgnoreCase)) {
			return true;
		} else {
			return false;
		}
	}
	
	// ^^^^^ SDEXML UTILIY ^^^^^ //
}
