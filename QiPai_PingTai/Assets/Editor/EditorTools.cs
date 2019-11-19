using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools {

    [MenuItem("Tools/Change Scene 0 &%Q")]
    public static void ChangeScene_0()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/0_Scene_SplashScreen.unity");
    }

    [MenuItem("Tools/Change Scene 1 &%W")]
    public static void ChangeScene_1()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/1_Scene_Main.unity");
    }

}
