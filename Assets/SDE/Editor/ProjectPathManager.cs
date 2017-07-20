using System.IO;
using UnityEngine;
using UnityEditor;

/*
  ProjectPathManager just holds the currently used master path head of the project. That ways when saving
  build files, grabbing editor resources, etc. from different projects, the individual scripts don't need
  to be updated.
*/
public static class ProjectPathManager {
	
	private static string _basePath = "";
	
	// returns the absolute path to the root of the StoryEditor project
	public static string BasePath {
		get {
			if (string.IsNullOrEmpty(_basePath)) {
				CalculateBasePath();
			}
			
			return _basePath;
		}
	}
	
	private static string _resourcePath = "";
	public static string ResourcePath {
		get {
			if (string.IsNullOrEmpty(_resourcePath)) {
				CalculateResourcePath();
			}
			
			return _resourcePath;
		}
	}
	
	static ProjectPathManager() {
		if (string.IsNullOrEmpty(_basePath)) {
			CalculateBasePath();
		}
		
		if (string.IsNullOrEmpty(_resourcePath)) {
			CalculateResourcePath();
		}
	}
	
	private static void CalculateBasePath() {
		string projectPath;
		
		projectPath = Directory.GetFiles(Application.dataPath, "ProjectPathManager.cs", SearchOption.AllDirectories)[0];
		
		// normalize the filepath
		projectPath = projectPath.Replace('\\', '/');
		// remove the file and it's directory from the filepath
		projectPath = projectPath.Replace("/Editor/ProjectPathManager.cs", "");
		
		_basePath = projectPath;
		
		Debug.Log("calculated StoryEditor project base path: " + _basePath);
	}
	
	private static void CalculateResourcePath() {
		// make sure that the base path has been calculated already
		if (string.IsNullOrEmpty(_basePath)) {
			CalculateBasePath();
		}
		
		// split the path on each directory
		string[] splitPath = _basePath.Split('/');
		
		// locate the "Asset" string and build from there
		int assetIndex = -1;
		for (int i = 0; i < splitPath.Length; i++) {
			if (splitPath[i] == "Assets") {
				assetIndex = i;
			}
		}
		
		if (assetIndex < 0) {
			Debug.Log("PROJECT PATH ERROR: couldn't calculate resource path from base!");
		} else {
			_resourcePath = "";
			for (int i = assetIndex; i < splitPath.Length; i++) {
				_resourcePath += splitPath[i] + '/';
			}
			_resourcePath += "Editor/Resources";
		}
		
		Debug.Log("calculated StoryEditor resource path: " + _resourcePath);
	}
}
