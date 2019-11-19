#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneEditor : Editor
{

    [MenuItem("Project/Open Scene/Scene 0 %&q", false, 0)]
    public static void OpenScene_0()
    {
        OpenScene("0_Scene_SplashScreen");
    }

    [MenuItem("Project/Open Scene/Scene 1 %&w", false, 1)]
    public static void OpenScene_1()
    {
        OpenScene("1_Scene_Main");
    }

    //[MenuItem("Project/Open Scene/GamePlay %&e", false, 2)]
    //public static void OpenGamePlaySCene()
    //{
    //    OpenScene("GamePlay");
    //}

    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
        }
    }

    private static void OpenNewScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/New/" + sceneName + ".unity");
        }
    }
}

#endif