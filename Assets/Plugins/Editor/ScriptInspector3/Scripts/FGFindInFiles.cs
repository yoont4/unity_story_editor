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
using System.IO;


namespace ScriptInspector
{

public static class FGFindInFiles
{
	public static List<string> assets;
	
	private static List<string> ignoreFileTypes = new List<string> { ".dll", ".a", ".so", ".dylib", ".exe" };
	
	public static List<SymbolDeclaration> FindDeclarations(SymbolDefinition symbol)
	{
		var candidates = FindDefinitionCandidates(symbol);
		foreach (var c in candidates)
		{
			var asset = AssetDatabase.LoadAssetAtPath(c, typeof(TextAsset)) as TextAsset;
			if (!asset)
				continue;
			var buffer = FGTextBufferManager.GetBuffer(asset);
			buffer.LoadImmediately();
		}
		
		var newSymbol = symbol.Rebind();
		var declarations = newSymbol == null ? null : newSymbol.declarations;
		return declarations ?? symbol.declarations;
	}
	
	static List<string> FindDefinitionCandidates(SymbolDefinition symbol)
	{
		var result = new List<string>();
		if (assets != null)
			assets.Clear();
		
		var symbolType = symbol;
		if (symbol.kind == SymbolKind.Namespace)
			return result;
		
		while (symbolType != null &&
			symbolType.kind != SymbolKind.Class && symbolType.kind != SymbolKind.Struct &&
			symbolType.kind != SymbolKind.Enum && symbolType.kind != SymbolKind.Interface &&
			symbolType.kind != SymbolKind.Delegate)
		{
			symbolType = symbolType.parentSymbol;
		}
		
		var assembly = symbolType.Assembly;
		var assemblyId = assembly.assemblyId;
		FindAllAssemblyScripts(assemblyId);
		for (int i = assets.Count; i --> 0; )
			assets[i] = AssetDatabase.GUIDToAssetPath(assets[i]);
		
		string[] words;
		string typeName = symbolType.name;
		switch (symbolType.kind)
		{
		case SymbolKind.Class: words = new [] { "class", typeName }; break;
		case SymbolKind.Struct: words = new [] { "struct", typeName }; break;
		case SymbolKind.Interface: words = new [] { "interface", typeName }; break;
		case SymbolKind.Enum: words = new [] { "enum", typeName }; break;
		case SymbolKind.Delegate: words = new [] { typeName, "(" }; break;
			default: return result;
		}
		
		for (int i = assets.Count; i --> 0; )
			if (ContainsWordsSequence(assets[i], words))
				result.Add(assets[i]);
		
		return result;
	}
	
	public static void FindAllReferences(SymbolDefinition symbol, string localAssetPath)
	{
		if (symbol.kind == SymbolKind.Accessor || symbol.kind == SymbolKind.Constructor || symbol.kind == SymbolKind.Destructor)
			symbol = symbol.parentSymbol;
		if (symbol == null)
			return;
		
		symbol = symbol.GetGenericSymbol();
		
		var candidates = FindReferenceCandidates(symbol, localAssetPath);
		
		var searchOptions = new FindResultsWindow.SearchOptions {
			text = symbol.name,
			matchWord = true,
			matchCase = true,
		};
		
		var candidateGuids = new string[candidates.Count];
		for (int i = 0; i < candidates.Count; i++)
			candidateGuids[i] = AssetDatabase.AssetPathToGUID(candidates[i]);
		
		var searchForVarRefs = symbol is TypeDefinitionBase && symbol.kind != SymbolKind.Delegate;
		if (searchForVarRefs)
		{
			searchOptions.altText1 = "var";
			
			var builtInTypesEnumerator = SymbolDefinition.builtInTypes.GetEnumerator();
			for (var i = 0; i < 16; i++)
			{
				builtInTypesEnumerator.MoveNext();
				var type = builtInTypesEnumerator.Current.Value;
				if (type == symbol)
				{
					searchOptions.altText2 = builtInTypesEnumerator.Current.Key;
					break;
				}
			}
		}
		
		var resultsWindow = FindResultsWindow.Create(
			"References to " + symbol.FullName,
			FindAllInSingleFile,
			candidateGuids,
			searchOptions,
			"References");
		resultsWindow.SetFilesValidator(ValidateFileForReferences);
		resultsWindow.SetResultsValidator(ValidateResultAsReference, symbol);
	}
	
