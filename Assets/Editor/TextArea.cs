using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextArea : SDEComponent {
	
	public string text;
	
	private GUIContent textContent;
	private float contentHeight;
	
	public GUIStyle textAreaStyle;
	
	// this links to either another node or the interrupt split
	public ConnectionPoint outPoint;
	
	public TextArea() {}
	
	public void Init(SDEComponent parent, string text) {
		Init(SDEComponentType.TextArea, parent, 
			new Rect(0, 0, parent.clickRect.width-2*TextAreaManager.X_PAD, 0), 
			TextAreaManager.textBoxDefault, 
			TextAreaManager.textBoxDefault, 
			TextAreaManager.textBoxSelected);
		
		// make the clickRect 4 pixels bigger on each side.
		// clickRect is used to define the TextArea's BG Box
		this.ExtendClickBound(new Vector2(TextAreaManager.X_PAD, TextAreaManager.Y_PAD));
		this.textAreaStyle = TextAreaManager.textAreaStyle;
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
		
		GUI.Box(clickRect, "", style);
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
			if (e.button == 0) {
				if (clickRect.Contains(e.mousePosition)) {
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
		}
	}
}
