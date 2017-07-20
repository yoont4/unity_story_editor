using UnityEngine;

/*
  ProjectPathManager just holds the currently used master path head of the project. That ways when saving
  build files, grabbing editor resources, etc. from different projects, the individual scripts don't need
  to be updated.
*/
public static class ProjectPathManager {
	
	public static string BasePath {
		get {
			return Application.dataPath + "/SDE";
		}
	}
	
	public static string ResourcePath = "Assets/SDE/Editor/Resources";
}
