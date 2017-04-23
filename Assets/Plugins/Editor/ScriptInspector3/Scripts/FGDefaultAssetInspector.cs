/* SCRIPT INSPECTOR 3
 * version 3.0.17, December 2016
 * Copyright © 2012-2016, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace ScriptInspector
{

[CustomEditor(typeof(DefaultAsset), true)]
public class FGDefaultAssetInspector : ScriptInspector
{
	private readonly HashSet<string> textFiles = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) {
		".md",
		".xaml",
		".text",
		".bat",
		".cmd",
		".sh",
		",command",
		".ini",
		".rsp",
		".plist",
		".log",
		".lua",
	};
	
	[System.NonSerialized]
	private string checkedPath;
	[System.NonSerialized]
	private bool checkResult;
	
	public override void OnInspectorGUI()
	{
		var path = AssetDatabase.GetAssetPath(target);
		if (path != checkedPath)
		{
			checkedPath = path;
			
			var extension = System.IO.Path.GetExtension(path);
			checkResult = !AssetDatabase.IsValidFolder(path) && textFiles.Contains(extension);
		}
		
		if (checkResult)
		{
			base.OnInspectorGUI();
		}
		else
		{
			DrawDefaultInspector();
		}
	}
	
	protected override void DoGUI()
	{
		var currentInspector = GetCurrentInspector();
		
		textEditor.OnInspectorGUI(false, new RectOffset(0, 0, 14, -13), currentInspector);
	}
}

}
#endif
