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
	}
	
	private static void CheckFlags() {
		foreach (string flag in flags) {
			if (flag.Length > ToggleMenu.MAX_TEXT_LENGTH) {
				EditorApplication.update += () => {throw new UnityException("GLOBAL FLAG LONGER THAN MAX TEXT LENGTH (" + ToggleMenu.MAX_TEXT_LENGTH + ')');};
			}
		}
	}
}
