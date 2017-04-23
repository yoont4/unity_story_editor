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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ScriptInspector
{

class GenerateOverrideMethod : ISnippetProvider
{
	class OverrideMethod : SnippetCompletion
	{
		public static Scope context;
		public static int overrideTextLength;
		
		private readonly MethodDefinition method;
		
		public OverrideMethod(MethodDefinition virtualMethod)
			: base(virtualMethod.name)
		{
			method = virtualMethod;
			displayFormat = GetDisplayName(virtualMethod);
		}
		
		private static string GetDisplayName(MethodDefinition method)
		{
			var parameters = method.PrintParameters(method.GetParameters(), true);
			string generics = method.NumTypeParameters == 0 ? "" :
				"<" +
				string.Join(", ", (from t in method.typeParameters select t.name).ToArray())
				+ ">";
			return "{0}" + generics + "(" + parameters + ") {{...}}";
		}
		
		public override string Expand()
		{
			string modifiersString =
				method.IsInternal ? (method.IsProtected ? "internal protected" : "internal") :
				method.IsProtected ? "protected" : "public";
			string returnType = method.ReturnType().RelativeName(context);
			string generics = method.NumTypeParameters == 0 ? "" :
				"<" +
				string.Join(", ", (from t in method.typeParameters select t.name).ToArray())
				+ ">";
			var parameters = method.GetParameters();
			var parametersSignature = method.PrintParameters(parameters, true); // TODO: Use the context!
			var argumentSeparator = "";
			var arguments = "";
			for (var i = 0; i < parameters.Count; i++)
			{
				var p = parameters[i];
				arguments += argumentSeparator;
				if (p.IsRef)
					arguments += "ref ";
				else if (p.IsOut)
					arguments += "out ";
				arguments += p.name;
				argumentSeparator = ", ";
			}
			var baseCall = method.IsAbstract ?
				"throw new " + ReflectedTypeReference.ForType(typeof(System.NotImplementedException)).definition.RelativeName(context) + "();" :
				"base." + method.name + generics + "(" + arguments + ");";
			var returnStatement = returnType == "void" || method.IsAbstract ? "" : "return ";
			
			var expandedCode = string.Format(
				"{0} override {1} {2}{3}({4}){5}{{\n\t{6}{7}$end$\n}}",
				modifiersString, returnType, method.name, generics, parametersSignature,
				SISettings.magicMethods_openingBraceOnSameLine ? " " : "\n",
				returnStatement, baseCall);
			return expandedCode;
		}

		public override void OverrideTypedInLength(ref int typedInLength)
		{
			typedInLength += overrideTextLength;
		}
	}	
	
	public IEnumerable<SnippetCompletion> EnumSnippets(
		SymbolDefinition context,
		FGGrammar.TokenSet expectedTokens,
		SyntaxToken tokenLeft,
		Scope scope)
	{
		OverrideMethod.context = scope;
		
		if (tokenLeft == null || tokenLeft.parent == null || tokenLeft.parent.parent == null)
			yield break;
		
		if (tokenLeft.tokenKind != SyntaxToken.Kind.Keyword)
			yield break;
		
		if (tokenLeft.text != "override")
			yield break;
		
		var bodyScope = scope as BodyScope;
		if (bodyScope == null)
			yield break;
		
		var contextType = bodyScope.definition as TypeDefinitionBase;
		if (contextType == null || contextType.kind != SymbolKind.Class && contextType.kind != SymbolKind.Struct)
			yield break;
		
		var baseType = contextType.BaseType();
		if (baseType == null || baseType.kind != SymbolKind.Class && baseType.kind != SymbolKind.Struct)
			yield break;
		
		var overrideMethodCandidates = new List<MethodDefinition>();
		baseType.ListOverrideCandidates(overrideMethodCandidates, contextType.Assembly);
		if (overrideMethodCandidates.Count == 0)
			yield break;
		
		var textBuffer = FGTextBuffer.activeEditor.TextBuffer;
		var firstToken = tokenLeft.parent.parent.GetFirstLeaf().token;
		if (firstToken.formatedLine != tokenLeft.formatedLine)
		{
			firstToken = tokenLeft.formatedLine.tokens[0];
			while (firstToken.tokenKind <= SyntaxToken.Kind.LastWSToken)
				firstToken = firstToken.formatedLine.tokens[firstToken.TokenIndex + 1];
		}
		var tokenSpan = textBuffer.GetTokenSpan(firstToken.parent);
		OverrideMethod.overrideTextLength = FGTextBuffer.activeEditor.caretPosition.characterIndex - tokenSpan.StartPosition.index;
		
		foreach (var method in overrideMethodCandidates)
		{
			var methodGroup = contextType.FindName(method.name, -1, false) as MethodGroupDefinition;
			if (methodGroup != null)
			{
				bool skipThis = false;
				var signature = method.PrintParameters(method.GetParameters(), true);
				foreach (var m in methodGroup.methods)
				{
					if (method.NumTypeParameters == m.NumTypeParameters &&
						signature == m.PrintParameters(m.GetParameters()))
					{
						skipThis = true;
						break;
					}
				}
				if (skipThis)
					continue;
			}
			
			var overrideCompletion = new OverrideMethod(method);
			yield return overrideCompletion;
		}
	}
	
	public string Get(
		string shortcut,
		SymbolDefinition context,
		FGGrammar.TokenSet expectedTokens,
		Scope scope)
	{
		return null;
	}
}
	
}
