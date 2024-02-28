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

            Mesh m=new Mesh();
            
            //go.GetComponent<SkinnedMeshRenderer>().BakeMesh(m);
            m=go.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            if (AssetDatabase.Contains(m)) {
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(m), "Assets/Prefabs");
            }
            else { 
            AssetDatabase.CreateAsset(m, "Assets/NewMesh.asset");
           }

            GameObject newMesh=new GameObject();
            newMesh.AddComponent<MeshRenderer>();
            newMesh.AddComponent<MeshFilter>().mesh = m;

            PrefabThese(newMesh);
            //TakeInCharacter(go);
        }
    }
    void makeSaveableMesh() { 
    
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
