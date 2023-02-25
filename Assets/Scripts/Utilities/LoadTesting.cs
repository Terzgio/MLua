using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LoadTesting : MonoBehaviour
{
    public static void loadScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/TestingScene.unity");
        Debug.Log("Ready for testing.");
        

    }
}
