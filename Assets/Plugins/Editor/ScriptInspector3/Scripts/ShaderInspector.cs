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

namespace ScriptInspector
{

using UnityEngine;
using UnityEditor;
using System.Reflection;


[CustomEditor(typeof(Shader))]
public class ShaderInspector : ScriptInspector
{
	private static System.Type unityShaderInspectorType;
	private static MethodInfo internalSetTargetsMethod;
	
	public bool showInfo = true;
	private Editor unityShaderInspector;
	
	public override void OnInspectorGUI()
	{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		EditorGUIUtility.LookLikeControls();
#else
		EditorGUIUtility.labelWidth = 0f;
		EditorGUIUtility.fieldWidth = 0f;
#endif
		EditorGUI.indentLevel = 0;
		
		var rc = GUILayoutUtility.GetRect(1f, 13f);
		rc.yMin -= 5f;
		var enabled = GUI.enabled;
		GUI.enabled = true;
		showInfo = InspectorFoldout(rc, showInfo, targets);
		GUI.enabled = enabled;
		if (showInfo)
		{
			if (unityShaderInspectorType == null)
			{
				unityShaderInspectorType = typeof(Editor).Assembly.GetType("UnityEditor.ShaderInspector");
				if (unityShaderInspectorType != null)
				{
					const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
					internalSetTargetsMethod = unityShaderInspectorType.GetMethod("InternalSetTargets", flags);
				}
			}
			if (targets != null && internalSetTargetsMethod != null)
			{
				if (unityShaderInspector == null)
				{
					unityShaderInspector = Editor.CreateEditor(target, unityShaderInspectorType);
				//	unityShaderInspector = (Editor) CreateInstance(unityShaderInspectorType);
					if (unityShaderInspector)
						internalSetTargetsMethod.Invoke(unityShaderInspector, new object[] { targets.Clone() });
				}
				if (unityShaderInspector)
					unityShaderInspector.OnInspectorGUI();
			}
		}
		
		var assetPath = AssetDatabase.GetAssetPath(target);
		if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets/"))
			base.OnInspectorGUI();
	}
	
	protected override void DoGUI()
	{
		var currentInspector = GetCurrentInspector();
		
#if UNITY_4_3
		textEditor.OnInspectorGUI(false, new RectOffset(0, -4, showInfo ? 29 : 22, -13), currentInspector);
#else
		textEditor.OnInspectorGUI(false, new RectOffset(0, 0, showInfo ? 29 : 22, -13), currentInspector);
#endif
	}
	
	private static GUIStyle inspectorTitlebar;
	private static GUIStyle inspectorTitlebarText;
	
	public static bool InspectorFoldout(Rect position, bool foldout, UnityEngine.Object[] targetObjs)
	{
		if (inspectorTitlebar == null)
		{
			inspectorTitlebar = new GUIStyle("IN Title");
			inspectorTitlebarText = "IN TitleText";
		} 
		
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		EditorGUIUtility.LookLikeControls(Screen.width, 0f);
#elif UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		EditorGUIUtility.LookLikeControls(EditorGUIUtility.currentViewWidth, 0f);
#else
		EditorGUIUtility.fieldWidth = 0f;
		EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth;
#endif
		foldout = EditorGUI.Foldout(position, foldout, GUIContent.none, true, inspectorTitlebar);
		
		position = inspectorTitlebar.padding.Remove(position);
		if (Event.current.type == EventType.Repaint)
			inspectorTitlebarText.Draw(position, "Shader Info", false, false, foldout, false);
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		EditorGUIUtility.LookLikeControls();
#else
		EditorGUIUtility.labelWidth = 0f;
		EditorGUIUtility.fieldWidth = 0f;
#endif
		
		return foldout;
	}
}

}
