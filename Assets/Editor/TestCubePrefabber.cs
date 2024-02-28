using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TestCubePrefabber : EditorWindow
{
    [MenuItem("Tools/CubeCollector")]

    public static void ShowWindow() {
        GetWindow<TestCubePrefabber>("Example");
    }    private void OnGUI() {
        if (GUILayout.Button("Break em!")) {
            TakeInCharacter();
        }
    }
    public void TakeInCharacter() {
        foreach (GameObject go in Selection.gameObjects) {
            Debug.Log(go.name);
            PrefabThese(go);
            //TakeInCharacter(go);
        }
    }
    private void PrefabThese(GameObject go) {
        if (!Directory.Exists("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        string localPath = "Assets/Prefabs/" + go.name + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        PrefabUtility.SaveAsPrefabAsset(go, localPath);
        //Destroy(go,5);
    }
}
