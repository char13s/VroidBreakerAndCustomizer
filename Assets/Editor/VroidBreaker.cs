using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]

public class VroidBreaker : EditorWindow
{
    [MenuItem("Tools/Vroid Breaker")]
    public static void ShowWindow() {
        GetWindow<VroidBreaker>("Example");
    }
    private void OnGUI() {
        if (GUILayout.Button("Break em!")) {
            TakeInCharacter();
        }
    }
    public void TakeInCharacter() {
        foreach (GameObject go in Selection.gameObjects) {
            Debug.Log(go.name);
            ScanObject(go);
        }
    }
    private void ScanObject(GameObject go) {
        string mainName = go.name;
        GameObject root = null;
        GameObject hair = null;
        GameObject face = null;
        GameObject body = null;
        if (go.transform.Find("Root"))
            root = go.transform.Find("Root").gameObject;
        if (go.transform.Find("Hair"))
            hair = go.transform.Find("Hair").gameObject;
        if (go.transform.Find("Face"))
            face = go.transform.Find("Face").gameObject;
        if (go.transform.Find("Body"))
            body = go.transform.Find("Body").gameObject;
        ///---------------------------- might need to come back and save Secondary spring colliders with meshes---------------------------------------------
        //GameObject secondary = go.transform.Find("Secondary").gameObject;    
        if (hair != null) {
            GameObject newHair = new GameObject();
        newHair.name = mainName + "'s Hair";
            Instantiate(root, newHair.transform);
            Instantiate(hair, newHair.transform);
            PrefabThese(newHair);
        }
        if (face != null) {
            GameObject newFace = new GameObject();
        newFace.name = mainName + "'s Face";
            Instantiate(root, newFace.transform);
            Instantiate(face, newFace.transform);
            PrefabThese(newFace);
        }
        SplitBody(root, body);
    }
    private void PrefabThese(GameObject go) {
        if (!Directory.Exists("Assets/Resources/MeshParts"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        string localPath = WhereToGo(go.name);
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        PrefabUtility.SaveAsPrefabAsset(go, localPath);
        DestroyImmediate(go);
    }
    private string WhereToGo(string name) {

        if (name.Contains("Shoe")) {
            return "Assets/Resources/MeshParts/Shoes/" + name + ".prefab";
        }
        else if (name.Contains("Top")) {
            return "Assets/Resources/MeshParts/Tops/" + name + ".prefab";
        }
        else if (name.Contains("Bottom")) {
            return "Assets/Resources/MeshParts/Bottoms/" + name + ".prefab";
        }
        else if (name.Contains("Face")) {
            return "Assets/Resources/MeshParts/Faces/" + name + ".prefab";
        }
        else {
            return "Assets/Resources/MeshParts/Hairs/" + name + ".prefab";
        }
    }
    private void SplitBody(GameObject root, GameObject body) {
        Material invisi = Resources.Load("Invisi") as Material;
        bool createTop = false;
        bool createBottom = false;
        bool createShoes = false;
        Debug.Log(invisi);
        List<Material> shoeMats = new List<Material>();
        List<Material> topsMats = new List<Material>();
        List<Material> bottomsMats = new List<Material>();
        Material[] gameObjectMats = new Material[body.GetComponent<SkinnedMeshRenderer>().sharedMaterials.Length];
        body.GetComponent<SkinnedMeshRenderer>().sharedMaterials.CopyTo(gameObjectMats, 0);
        foreach (Material mat in gameObjectMats) {
            if (mat.name.Contains("Shoe")) {
                shoeMats.Add(mat);
                createShoes = true;
            }
            else {
                shoeMats.Add(invisi);
            }
            if (mat.name.Contains("Bottom")) {
                bottomsMats.Add(mat);
                createBottom = true;
            }
            else {
                bottomsMats.Add(invisi);
            }
            if (mat.name.Contains("Top") || mat.name.Contains("Onepiece")) {
                topsMats.Add(mat);
                createTop = true;
            }
            else {
                topsMats.Add(invisi);
            }
        }
        if (createTop) {
            GameObject Top = new GameObject();
            Top.name = "Tops";
            Instantiate(root, Top.transform);
            GameObject top = Instantiate(body, Top.transform);
            top.GetComponent<SkinnedMeshRenderer>().sharedMaterials = topsMats.ToArray();
            PrefabThese(Top);
        }
        if (createBottom) {
            GameObject Bottom = new GameObject();
            Bottom.name = "Bottoms";
            Instantiate(root, Bottom.transform);
            GameObject bottom = Instantiate(body, Bottom.transform);
            bottom.GetComponent<SkinnedMeshRenderer>().sharedMaterials = bottomsMats.ToArray();
            PrefabThese(Bottom);
        }
        if (createShoes) {
            GameObject Shoe = new GameObject();
            Shoe.name = "Shoes";
            Instantiate(root, Shoe.transform);
            GameObject shoe = Instantiate(body, Shoe.transform);
            shoe.GetComponent<SkinnedMeshRenderer>().sharedMaterials = shoeMats.ToArray();
            PrefabThese(Shoe);
        }
    }
}
