using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextArea : SDEComponent {
	
	public string text;
	
	// used for keyboard focus controls
	public int textID;
	
	public int maxLength = -1;
	
	private GUIContent textContent;
	private float contentHeight;
	
	public GUIStyle textAreaStyle;
	public GUIStyle textButtonStyle;
	
	public TextArea() {}
	
	public void Init(SDEComponent parent, string text, float width) {
		Init(SDEComponentType.TextArea, parent, 
			new Rect(0, 0, width-2*TextAreaManager.X_PAD, 0), 
			SDEStyles.textBoxDefault,
			SDEStyles.textBoxDefault, 
			SDEStyles.textBoxSelected);
		
		Init(text);
	}
	
	public void Init(SDEContainer container, string text, float width) {
		Init(SDEComponentType.TextArea, null, 
			new Rect(0, 0, width-2*TextAreaManager.X_PAD, 0), 
			SDEStyles.textBoxDefault, 
			SDEStyles.textBoxDefault, 
			SDEStyles.textBoxSelected,
			container);
		
		Init(text);
	}
	
	public void Init(string text, float width, float x, float y) {
		Init(SDEComponentType.TextArea, null, 
			new Rect(0, 0, width-2*TextAreaManager.X_PAD, 0), 
			SDEStyles.textBoxDefault, 
			SDEStyles.textBoxDefault, 
			SDEStyles.textBoxSelected);
		
		Init(text);
		
		clickRect.x = x;
		clickRect.y = y;
		rect.x = x + widthPad;
		rect.y = y + heightPad;
	}
	
	private void Init(string text) {
		// make the clickRect 4 pixels bigger on each side.
		// clickRect is used to define the TextArea's BG Box
		this.ExtendClickBound(new Vector2(TextAreaManager.X_PAD, TextAreaManager.Y_PAD));
		this.textAreaStyle = SDEStyles.textAreaDefault;
		this.textButtonStyle = SDEStyles.textButtonDefault;
		this.text = text; 
	}
	
	/*
	  TextArea has a bounding box that borders it by 4px each side.
	  i.e. if a Node is 200px wide, the bounding box would be 200px, and
	  the TextArea itself would be 192px.
	*/
	public override void Draw() {
		// GUI.TextArea sucks so I need to draw a box around it and 
		// then make a smaller TextArea inside because .padding breaks
		// the formatting while editing.
		
		textContent = new GUIContent(text);
		contentHeight = textAreaStyle.CalcHeight(textContent, rect.width);
		rect.height = contentHeight;
		clickRect.height = contentHeight + 2*TextAreaManager.Y_PAD;
		
		if (container != null) {
			// calculate position based off of container
			// NOTE: do not need to account for parent/container size, because the
			// container manages the offset relative to other containers
			clickRect.x = container.rect.x;
			clickRect.y = container.rect.y;
			rect.x = clickRect.x + widthPad;
			rect.y = clickRect.y + heightPad;
		} else if (parent != null) {
			// TODO: figure out if this can be removed (all TextAreas are currently
			// bound to a container, not another Component).
			// calculate position based off of parent Node
			clickRect.x = parent.rect.x - parent.widthPad;
			clickRect.y = parent.rect.y - parent.heightPad + parent.clickRect.height;
			rect.x = clickRect.x + widthPad;
			rect.y = clickRect.y + heightPad;
		} else {
			// TOOD: figure out if we need this or not. What if we just want a TextArea that floats?
			//throw new UnityException("Tried to draw TextArea without container or parent!");
		}
		
		GUI.Box(clickRect, "", style);
		
		// get the Keyboard focusable ControlID of the TextArea before it's drawn
		textID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
		
		if (maxLength > 0) {
			text = GUI.TextArea(rect, text, maxLength, textAreaStyle);
		} else {
			text = GUI.TextArea(rect, text, textAreaStyle);
		}
	}
	
	public override void ProcessEvent(Event e) {
		base.ProcessEvent(e);
		
		switch(e.type) {
		case EventType.MouseDown:
			// handle selection
			if (e.button == 0) {
				if (Contains(e.mousePosition)) {
					FeatureManager.dragEnabled = false;
				} else {
					GUIUtility.keyboardControl = 0;
				}
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
			// record key presses in history (if selected)
			if (Selected && e.keyCode == KeyCode.None && e.character != '\t') {
				HistoryManager.RecordCompleteComponent(this);
			}
			break;
		}
	}
	
	/*
	  ProcessKeyboardInput() listens for modified key code events.
	
	  i.e. ctrl+<key>, ctrl+shift+<key>, etc.
	*/
	private void ProcessKeyboardCommand(string command) {
		TextEditor t = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
		
		switch (command) {
		case "SelectAll":
			t.SelectAll();
			break;
			
		case "Copy":
			t.Copy();
			break;
			
		case "Paste":
			// record history before pasting text
			HistoryManager.RecordCompleteComponent(this);
			
			int startIndex = Math.Min(t.selectIndex, t.cursorIndex);
			
			// check for selection before pasting clipboard
			if (t.selectIndex != t.cursorIndex) {
				text = text.Remove(startIndex, Math.Abs(t.selectIndex - t.cursorIndex));
			}
			
			// insert the clipboard text
			text = text.Insert(startIndex, EditorGUIUtility.systemCopyBuffer);
			
			// this moves the internal TextEditor cursor to the right index
			t.Paste();
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
	
	// hashes on the text instead of the object
	public override int GetHashCode() {
		return text.GetHashCode();
	}
}