	public static void RenameSymbol(SymbolDefinition symbol, string localAssetPath)
	{
		if (symbol.kind == SymbolKind.Accessor || symbol.kind == SymbolKind.TypeAlias)
			return;
		
		if (symbol.kind == SymbolKind.Constructor || symbol.kind == SymbolKind.Destructor)
			symbol = symbol.parentSymbol;
		if (symbol == null)
			return;
		
		symbol = symbol.GetGenericSymbol();
		
		var assembly = symbol.Assembly;
		if (assembly == null)
			return;
		var assemblyId = assembly.assemblyId;
		if (assemblyId != AssemblyDefinition.UnityAssembly.CSharpFirstPass &&
			assemblyId != AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass &&
			assemblyId != AssemblyDefinition.UnityAssembly.CSharp &&
			assemblyId != AssemblyDefinition.UnityAssembly.CSharpEditor)
		{
			// Only symbols defined in C# scripts can be renamed
			return;
		}
		
		var candidates = FindReferenceCandidates(symbol, localAssetPath);
		
		var searchOptions = new FindResultsWindow.SearchOptions {
			text = symbol.name,
			matchWord = true,
			matchCase = true,
		};
		
		var candidateGuids = new string[candidates.Count];
		for (int i = 0; i < candidates.Count; i++)
			candidateGuids[i] = AssetDatabase.AssetPathToGUID(candidates[i]);
		
		var searchForVarRefs = symbol is TypeDefinitionBase && symbol.kind != SymbolKind.Delegate;
		if (searchForVarRefs)
		{
			searchOptions.altText1 = "var";
			
			var builtInTypesEnumerator = SymbolDefinition.builtInTypes.GetEnumerator();
			for (var i = 0; i < 16; i++)
			{
				builtInTypesEnumerator.MoveNext();
				var type = builtInTypesEnumerator.Current.Value;
				if (type == symbol)
				{
					searchOptions.altText2 = builtInTypesEnumerator.Current.Key;
					break;
				}
			}
		}
		
		FindResultsWindow resultsWindow = FindResultsWindow.Create(
			"Rename " + symbol.FullName,
			FindAllInSingleFile,
			candidateGuids,
			searchOptions,
			"Rename");
		resultsWindow.SetFilesValidator(ValidateFileForReferences);
		resultsWindow.SetResultsValidator(ValidateResultAsReference, symbol);
		resultsWindow.SetReplaceText(symbol.name);
	}
	
