#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ClearConsole : Editor {
    [MenuItem("Tools/ClearConsole %&v")]
    public static void CustomClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif
