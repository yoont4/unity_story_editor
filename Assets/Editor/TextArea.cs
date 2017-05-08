using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextArea : SDEComponent {
	
	public string text;
	
	// used for keyboard focus controls
	public int textID;
	
	private GUIContent textContent;
	private float contentHeight;
	
	public GUIStyle textAreaStyle;
	public GUIStyle textAreaButtonStyle;
	
	// this links to either another node or the interrupt split
	public ConnectionPoint outPoint;
	
	public TextArea() {}
	
	public void Init(SDEComponent parent, string text, float width) {
		Init(SDEComponentType.TextArea, parent, 
			new Rect(0, 0, width-2*TextAreaManager.X_PAD, 0), 
			TextAreaManager.textBoxDefault, 
			TextAreaManager.textBoxDefault, 
			TextAreaManager.textBoxSelected);
		
		// make the clickRect 4 pixels bigger on each side.
		// clickRect is used to define the TextArea's BG Box
		this.ExtendClickBound(new Vector2(TextAreaManager.X_PAD, TextAreaManager.Y_PAD));
		this.textAreaStyle = TextAreaManager.textAreaStyle;
		this.textAreaButtonStyle = TextAreaManager.textAreaButtonStyle;
		this.text = text; 
		
		this.outPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.outPoint.Init(this, ConnectionPointType.Out);
	}
	
	/*
	  TextArea has a bounding box that borders it by 4px each side.
	  i.e. if a Node is 200px wide, the bounding box would be 200px, and
	  the TextArea itself would be 192px.
	*/
	public override void Draw() {
		if (outPoint != null) {
			outPoint.Draw();
		}
		
		// NOTE: child should always be of type TextArea
		if (child != null) {
			child.Draw();
		}
		
		// GUI.TextArea sucks so I need to draw a box around it and 
		// then make a smaller TextArea inside because .padding breaks
		// the formatting while editing.
		
		textContent = new GUIContent(text);
		contentHeight = textAreaStyle.CalcHeight(textContent, rect.width);
		rect.height = contentHeight;
		clickRect.height = contentHeight + 2*TextAreaManager.Y_PAD;
		
		// calculate position based off of parent Node
		clickRect.x = parent.rect.x - parent.widthPad;
		clickRect.y = parent.rect.y - parent.heightPad + parent.clickRect.height;
		rect.x = clickRect.x + widthPad;
		rect.y = clickRect.y + heightPad;
		
		// don't draw the remove button if its the only child of a Node
		if (child != null || parent.componentType != SDEComponentType.Node) {
			if (GUI.Button(new Rect(clickRect.x-11, clickRect.y + clickRect.height/2 - 6, 12, 12), "-", textAreaButtonStyle)) {
				Remove();
			}
		}
		
		GUI.Box(clickRect, "", style);
		
		// get the Keyboard focusable ControlID of the TextArea before it's drawn
		textID = GUIUtility.GetControlID(FocusType.Keyboard);
		text = GUI.TextArea(rect, text, textAreaStyle);
	}
	
	public override void ProcessEvent(Event e) {
		// process child component events first
		if (outPoint != null) {
			outPoint.ProcessEvent(e);
		}
		
		if (child != null) {
			child.ProcessEvent(e);
		}
		
		base.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle selection
			if (e.button == 0) {
				if (clickRect.Contains(e.mousePosition)) {
					FeatureManager.dragEnabled = false;
				} else {
					GUIUtility.keyboardControl = 0;
				}
			}
			
			if (e.button == 1 && Selected && clickRect.Contains(e.mousePosition)) {
				ProcessContextMenu();
				e.Use();
			}
			break;
			
		case EventType.MouseUp:
			if (e.button == 0) {
				FeatureManager.dragEnabled = true;
			}
			break;
			
		case EventType.ValidateCommand:
			if (ValidateKeyboardCommand(e.commandName)) {
				e.Use();
			}
			break;
			
		case EventType.ExecuteCommand:
			ProcessKeyboardCommand(e.commandName);
			break;
			
		case EventType.KeyDown:
			// check for TextArea cycling
			if (e.keyCode == KeyCode.Tab && Selected) {
				if (child != null) {
					// transfer selection state
					this.Selected = false;
					child.Selected = true;
					
					// pass keyboard control
					GUIUtility.keyboardControl = ((TextArea)child).textID;
					e.Use();
				} else if (parent != null && parent.componentType == SDEComponentType.TextArea) {
					// if at the bottom of the TextArea stack, jump back to the top
					SDEComponent newFocusedText = this;
					while(newFocusedText.parent != null && newFocusedText.parent.componentType == SDEComponentType.TextArea) {
						newFocusedText = newFocusedText.parent;
					}
					
					// transfer selection state
					this.Selected = false;
					newFocusedText.Selected = true;
					
					// pass keyboard control
					GUIUtility.keyboardControl = ((TextArea)newFocusedText).textID;
					e.Use();
				}
			}
			break;
		}
	}
	
	/*
	  ProcessKeyboardInput() listens for modified key code events.
	
	  i.e. ctrl+<key>, ctrl+shift+<key>, etc.
	*/
	private void ProcessKeyboardCommand(string command) {
		switch (command) {
		case "SelectAll":
			TextEditor t = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			t.SelectAll();
			break;
			
		case "Copy":
			Debug.Log("TextArea: 'Copy' command not implemented yet");
			break;
			
		case "Paste":
			Debug.Log("TextArea: 'Paste' command not implemented yet");
			break;
			
		default:
			Debug.Log("no supported keyboard input for this key");
			break;
		}
	}
	
	/*
	  ValidateKeyboardCommand() validates a set number of commands for TextAreas.
	*/
	private bool ValidateKeyboardCommand(string command) {
		switch (command) {
		case "SelectAll":
			return true;
			
		case "Copy":
			return true;
			
		case "Paste":
			return true;
			
		default:
			return false;
		}
	}
	
	/*
	  ProcessContextMenu() creates and hooks up the context menu attached to this Node.
	*/
	private void ProcessContextMenu() {
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Remove TextArea"), false, Remove);
		genericMenu.ShowAsContext();
	}
	
	/*
	  Just a wrapper for the TextAreaManager's RemoveTextArea function, so it can be passed
	  to the ContextMenu's menu function argument.
	*/
	private void Remove() {
		TextAreaManager.RemoveTextArea(this);
	}
}
