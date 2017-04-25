using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextArea {
	
	// parent Node reference
	public Node node;
	
	public string text;
	public Rect textRect;
	public Rect boxRect;
	
	public GUIStyle textAreaStyle;
	public GUIStyle defaultTextBoxStyle;
	public GUIStyle selectedTextBoxStyle;
	
	private GUIContent textContent;
	private float contentHeight;
	
	public TextArea(Node node, string text) {
		this.node = node;
		
		// fields are 0, because they will get recalculated on Draw()
		this.boxRect = new Rect(0, 0, node.rect.width, 0);
		this.textRect = new Rect(0, 0, node.rect.width-8f, 0);
		this.text = text;
		
		// TODO: implement TextAreaManager to manage all the styles
		// copy ConnectionPoint structure
		this.textAreaStyle = TextAreaManager.textAreaStyle;
		this.defaultTextBoxStyle = TextAreaManager.defaultTextBoxStyle;
		this.selectedTextBoxStyle = TextAreaManager.selectedTextBoxStyle;
	}
	
	/*
	  TextArea has a bounding box that borders it by 4px each side.
	  i.e. if a Node is 200px wide, the bounding box would be 200px, and
	  the TextArea itself would be 192px.
	*/
	public void Draw() {
		// GUI.TextArea fucking sucks so I need to draw a box around it
		// and then make a smaller TextArea inside because .padding breaks
		// the formatting while editing.
		
		textContent = new GUIContent(text);
		contentHeight = textAreaStyle.CalcHeight(textContent, textRect.width);
		textRect.height = contentHeight;
		boxRect.height = contentHeight + 8f;
		
		// calculate position based off of parent Node
		boxRect.x = node.rect.x;
		boxRect.y = node.rect.y+NodeManager.NODE_HEIGHT;
		textRect.x = boxRect.x + 4f;
		textRect.y = boxRect.y + 4f;
		
		// TODO: implement box style state
		GUI.Box(boxRect, "", defaultTextBoxStyle);
		text = GUI.TextArea(textRect, text, textAreaStyle);
	}
	
	public void ProcessEvent(Event e) {
		switch(e.type) {
		case EventType.MouseDown:
			if (e.button == 0) {
				if (boxRect.Contains(e.mousePosition)) {
					
				} else {
					GUIUtility.keyboardControl = 0;
				}
			}
			break;
		}
	}
}
