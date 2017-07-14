using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

/*
  GlobalFlags holds the list of global flags being used in this project
*/
[InitializeOnLoad]
public static class GlobalFlags {

	public static readonly string[] flags =
	{
		"write flags here",
		"like so"
	};
	
	static GlobalFlags() {
		CheckFlags();
		ExportFlags();
	}
	
	private static void CheckFlags() {
		foreach (string flag in flags) {
			if (flag.Length > ToggleMenu.MAX_TEXT_LENGTH) {
				EditorApplication.update += () => {throw new UnityException("GLOBAL FLAG LONGER THAN MAX TEXT LENGTH (" + ToggleMenu.MAX_TEXT_LENGTH + ')');};
			}
		}
	}
	
	private static void ExportFlags() {
		string path = Application.dataPath + "/SDE/_GlobalFlagBuild.cs";
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		
		// build the custom class string
		string output = 
			"using System.Collections;\n" +
			"using System.Collections.Generic;\n\n" +
			"// THIS IS A PROCEDURALLY GENERATED FILE!\n" +
			"// DO NOT EDIT, MODIFY, OR WRITE TO EXCEPT TO WHEN CHECKING FLAGS!\n" +
			"public static class GlobalFlagBuild {\n" +
			"    public static Dictionary<string, bool> flags;\n\n" +
			"    static GlobalFlagBuild() {\n" +
			"        flags = new Dictionary<string, bool>() {\n";
		
		for (int i = 0; i < flags.Length; i++) {
			output += 
				"            {\"" + flags[i] + "\", false}";
			if (i == flags.Length - 1) {
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
	}
}
