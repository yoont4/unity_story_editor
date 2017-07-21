using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  GlobalFlags holds the list of global variables being used in this project
*/
[InitializeOnLoad]
public static class GlobalVariables {
	
	public static readonly string[] variables =
	{
		"write variables",
		"like so",
		"NOT flags",
		"value based"
	};
	
	static GlobalVariables() {
		CheckVariables();
		ExportVariables();
	}
	
	private static void CheckVariables() {
		// check if any strings are over the Max text length
		foreach (string variable in variables) {
			if (variable.Length > ToggleMenu.MAX_TEXT_LENGTH) {
				throw new UnityException("GLOBAL VARIABLE LONGER THAN MAX TEXT LENGTH (" + ToggleMenu.MAX_TEXT_LENGTH + "): " + variable);
			}
		}
		
		// check for duplicates
		HashSet<string> variableSet = new HashSet<string>();

		foreach (string variable in variables) {
			if (!variableSet.Add(variable)) {
				throw new UnityException("DUPLICATE GLOBAL VARIABLE FOUND: " + variable);
			}
		}
	}
	
	private static void ExportVariables() {
		string path = ProjectPathManager.BasePath + "/_GlobalVariablesBuild.cs";
		Debug.Log("exporting global variables to: " + path);
		
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		
		// build the custom class string
		string output = 
			"using System.Collections;\n" +
			"using System.Collections.Generic;\n\n" +
			"// THIS IS A PROCEDURALLY GENERATED FILE!\n" +
			"// DO NOT EDIT, MODIFY, OR WRITE TO EXCEPT TO WHEN CHECKING VARIABLES!\n" +
			"public static class GlobalVariableBuild {\n" +
			"    public static Dictionary<string, int> variables;\n\n" +
			"    static GlobalVariableBuild() {\n" +
			"        variables = new Dictionary<string, int>() {\n";
		
		for (int i = 0; i < variables.Length; i++) {
			output += 
				"            {\"" + variables[i] + "\", 0}";
			if (i == variables.Length - 1) {
				output += "\n";
			} else {
				output += ",\n";
			}
		}
		
		
		output += 
			"        };\n" +
			"    }\n" +
			"}";
		
		using (StreamWriter stream = new StreamWriter(path, false, encoding)) {
			stream.Write(output);
		}
		
		Debug.Log("global variables exported successfully.");
	}
}