	static bool ValidateFileForReferences(string assetGuid, FindResultsWindow.FilteringOptions options)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		var isCsScript = assetPath.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase);
		if (isCsScript)
			return true;
		var isJsScript = assetPath.EndsWith(".js", System.StringComparison.OrdinalIgnoreCase);
		if (isJsScript)
			return options.jsScripts;
		var isBooScript = assetPath.EndsWith(".boo", System.StringComparison.OrdinalIgnoreCase);
		if (isBooScript)
			return options.booScripts;
		if (FindReplaceWindow.shaderFileTypes.Contains(Path.GetExtension(assetPath).ToLowerInvariant()))
			return options.shaders;
		return options.textFiles;
	}
	
	static FindResultsWindow.ResultType ValidateResultAsReference(string assetGuid, TextPosition location, int length, ref SymbolDefinition referencedSymbol)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		if (string.IsNullOrEmpty(assetPath))
			return FindResultsWindow.ResultType.RemoveResult;
		
		var isCsScript = assetPath.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase);
		
		var buffer = FGTextBufferManager.GetBuffer(assetGuid);
		if (buffer == null)
			return FindResultsWindow.ResultType.RemoveResult;
		
		if (buffer.Parser == null)
		{
			buffer.LoadImmediately();
			referencedSymbol = referencedSymbol.Rebind();
		}
		
		var formatedLine = buffer.formatedLines[location.line];
		
		var textLine = buffer.lines[location.line];
		var isVarResult =
			length == 3 &&
			referencedSymbol is TypeDefinitionBase &&
			location.index + 3 < textLine.Length &&
			textLine[location.index] == 'v' &&
			textLine[location.index + 1] == 'a' &&
			textLine[location.index + 2] == 'r';
		
		if (isCsScript)
		{
			if (formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive)
			{
				if (isVarResult)
					return FindResultsWindow.ResultType.RemoveResult;
				return FindResultsWindow.ResultType.InactiveCode;
			}
		}
		else if (isVarResult)
		{
			return FindResultsWindow.ResultType.RemoveResult;
		}
		
		int tokenIndex;
		bool atTokenEnd;
		var token = buffer.GetTokenAt(new TextPosition(location.line, location.index + 1), out location.line, out tokenIndex, out atTokenEnd);
		switch (token.tokenKind)
		{
			case SyntaxToken.Kind.Preprocessor:
				return FindResultsWindow.ResultType.RemoveResult;
				
			case SyntaxToken.Kind.Comment:
			case SyntaxToken.Kind.PreprocessorArguments:
			case SyntaxToken.Kind.PreprocessorSymbol:
				if (isVarResult)
					return FindResultsWindow.ResultType.RemoveResult;
				return FindResultsWindow.ResultType.Comment;
			
			case SyntaxToken.Kind.StringLiteral:
			case SyntaxToken.Kind.VerbatimStringLiteral:
				if (isVarResult)
					return FindResultsWindow.ResultType.RemoveResult;
				return FindResultsWindow.ResultType.String;
		}
		
		if (!isCsScript || token.parent == null)
			return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
		var resolvedSymbol = token.parent.resolvedSymbol;
		if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
			FGResolver.ResolveNode(token.parent.parent);
		
		if (resolvedSymbol != null && resolvedSymbol.kind == SymbolKind.MethodGroup && token.parent.parent != null)
		{
			var nextLeaf = token.parent.parent.FindNextLeaf();
			if (nextLeaf != null && nextLeaf.IsLit("("))
			{
				var nextNode = nextLeaf.parent;
				if (nextNode.RuleName == "arguments")
				{
					FGResolver.ResolveNode(nextNode);
					if (token.parent != null)
						if (token.parent.resolvedSymbol == null || token.parent.resolvedSymbol.kind == SymbolKind.Error)
							token.parent.resolvedSymbol = resolvedSymbol;
				}
			}
		}
		
		resolvedSymbol = token.parent != null ? token.parent.resolvedSymbol : null;
		if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
			return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
		if (resolvedSymbol.kind == SymbolKind.Constructor || resolvedSymbol.kind == SymbolKind.Destructor)
			resolvedSymbol = resolvedSymbol.parentSymbol;
		if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
			return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
		var constructedSymbol = resolvedSymbol;
		resolvedSymbol = resolvedSymbol.GetGenericSymbol();
		
		if (referencedSymbol.kind == SymbolKind.MethodGroup && resolvedSymbol.kind == SymbolKind.Method)
			resolvedSymbol = resolvedSymbol.parentSymbol;
		
		if (resolvedSymbol != referencedSymbol)
		{
			var typeArgument = referencedSymbol as TypeDefinitionBase;
			var constructedType = constructedSymbol as ConstructedTypeDefinition;
			if (isVarResult && typeArgument != null && constructedType != null)
				if (IsUsedAsTypeArgument(typeArgument.GetGenericSymbol() as TypeDefinitionBase, constructedType))
					return FindResultsWindow.ResultType.VarTemplateReference;
			
			if (resolvedSymbol.kind == SymbolKind.Property && referencedSymbol.kind == SymbolKind.Property ||
				resolvedSymbol.kind == SymbolKind.Event && referencedSymbol.kind == SymbolKind.Event ||
				resolvedSymbol.kind == SymbolKind.Indexer && referencedSymbol.kind == SymbolKind.Indexer)
			{
				var resolvedProperty = resolvedSymbol as InstanceDefinition;
				var referencedProperty = referencedSymbol as InstanceDefinition;
				if (resolvedProperty != null && referencedProperty != null)
				{
					var resolvedType = resolvedProperty.parentSymbol as TypeDefinitionBase
						?? resolvedProperty.parentSymbol.parentSymbol as TypeDefinitionBase;
					var referencedType = referencedProperty.parentSymbol as TypeDefinitionBase
						?? referencedProperty.parentSymbol.parentSymbol as TypeDefinitionBase;
					
					var isInterface = resolvedType.kind == SymbolKind.Interface || referencedType.kind == SymbolKind.Interface;
					
					var resolvedIsVirtual = isInterface || resolvedProperty.IsOverride || resolvedProperty.IsVirtual || resolvedProperty.IsAbstract;
					var referencedIsVirtual = isInterface || referencedProperty.IsOverride || referencedProperty.IsVirtual || referencedProperty.IsAbstract;
					if (resolvedIsVirtual && referencedIsVirtual)
					{
						if (resolvedSymbol.kind != SymbolKind.Indexer ||
							System.Linq.Enumerable.SequenceEqual(
							System.Linq.Enumerable.Select(resolvedProperty.GetParameters(), x => x.TypeOf()),
							System.Linq.Enumerable.Select(referencedProperty.GetParameters(), x => x.TypeOf()) ))
						{
							if (resolvedType.DerivesFrom(referencedType))
								return FindResultsWindow.ResultType.OverridingMethod;
							if (referencedType.DerivesFrom(resolvedType))
								return FindResultsWindow.ResultType.OverriddenMethod;
						}
					}
				}
			}
			
			if (resolvedSymbol.kind == SymbolKind.Method && referencedSymbol.kind == SymbolKind.Method)
			{
				if (resolvedSymbol.parentSymbol == referencedSymbol.parentSymbol)
					return FindResultsWindow.ResultType.MethodOverload;
				
				var resolvedMethod = resolvedSymbol as MethodDefinition;
				var referencedMethod = referencedSymbol as MethodDefinition;
				if (resolvedMethod != null && referencedMethod != null)
				{
					var resolvedType = resolvedMethod.parentSymbol as TypeDefinitionBase
						?? resolvedMethod.parentSymbol.parentSymbol as TypeDefinitionBase;
					var referencedType = referencedMethod.parentSymbol as TypeDefinitionBase
						?? referencedMethod.parentSymbol.parentSymbol as TypeDefinitionBase;
					
					var isInterface = resolvedType.kind == SymbolKind.Interface || referencedType.kind == SymbolKind.Interface;
					
					var resolvedIsVirtual = isInterface || resolvedMethod.IsOverride || resolvedMethod.IsVirtual || resolvedMethod.IsAbstract;
					var referencedIsVirtual = isInterface || referencedMethod.IsOverride || referencedMethod.IsVirtual || referencedMethod.IsAbstract;
					if (resolvedIsVirtual && referencedIsVirtual)
					{
						if (System.Linq.Enumerable.SequenceEqual(
							System.Linq.Enumerable.Select(resolvedMethod.GetParameters(), x => x.TypeOf()),
							System.Linq.Enumerable.Select(referencedMethod.GetParameters(), x => x.TypeOf()) ))
						{
							if (resolvedType.DerivesFrom(referencedType))
								return FindResultsWindow.ResultType.OverridingMethod;
							if (referencedType.DerivesFrom(resolvedType))
								return FindResultsWindow.ResultType.OverriddenMethod;
						}
					}
				}
			}

			if (resolvedSymbol.kind != SymbolKind.MethodGroup || referencedSymbol.parentSymbol != resolvedSymbol)
				return FindResultsWindow.ResultType.RemoveResult;
		}
		
		if (isVarResult)
			return FindResultsWindow.ResultType.VarReference;
		
		if (FGResolver.IsWriteReference(token))
			return FindResultsWindow.ResultType.WriteReference;
		return FindResultsWindow.ResultType.ReadReference;
	}
	
	static bool IsUsedAsTypeArgument(TypeDefinitionBase typeArgument, ConstructedTypeDefinition constructedType)
	{
		var arguments = constructedType.typeArguments;
		if (arguments == null)
			return false;
		
		for (int i = arguments.Length; i --> 0; )
		{
			var argRef = arguments[i];
			if (argRef == null)
				continue;
			var arg = argRef.definition;
			if (arg == null)
				continue;
			
			if (arg.GetGenericSymbol() == typeArgument)
				return true;
			
			var constructedArg = arg as ConstructedTypeDefinition;
			if (constructedArg != null && IsUsedAsTypeArgument(typeArgument, constructedArg))
				return true;
		}
		return false;
	}
	
	static List<string> FindReferenceCandidates(SymbolDefinition symbol, string localAssetPath)
	{
		var result = new List<string> { localAssetPath };
		if (assets != null)
			assets.Clear();
		else
			assets = new List<string>();
		
		if (symbol.kind == SymbolKind.CatchParameter ||
			symbol.kind == SymbolKind.Destructor ||
			symbol.kind == SymbolKind.ForEachVariable ||
			symbol.kind == SymbolKind.FromClauseVariable ||
			symbol.kind == SymbolKind.Label ||
			symbol.kind == SymbolKind.LambdaExpression ||
			symbol.kind == SymbolKind.LocalConstant ||
			symbol.kind == SymbolKind.Parameter ||
			symbol.kind == SymbolKind.Variable)
		{
			// Local symbols cannot appear in any other file
			return result;
		}
		
		var allTextAssetGuids = FindAllTextAssets();
		for (int i = allTextAssetGuids.Count; i --> 0; )
		{
			var path = AssetDatabase.GUIDToAssetPath(allTextAssetGuids[i]);
			if (path != localAssetPath)
				if (!ignoreFileTypes.Contains(Path.GetExtension(path.ToLowerInvariant())))
					assets.Add(AssetDatabase.GUIDToAssetPath(allTextAssetGuids[i]));
		}
			
		for (int i = assets.Count; i --> 0; )
			result.Add(assets[i]);
		
		result.Sort((a, b) => {
			// Search .cs files first
			var extA = Path.GetExtension(a);
			if (extA.Equals(".cs", System.StringComparison.OrdinalIgnoreCase))
				return -1;
			var extB = Path.GetExtension(b);
			if (extB.Equals(".cs", System.StringComparison.OrdinalIgnoreCase))
				return 1;
			
			// Then .js and .boo files
			if (extA.Equals(".js", System.StringComparison.OrdinalIgnoreCase) ||
			  extA.Equals(".boo", System.StringComparison.OrdinalIgnoreCase))
				return -1;
			if (extB.Equals(".js", System.StringComparison.OrdinalIgnoreCase) ||
			  extB.Equals(".boo", System.StringComparison.OrdinalIgnoreCase))
				return 1;
			
			// And everything else at the end
			return 0;
		});
		
		return result;
	}
		
	public static void FindAllInSingleFile(
		System.Action<string, string, TextPosition, int> addResultAction,
		string assetGuid,
		FindResultsWindow.SearchOptions search)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		var isCsFile = Path.GetExtension(assetPath).Equals(".cs", System.StringComparison.OrdinalIgnoreCase);
		
		var allLines = GetOrReadAllLines(assetGuid);
		if (!isCsFile || search.altText1 == null)
		{
			foreach (var textPosition in FindAll(allLines, search))
			{
				var line = allLines[textPosition.line];
				addResultAction(line, assetGuid, textPosition, search.text.Length);
			}
		}
		else
		{
			var results = FindAll(allLines, search).GetEnumerator();
			
			var altSearch = new FindResultsWindow.SearchOptions {
				text = search.altText1,
				matchCase = search.matchCase,
				matchWord = search.matchWord,
			};
			var altResults = FindAll(allLines, altSearch).GetEnumerator();
			
			IEnumerator<TextPosition> altResults2 = null;
			if (search.altText2 != null)
			{
				var altSearch2 = new FindResultsWindow.SearchOptions {
					text = search.altText2,
					matchCase = search.matchCase,
					matchWord = search.matchWord,
				};
				altResults2 = FindAll(allLines, altSearch2).GetEnumerator();
			}
			
			bool more = results.MoveNext();
			bool altMore = altResults.MoveNext();
			bool altMore2 = altResults2 != null && altResults2.MoveNext();
			while (more || altMore || altMore2)
			{
				if (more && (!altMore || results.Current <= altResults.Current)
					&& (!altMore2 || results.Current <= altResults2.Current))
				{
					var line = allLines[results.Current.line];
					addResultAction(line, assetGuid, results.Current, search.text.Length);
					more = results.MoveNext();
				}
				else if (altMore && (!more || altResults.Current <= results.Current)
					&& (!altMore2 || altResults.Current <= altResults2.Current))
				{
					var line = allLines[altResults.Current.line];
					addResultAction(line, assetGuid, altResults.Current, search.altText1.Length);
					altMore = altResults.MoveNext();
				}
				else
				{
					var line = allLines[altResults2.Current.line];
					addResultAction(line, assetGuid, altResults2.Current, search.altText2.Length);
					altMore2 = altResults2.MoveNext();
				}
			}
		}
	}
	
	public static IList<string> GetOrReadAllLines(string assetGuid)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		return GetOrReadAllLinesForPath(assetPath);
	}
	
	public static IList<string> GetOrReadAllLinesForPath(string assetPath)
	{
		string[] lines;
		try
		{
			var textBuffer = FGTextBufferManager.TryGetBuffer(assetPath);
			if (textBuffer != null)
				return textBuffer.lines;
			
			lines = File.ReadAllLines(assetPath);
		}
		catch (IOException e)
		{
			Debug.LogError(e);
			return null;
		}
		return lines;
	}
	
	internal static IEnumerable<TextPosition> FindAll(IList<string> lines, FindResultsWindow.SearchOptions search)
	{
		var length = search.text.Length;
		if (length == 0)
			yield break;
		
		var comparison = search.matchCase ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase;
		
		char firstChar = search.text[0];
		bool startsAsWord = firstChar == '_' || char.IsLetterOrDigit(firstChar);
		char lastChar = search.text[search.text.Length - 1];
		bool endsAsWord = lastChar == '_' || char.IsLetterOrDigit(lastChar);
		
		int skipThisWord = search.text.IndexOf(firstChar.ToString(), 1, comparison);
		if (skipThisWord < 0)
			skipThisWord = search.text.Length;
		
		var l = 0;
		var c = 0;
		while (l < lines.Count)
		{
			var line = lines[l];
			
			if (c > line.Length - length)
			{
				c = 0;
				++l;
				continue;
			}
			
			c = line.IndexOf(search.text, c, comparison);
			if (c < 0)
			{
				c = 0;
				++l;
				continue;
			}
			
			if (search.matchWord)
			{
				if (startsAsWord && c > 0)
				{
					char prevChar = line[c - 1];
					if (prevChar == '_' || char.IsLetterOrDigit(prevChar))
					{
						c += skipThisWord;
						continue;
					}
				}
				if (endsAsWord && c + length < line.Length)
				{
					char nextChar = line[c + length];
					if (nextChar == '_' || char.IsLetterOrDigit(nextChar))
					{
						c += skipThisWord;
						continue;
					}
				}
			}
			
			yield return new TextPosition(l, c);
			c += length;
		}
	}
	
	public static bool ContainsWordsSequence(string assetPath, params string[] words)
	{
		try
		{
			var lines = File.ReadAllLines(assetPath);
			var l = 0;
			var w = 0;
			var s = 0;
			while (l < lines.Length)
			{
				if (s > lines[l].Length - words[0].Length)
				{
					s = 0;
					++l;
					continue;
				}
				
				s = lines[l].IndexOf(words[0], s, System.StringComparison.Ordinal);
				if (s < 0)
				{
					s = 0;
					++l;
					continue;
				}
				
				if (s > 0)
				{
					var c = lines[l][s - 1];
					if (c == '_' || char.IsLetterOrDigit(c))
					{
						s += words[0].Length;
						continue;
					}
				}
				
				s += words[0].Length;
				if (s < lines[l].Length)
				{
					if (words[1] != "(")
					{
						var c = lines[l][s];
						s++;
						if (c != ' ' && c != '\t')
							continue;
					}
				}
				else
				{
					s = 0;
					++l;
					if (l == lines.Length)
						break;
				}
				
				w = 1;
				while (w < words.Length)
				{
					// Skip additional whitespaces
					while (s < lines[l].Length)
					{
						var c = lines[l][s];
						if (c == ' ' || c == '\t')
							++s;
						else
							break;
					}
					
					if (s == lines[l].Length)
					{
						s = 0;
						++l;
						if (l == lines.Length)
							break;
						continue;
					}
					
					if (!lines[l].Substring(s).StartsWith(words[w], System.StringComparison.Ordinal))
					{
						w = 0;
						break;
					}
					
					s += words[w].Length;
					if (s < lines[l].Length && words[w] != "(")
					{
						var c = lines[l][s];
						if (c == '_' || char.IsLetterOrDigit(c))
						{
							w = 0;
							break;
						}
					}
					
					++w;
				}
				
				if (w == words.Length)
				{
					return true;
				}
			}
		}
		catch (IOException e)
		{
			Debug.LogError(e);
		}
		return false;
	}
	
	public static void Reset()
	{
		if (assets != null)
			assets.Clear();
	}
	
	public static List<string> FindAllTextAssets()
	{
		var hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
		hierarchyProperty.SetSearchFilter("t:TextAsset", 0);
		hierarchyProperty.Reset();
		List<string> list = new List<string>();
		while (hierarchyProperty.Next(null))
			list.Add(hierarchyProperty.guid);
		return list;
	}
	
	public static void FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly assemblyId)
	{
		var editor = false;
		var firstPass = false;
		var pattern = "";
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptFirstPass:
		case AssemblyDefinition.UnityAssembly.BooFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
			firstPass = true;
			break;
		}
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharp:
		case AssemblyDefinition.UnityAssembly.CSharpEditor:
			pattern = ".cs";
			break;
		case AssemblyDefinition.UnityAssembly.UnityScriptFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScript:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditor:
			pattern = ".js";
			break;
		case AssemblyDefinition.UnityAssembly.BooFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.Boo:
		case AssemblyDefinition.UnityAssembly.BooEditor:
			pattern = ".boo";
			break;
		}
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditor:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditor:
		case AssemblyDefinition.UnityAssembly.BooEditor:
			editor = true;
			break;
		}
		
		//var scripts = FindAssets("t:MonoScript");
		var scripts = Directory.GetFiles("Assets", "*" + pattern, SearchOption.AllDirectories);
		var count = scripts.Length;
		
		if (assets == null)
			assets = new List<string>(count);
		
		bool isUnity_5_2_1p4_orNewer = true;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		isUnity_5_2_1p4_orNewer =
			Application.unityVersion.StartsWith("5.2.1p") &&
			int.Parse(Application.unityVersion.Substring("5.2.1p".Length)) >= 4;
