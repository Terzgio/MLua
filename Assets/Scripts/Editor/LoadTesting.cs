using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class LoadTesting : MonoBehaviour
{
    public static void loadScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TestingScene.unity");
        Debug.Log("Ready for testing.");
        

    }
}
