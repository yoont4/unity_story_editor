using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  OutstreamSplitters are used for control flow, with a positive and negative
  outstream ConnectionPoint.
  
  positive outpoints are drawn on top, negative outpoints are drawn on bottom
*/
public class OutstreamSplitter : ScriptableObject {
	
	public Rect rect;
	public ConnectionPoint positiveOutpoint;
	public ConnectionPoint negativeOutpoint;
	
	private Texture2D image;
	
	public OutstreamSplitter(){}
	
	public void Init() {
		this.positiveOutpoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.positiveOutpoint.Init(null, ConnectionPointType.Out);
		this.positiveOutpoint.anchored = true;
		
		this.negativeOutpoint = ScriptableObject.CreateInstance<ConnectionPoint>();
		this.negativeOutpoint.Init(null, ConnectionPointType.Out);
		this.negativeOutpoint.anchored = true;
		
		// set the rect to be the same size as the splitter texture
		this.rect.width = 32f;
		this.rect.height = 32f;
		
		// get the texture
		this.image = SDEStyles.outstreamSplitterTexture;
	}
	
	public void Draw() {
		// set the positions
		positiveOutpoint.SetPosition(rect.x+16, rect.y-1);
		negativeOutpoint.SetPosition(rect.x+16, rect.y+17);
		
		positiveOutpoint.Draw();
		negativeOutpoint.Draw();
		
		GUI.DrawTexture(rect, image);
	}
	
	public void ProcessEvent(Event e) {
		positiveOutpoint.ProcessEvent(e);
		negativeOutpoint.ProcessEvent(e);
	}
	
	public void SetPosition (float x, float y) {
		rect.x = x;
		rect.y = y;
	}
}
