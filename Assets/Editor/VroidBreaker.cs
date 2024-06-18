using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UniHumanoid;
using Unity.Collections;

[ExecuteInEditMode]

public class VroidBreaker : EditorWindow
{
    GameObject mainBody;
    bool hair;
    bool face;
    bool top;
    bool bottoms;
    bool shoes;
    [MenuItem("Tools/Vroid Breaker")]
    public static void ShowWindow() {
        GetWindow<VroidBreaker>("Example");
    }
    private void OnGUI() {
        //bool hair=false;
        hair = GUILayout.Toggle(hair, "Export Hair?");
        face = GUILayout.Toggle(face, "Export Face?");
        top = GUILayout.Toggle(top, "Export Top Clothes?");
        bottoms = GUILayout.Toggle(bottoms, "Export Bottom Clothes?");
        shoes = GUILayout.Toggle(shoes, "Export Shoes?");
        if (GUILayout.Button("Break em!")) {
            TakeInCharacter();
        }
    }
    public void TakeInCharacter() {
        foreach (GameObject go in Selection.gameObjects) {
            ScanObject(go);
        }
    }
    private void CreateFolders(string name) {
        string directory;
        if (!Directory.Exists("Assets/Resources/" + name)) {
            AssetDatabase.CreateFolder("Assets/Resources/", name);
            AssetDatabase.CreateFolder("Assets/Resources/" + name, "MeshParts");
        }
        directory = "Assets/Resources/" + name + "/MeshParts";
        if (!Directory.Exists(directory + "/Tops"))
            AssetDatabase.CreateFolder(directory, "Tops");
        if (!Directory.Exists(directory + "/Faces"))
            AssetDatabase.CreateFolder(directory, "Faces");
        if (!Directory.Exists(directory + "/Bottoms"))
            AssetDatabase.CreateFolder(directory, "Bottoms");
        if (!Directory.Exists(directory + "/Shoes"))
            AssetDatabase.CreateFolder(directory, "Shoes");
        if (!Directory.Exists(directory + "/Hairs"))
            AssetDatabase.CreateFolder(directory, "Hairs");
    }
    private void ScanObject(GameObject go) {
        CreateFolders(go.name);
        Animator anim = go.GetComponent<Animator>();
        anim.enabled = false;
        if (hair) {
            GameObject hair;
            if (go.transform.Find("Hair")) {
                hair = Instantiate(go);
                Mesh mesh = new Mesh();
                BlankTheObjects(hair, "Hair");
                DestoryOffParts(hair);
                SkinnedMeshRenderer skinnedMeshRenderer = hair.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.BakeMesh(mesh);
                mesh.SetBoneWeights(skinnedMeshRenderer.sharedMesh.GetBonesPerVertex(), skinnedMeshRenderer.sharedMesh.GetAllBoneWeights());
                mesh.bindposes = skinnedMeshRenderer.sharedMesh.GetBindposes().ToArray();
                hair.name = "Hair";
                if (AssetDatabase.Contains(mesh)) {
                    AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(mesh), "Assets/Resources/MeshDataAssets/" + hair.name + ".asset");
                }
                else {
                    string localPath = "Assets/Resources/MeshDataAssets/NewMesh" + hair.name + ".asset";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                    AssetDatabase.CreateAsset(mesh, localPath);
                }
                PrefabThese(hair);
            }
        }
        if (face) {
            GameObject face = Instantiate(go);
            face.name = "Face";
            BlankTheObjects(face, "Face");
            DestoryOffParts(face);
            PrefabThese(face);
        }
        GameObject body = Instantiate(go);
        BlankTheObjects(body, "Body");
        DestoryOffParts(body);
        SplitBody(go, body);
    }
    private void DestoryOffParts(GameObject go) {
        foreach (Transform got in go.transform.GetChildren()) {
            if (!got.gameObject.activeSelf) {
                DestroyImmediate(got.gameObject);
            }
        }
    }
    private void BlankTheObjects(GameObject go, string desiredPart) {
        if (go.transform.Find("Hair"))
            go.transform.Find("Hair").gameObject.SetActive(false);
        if (go.transform.Find("Face"))
            go.transform.Find("Face").gameObject.SetActive(false);
        if (go.transform.Find("Body"))
            go.transform.Find("Body").gameObject.SetActive(false);
        if (go.transform.Find(desiredPart))
            go.transform.Find(desiredPart).gameObject.SetActive(true);
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
        Material main = null;
        List<Material> shoeMats = new List<Material>();
        List<Material> topsMats = new List<Material>();
        List<Material> bottomsMats = new List<Material>();
        Material[] gameObjectMats = new Material[body.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials.Length];
        body.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials.CopyTo(gameObjectMats, 0);
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
            if (mat.name.Contains("Body")) {
                main = mat;
            }
        }
        if (createTop&&top) {
            GameObject Top = Instantiate(body);
            Mesh mesh = new Mesh();
            SkinnedMeshRenderer skinnedMeshRenderer = Top.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            mesh.vertices = skinnedMeshRenderer.sharedMesh.vertices;
            topsMats.RemoveAt(0);
            topsMats.Insert(0, main);
            skinnedMeshRenderer.sharedMaterials[0] = main;
            Top.name = "Tops";
            Humanoid rig = root.GetComponent<Humanoid>();
            GameObject boxHolder = new GameObject();
            BoxCollider box = boxHolder.AddComponent<BoxCollider>();
            Vector3 right = rig.RightMiddleProximal.position;
            Vector3 left = rig.LeftMiddleProximal.position;
            Vector3 up = rig.Head.position;
            Vector3 down = rig.Hips.position;
            float x = Vector3.Distance(left, right);
            float y = Vector3.Distance(up, down);
            box.size = new Vector3(x * 1.4f, y * 1.75f, 2);
            box.center = rig.Neck.position;
            box.isTrigger = true;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++) {
                if (!box.bounds.Contains(verts[i]) && mesh.bounds.Contains(verts[i])) {
                    verts[i] = rig.Hips.position;
                }
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(skinnedMeshRenderer.sharedMesh.GetBonesPerVertex(), skinnedMeshRenderer.sharedMesh.GetAllBoneWeights());
            mesh.bindposes = skinnedMeshRenderer.sharedMesh.GetBindposes().ToArray();
            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMesh.SetBoneWeights(skinnedMeshRenderer.sharedMesh.GetBonesPerVertex(), skinnedMeshRenderer.sharedMesh.GetAllBoneWeights());
            Instantiate(skinnedMeshRenderer.sharedMesh);
            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMaterials = topsMats.ToArray();
            //Mesh m = Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            if (AssetDatabase.Contains(mesh)) {
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(mesh), "Assets/Resources/MeshDataAssets");
            }
            else {
                string localPath = "Assets/Resources/MeshDataAssets/NewMesh" + Top.name + ".asset";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                //PrefabUtility.SaveAsPrefabAsset(go, localPath);
                AssetDatabase.CreateAsset(mesh, localPath);
            }
            PrefabThese(Top);
            DestroyImmediate(boxHolder);
        }
        if (createBottom&&bottoms) {
            GameObject Bottom = Instantiate(body);
            Mesh mesh = new Mesh();
            SkinnedMeshRenderer skinnedMeshRenderer = Bottom.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            skinnedMeshRenderer.sharedMesh.ClearBlendShapes();
            Humanoid rig = root.GetComponent<Humanoid>();
            GameObject boxHolder = new GameObject();
            BoxCollider box = boxHolder.AddComponent<BoxCollider>();
            Vector3 right = rig.RightMiddleProximal.position;
            Vector3 left = rig.LeftMiddleProximal.position;
            Vector3 up = rig.Head.position + new Vector3(0, 0.1f, 0);
            Vector3 down = rig.Hips.position;
            float x = Vector3.Distance(left, right);
            float y = Vector3.Distance(up, down);
            box.size = new Vector3(x * 1.6f, y * 0.8f, 2.2f);
            box.center = rig.Neck.position + new Vector3(0, 0.1f, 0);
            box.isTrigger = true;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++) {
                if (box.bounds.Contains(verts[i]) && !mesh.bounds.Contains(verts[i])) {
                    verts[i] = rig.Hips.position;
                }
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(skinnedMeshRenderer.sharedMesh.GetBonesPerVertex(), skinnedMeshRenderer.sharedMesh.GetAllBoneWeights());
            mesh.bindposes = skinnedMeshRenderer.sharedMesh.GetBindposes().ToArray();
            skinnedMeshRenderer.sharedMesh = mesh;
            mesh.vertices = skinnedMeshRenderer.sharedMesh.vertices;
            skinnedMeshRenderer.sharedMesh.ClearBlendShapes();
            Instantiate(skinnedMeshRenderer.sharedMesh);
            bottomsMats.RemoveAt(0);
            bottomsMats.Insert(0, main);
            Bottom.name = "Bottoms";
            skinnedMeshRenderer.sharedMaterials = bottomsMats.ToArray();
            skinnedMeshRenderer.sharedMaterials[0] = main;
            if (AssetDatabase.Contains(mesh)) {
            }
            else {
                string localPath = "Assets/Resources/MeshDataAssets/NewMesh" + Bottom.name + ".asset";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                AssetDatabase.CreateAsset(mesh, localPath);
            }
            PrefabThese(Bottom);
            DestroyImmediate(boxHolder);
        }
        if (createShoes&&shoes) {
            GameObject Shoe = Instantiate(body);
            Shoe.name = "Shoes";
            Shoe.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = shoeMats.ToArray();
            PrefabThese(Shoe);
        }
        DestroyImmediate(body);
    }
}
