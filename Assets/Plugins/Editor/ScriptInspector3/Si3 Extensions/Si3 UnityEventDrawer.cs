/* SCRIPT INSPECTOR 3
 * version 3.0.18, May 2017
 * Copyright © 2012-2017, Flipbook Games
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

#if !UNITY3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5


namespace ScriptInspector.Extensions.FlipbookGames
{

using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[InitializeOnLoad]
[CustomPropertyDrawer(typeof(UnityEventBase), true)]
public class Si3UnityEventDrawer : UnityEventDrawer
{
	private const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	private const BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	
	static Si3UnityEventDrawer()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
			Initialize();
		else
			EditorApplication.update += Initialize;
	}
	
	static void Initialize()
	{
		EditorApplication.update -= Initialize;
		
		var sauType = typeof(UnityEventDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
		if (sauType == null)
			return;
		var dtftField = sauType.GetField("s_DrawerTypeForType", staticFlags);
		if (dtftField == null)
			return;
		var dictionary = dtftField.GetValue(null) as IDictionary;
		if (dictionary == null)
		{
			var bdtftMethod = sauType.GetMethod("BuildDrawerTypeForTypeDictionary", staticFlags);
			if (bdtftMethod == null)
				return;
			
			bdtftMethod.Invoke(null, null);
			
			dictionary = dtftField.GetValue(null) as IDictionary;
			if (dictionary == null)
				return;
		}
		
		var drawerKeySetType = typeof(UnityEventDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility+DrawerKeySet");
		if (drawerKeySetType == null)
			return;
		var drawerField = drawerKeySetType.GetField("drawer", instanceFlags);
		var typeField = drawerKeySetType.GetField("type", instanceFlags);
		if (dtftField == null || typeField == null)
			return;
		
		var thisType = typeof(Si3UnityEventDrawer);
		var baseType = typeof(UnityEventBase);
		var unityDrawerType = typeof(UnityEventDrawer);
		
	resync:
		foreach (var i in dictionary)
		{
			var kv = (DictionaryEntry) i;
			var key = kv.Key as System.Type;
			var value = kv.Value;
			if (baseType.IsAssignableFrom(key))
			{
				var drawer = drawerField.GetValue(value) as System.Type;
				if (drawer == unityDrawerType)
				{
					drawerField.SetValue(value, thisType);
					dictionary[key] = value;
					goto resync;
				}
			}
		}
	}
	
	[System.NonSerialized]
	private UnityEditorInternal.ReorderableList.ElementCallbackDelegate originalCallback;
	
	[System.NonSerialized]
	private SerializedProperty listenersArray;
	[System.NonSerialized]
	private UnityEventBase currentEvent;
	private FieldInfo targetField;
	[System.NonSerialized]
	private object persistentCallGroup;
	private System.Type persistentCallType;
	private System.Type persistentCallGroupType;
	private FieldInfo persistentCallsField;
	private FieldInfo listenersArrayField;
	private MethodInfo findMethodMethod;
	private MethodInfo getListenerMethod;
	private MethodInfo restoreStateMethod;
	private FieldInfo reorderableListField;
	private MethodInfo rebuildMethod;
	private FieldInfo dummyEventField;
	private FieldInfo textField;
	
	private readonly char[] propertyPathSeparators = { '.', '[', ']' };
	private static readonly GUIContent tempContent = new GUIContent();
	private readonly GUIStyle headerBackground = (GUIStyle) "RL Header";
	
	[System.NonSerialized]
	private bool expanded;
	
	protected override void DrawEventHeader(Rect headerRect)
	{
		var indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		headerRect.xMin += 10f;
		if (currentEvent != null)
		{
			if (currentEvent.GetType().ToString() == "UnityEngine.EventSystems.EventTrigger+TriggerEvent")
				headerRect.xMax -= 20f;
			
			tempContent.text = "";
			tempContent.tooltip = "";
			expanded = EditorGUI.Foldout(headerRect, expanded, tempContent, true);
			
			var count = currentEvent.GetPersistentEventCount();
			tempContent.text = count.ToString();
			var size = EditorStyles.label.CalcSize(tempContent);
			var rc = headerRect;
			headerRect.xMax -= size.x + 4f;
			if (Event.current.type == EventType.Repaint)
			{
				rc.xMin = rc.xMax - size.x;
				EditorStyles.label.Draw(rc, tempContent, false, false, false, false);
			}
		}
		base.DrawEventHeader(headerRect);
		
		EditorGUI.indentLevel = indentLevel;
	}
	
	protected bool InitializeForProperty(SerializedProperty property)
	{
		if (restoreStateMethod == null)
		{
			restoreStateMethod = typeof(UnityEventDrawer).GetMethod("RestoreState", instanceFlags);
			reorderableListField = typeof(UnityEventDrawer).GetField("m_ReorderableList", instanceFlags);
			
			dummyEventField = typeof(UnityEventDrawer).GetField("m_DummyEvent", instanceFlags);
			textField = typeof(UnityEventDrawer).GetField("m_Text", instanceFlags);
			
			persistentCallType = System.Type.GetType("UnityEngine.Events.PersistentCall,UnityEngine");
			if (persistentCallType != null)
			{
				findMethodMethod = typeof(UnityEventBase).GetMethod("FindMethod", instanceFlags, null, new System.Type[] { persistentCallType }, null);
				persistentCallGroupType = System.Type.GetType("UnityEngine.Events.PersistentCallGroup,UnityEngine");
				if (persistentCallGroupType != null)
					getListenerMethod = persistentCallGroupType.GetMethod("GetListener", instanceFlags);
				persistentCallsField = typeof(UnityEventBase).GetField("m_PersistentCalls", instanceFlags);
				listenersArrayField = typeof(UnityEventDrawer).GetField("m_ListenersArray", instanceFlags);
				
				rebuildMethod = typeof(UnityEventBase).GetMethod("RebuildPersistentCallsIfNeeded", instanceFlags | BindingFlags.FlattenHierarchy);
			}
		}
		
		if (restoreStateMethod == null || reorderableListField == null || findMethodMethod == null ||
			getListenerMethod == null || persistentCallsField == null || listenersArrayField == null ||
			rebuildMethod == null)
		{
			return false;
		}
		
		expanded = property.isExpanded;
		
		restoreStateMethod.Invoke(this, new object[]{ property });
		
		var reorderableList = reorderableListField.GetValue(this) as UnityEditorInternal.ReorderableList;
		if (reorderableList == null)
		{
			return false;
		}
		
		if (originalCallback == null || originalCallback.Target == null)
			originalCallback = reorderableList.drawElementCallback;
		reorderableList.drawElementCallback = DrawEventListener;
		
		//...
		
		listenersArray = listenersArrayField.GetValue(this) as SerializedProperty;
		if (listenersArray != null && !listenersArray.isArray)
		{
#if SI3_WARNINGS
			Debug.LogWarning(listenersArray);
#endif
			listenersArray = null;
		}
		
		object targetObject = property.serializedObject.targetObject;
		targetField = null;
		var fieldNames = property.propertyPath.Split(propertyPathSeparators, System.StringSplitOptions.RemoveEmptyEntries);
		for (var i = 0; targetObject != null && i < fieldNames.Length; ++i)
		{
			if (fieldNames[i] == "Array")
			{
				targetField = null;
				
				var array = targetObject as IList;
				if (array == null)
					break;
				var index = int.Parse(fieldNames[i += 2]);
				if (index >= array.Count)
					break;
				targetObject = array[index];
			}
			else
			{
				for (var type = targetObject.GetType(); type != typeof(object); type = type.BaseType)
				{
					targetField = type.GetField(fieldNames[i], instanceFlags | BindingFlags.DeclaredOnly);
					if (targetField != null)
						break;
				}
				if (targetField == null)
				{
					Debug.LogWarning("Could not find field #" + i + " in type " + targetObject.GetType().FullName + "\n" + string.Join(", ", fieldNames));
					targetObject = null;
					break;
				}
				targetObject = targetField.GetValue(targetObject);
			}
		}
		currentEvent = targetObject as UnityEventBase;
		
		return true;
	}
		
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!InitializeForProperty(property))
			return base.GetPropertyHeight(property, label);
		
		var height = base.GetPropertyHeight(property, label);
		return property.isExpanded ? height : 16f;
	}
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!InitializeForProperty(property))
		{
			base.OnGUI(position, property, label);
			return;
		}
		
		expanded = property.isExpanded;
		if (!expanded)
			position.height = 16f;
		
		if (expanded || dummyEventField == null || textField == null)
		{
			base.OnGUI(position, property, label);
		}
		else
		{
			if (currentEvent != null)
			{
				if (Event.current.type == EventType.Repaint)
					headerBackground.Draw(position, false, false, false, false);
				
				dummyEventField.SetValue(this, currentEvent);
				textField.SetValue(this, label.text);
				
				var rc = position;
				rc.xMin += 6f;
				rc.xMax -= 6f;
				rc.height -= 2f;
				rc.y++;
				DrawEventHeader(rc);
			}
		}
		
		property.isExpanded = expanded;
		
		if (targetField != null)
		{
			if (System.Attribute.IsDefined(targetField, typeof(TooltipAttribute)))
			{
				var attribute = (TooltipAttribute) System.Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
				if (attribute.tooltip != "")
				{
					position.height = 18f;
					tempContent.text = "";
					tempContent.tooltip = attribute.tooltip;
					GUI.Label(position, tempContent);
				}
			}
		}
	}
	
	private void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
	{
		var element = listenersArray.GetArrayElementAtIndex(index);
		var propertyMethodName = element.FindPropertyRelative("m_MethodName");
		bool isDefined = !string.IsNullOrEmpty(propertyMethodName.stringValue);
		
		MethodInfo method = null;
		if (isDefined && currentEvent != null)
		{
			try
			{
				rebuildMethod.Invoke(currentEvent, null);
			} catch {}
			persistentCallGroup = persistentCallsField.GetValue(currentEvent);
			var persistentCall = getListenerMethod.Invoke(persistentCallGroup, new object[] { index });
			method = findMethodMethod.Invoke(currentEvent, new [] { persistentCall }) as MethodInfo;
			
			isDefined = false;
			if (method != null)
			{
				var declaringType = method.DeclaringType;
				if (declaringType != null)
				{
					var name = declaringType.Assembly.GetName().Name.ToLowerInvariant();
					isDefined = name == "assembly-csharp" || name == "assembly-csharp-firstpass" ||
						name == "assembly-csharp-editor" || name == "assembly-csharp-editor-firstpass";
				}
			}
		}
		
		bool isDoubleClick = Event.current.type == EventType.MouseDown
			&& Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition);
		
		Rect rc = rect;
		rc.y += 3f;
		rc.height = 15f;
		rc.xMin = rc.xMax - 21f;
		rc.width = 21f;
		
		rect.width -= 20f;
		if (originalCallback != null)
			originalCallback(rect, index, isactive, isfocused);
		
		if (isactive && isfocused && Event.current.type == EventType.KeyDown && Event.current.character == '\n')
			isDoubleClick = true;
		
		bool wasEnabled = GUI.enabled;
		GUI.enabled = isDefined;
		bool isButtonClick = GUI.Button(rc, "...", EditorStyles.miniButtonRight);
		if (isDefined && (isButtonClick || isDoubleClick && Event.current.type != EventType.Used))
		{
			var declaringType = ReflectedTypeReference.ForType(method.DeclaringType).definition;
			if (declaringType != null)
			{
				var member = declaringType.FindName(method.IsSpecialName ? method.Name.Substring("set_".Length) : method.Name, 0, false);
				if (member != null)
				{
					List<SymbolDeclaration> declarations = null;
					
					var methodGroup = member as MethodGroupDefinition;
					if (member.kind == SymbolKind.MethodGroup && methodGroup != null)
					{
						var parameters = method.GetParameters();
						foreach (var m in methodGroup.methods)
						{
							if (m.IsStatic)
								continue;
							
							var p = m.GetParameters() ?? new List<ParameterDefinition>();
							if (p.Count != parameters.Length)
								continue;
							
							for (var i = p.Count; i --> 0; )
							{
								var pType = p[i].TypeOf();
								if (pType == null)
									goto nextMethod;
								
								var parameterType = ReflectedTypeReference.ForType(parameters[i].ParameterType).definition as TypeDefinitionBase;
								if (!pType.IsSameType(parameterType))
									goto nextMethod;
							}
							
							// Yay! We found it :)
							declarations = m.declarations;
							if (declarations == null || declarations.Count == 0)
							{
								declarations = FGFindInFiles.FindDeclarations(m);
								if (declarations == null || declarations.Count == 0)
								{
									// Boo! Something went wrong.
									break;
								}
							}
							
						nextMethod:
							continue;
						}
					}
					else
					{
						// It's a property
						
						declarations = member.declarations;
						if (declarations == null || declarations.Count == 0)
							declarations = FGFindInFiles.FindDeclarations(member);
					}
					
					if (declarations != null && declarations.Count > 0)
					{
						foreach (var decl in declarations)
						{
							var node = decl.NameNode();
							if (node == null || !node.HasLeafs())
								continue;
							
							string cuPath = null;
							for (var scope = decl.scope; scope != null; scope = scope.parentScope)
							{
								var cuScope = scope as CompilationUnitScope;
								if (cuScope != null)
								{
									cuPath = cuScope.path;
									break;
								}
							}
							if (cuPath == null)
								continue;
							
							var cuObject = AssetDatabase.LoadAssetAtPath(cuPath, typeof(MonoScript));
							if (cuObject == null)
								continue;
							
							var buffer = FGTextBufferManager.GetBuffer(cuObject);
							if (buffer == null)
								continue;
							
							// Declaration is valid!
							
							if (buffer.lines.Count == 0)
							{
								buffer.LoadImmediately();
							}
							var span = buffer.GetParseTreeNodeSpan(node);
							EditorApplication.delayCall += () =>
							{
								FGCodeWindow.OpenAssetInTab(AssetDatabase.AssetPathToGUID(cuPath), span.line + 1);
							};
						}
					}
				}
			}
		}
		GUI.enabled = wasEnabled;
	}
}
	
}


#endif