#endif
		
		for (var i = count; i --> 0; )
		{
			var path = scripts[i];
			scripts[i] = path = path.Replace('\\', '/');
			string lowerPath = path.ToLowerInvariant();
			
			if (path.Contains("/.") || lowerPath.StartsWith("assets/webplayertemplates/", System.StringComparison.Ordinal))
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			scripts[i] = AssetDatabase.AssetPathToGUID(scripts[i]);
			
			var extension = Path.GetExtension(lowerPath);
			if (extension != pattern)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			var isFirstPass = lowerPath.StartsWith("assets/standard assets/", System.StringComparison.Ordinal) ||
				lowerPath.StartsWith("assets/pro standard assets/", System.StringComparison.Ordinal) ||
				lowerPath.StartsWith("assets/plugins/", System.StringComparison.Ordinal);
			if (firstPass != isFirstPass)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			var isEditor = false;
			if (isFirstPass && !isUnity_5_2_1p4_orNewer)
				isEditor = lowerPath.StartsWith("assets/plugins/editor/", System.StringComparison.Ordinal) ||
					lowerPath.StartsWith("assets/standard assets/editor/", System.StringComparison.Ordinal) ||
					lowerPath.StartsWith("assets/pro standard assets/editor/", System.StringComparison.Ordinal);
			else
				isEditor = lowerPath.Contains("/editor/");
			if (editor != isEditor)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			assets.Add(scripts[i]);
		}
		//var joined = string.Join(", ", scripts, 0, count);
		//Debug.Log(joined);
	}
}
	
}
