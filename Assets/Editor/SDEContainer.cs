using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SDEContainerType {DialogBox}

public abstract class SDEContainer : ScriptableObject {
	
	// set if this container is the child of a Node
	public Node parentNode;
	
	public SDEContainerType containerType;
	
	// references for container chaining
	public SDEContainer parent;
	public SDEContainer child;
	
	// x,y pos of the container and the size of it's contents
	public Rect rect;
	
	// references for control flow
	public ConnectionPoint outPoint;
	
	public SDEContainer() {}
	public virtual void Init(SDEContainer parent) {
		this.parent = parent;
		
	}
	
	public virtual void Init(Node parentNode) {
		this.parentNode = parentNode;
	}
	
	public abstract void Draw();
	public abstract void ProcessEvent(Event e);
}
