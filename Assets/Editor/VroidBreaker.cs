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
            ScanObject(go);
        }
    }
    private void ScanObject(GameObject go) {
        go.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Pose") as RuntimeAnimatorController;
        go.GetComponent<Animator>().applyRootMotion = true;
        GameObject hair = Instantiate(go);
        GameObject face = Instantiate(go);
        GameObject body = Instantiate(go);
        BlankTheObjects(hair, "Hair");
        BlankTheObjects(face, "Face");
        BlankTheObjects(body, "Body");
        SplitBody(go, body);
        PrefabThese(hair);
        PrefabThese(face);
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
        Material main=null;
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
        if (createTop) {
            GameObject Top = Instantiate(body);
            Mesh mesh = new Mesh();
            SkinnedMeshRenderer skinnedMeshRenderer = Top.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            mesh.vertices=skinnedMeshRenderer.sharedMesh.vertices;
            topsMats.RemoveAt(0);
            topsMats.Insert(0,main);
            Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0] = main;
            Top.name = "Tops";
            Humanoid rig = Top.GetComponent<Humanoid>();
            GameObject boxHolder = new GameObject();         
            BoxCollider box = boxHolder.AddComponent<BoxCollider>();
            Vector3 right =rig.RightMiddleProximal.position;
            Vector3 left = rig.LeftMiddleProximal.position;
            Vector3 up = rig.Head.position;
            Vector3 down = rig.Hips.position;
            float x = Vector3.Distance(left,right);
            float y = Vector3.Distance(up, down);
            box.size =new Vector3(x*1.4f,y*1.75f,2);
            box.center = rig.Neck.position;
            box.isTrigger = true;
            Vector3[] verts=mesh.vertices;
            for (int i=0;i< verts.Length;i++) {
                if (!box.bounds.Contains(verts[i])&&mesh.bounds.Contains(verts[i])) {
                    verts[i] = rig.Hips.position;
                }
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMesh.SetBoneWeights(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            Instantiate(skinnedMeshRenderer.sharedMesh);
            Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = topsMats.ToArray();
            PrefabThese(Top);
        }
        if (createBottom) {
            GameObject Bottom = Instantiate(body);
            Mesh mesh = new Mesh();
            SkinnedMeshRenderer skinnedMeshRenderer = Bottom.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            Humanoid rig = Bottom.GetComponent<Humanoid>();
            GameObject boxHolder = new GameObject();
            BoxCollider box = boxHolder.AddComponent<BoxCollider>();
            Vector3 right = rig.RightMiddleProximal.position;
            Vector3 left = rig.LeftMiddleProximal.position;
            Vector3 up = rig.Head.position+new Vector3(0,0.1f,0);
            Vector3 down = rig.Hips.position;
            float x = Vector3.Distance(left, right);
            float y = Vector3.Distance(up, down);
            box.size = new Vector3(x * 1.4f, y * 1f, 2);
            box.center = rig.Neck.position;
            Vector3 center = Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.center;
            box.isTrigger = true;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++) {
                if (box.bounds.Contains(verts[i]) && !mesh.bounds.Contains(verts[i])) {
                    verts[i] = rig.Hips.position;
                }
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(),Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            mesh.bindposes = Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBindposes().ToArray();
            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMesh.SetBoneWeights(Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            Instantiate(skinnedMeshRenderer.sharedMesh);
            mesh.vertices = skinnedMeshRenderer.sharedMesh.vertices;
            bottomsMats.RemoveAt(0);
            bottomsMats.Insert(0, main);
            Bottom.name = "Bottoms";
            Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = bottomsMats.ToArray();
            Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0] = main;
            PrefabThese(Bottom);
        }
        if (createShoes) {
            GameObject Shoe = Instantiate(body);
            Shoe.name = "Shoes";
            Shoe.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = shoeMats.ToArray();
            PrefabThese(Shoe);
        }
        DestroyImmediate(body);
    }
}
