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
            Debug.Log(go.name);
            ScanObject(go);
        }
    }
    private void ScanObject(GameObject go) {
        //mainBody = go;
        go.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Pose") as RuntimeAnimatorController;
        go.GetComponent<Animator>().applyRootMotion = true;
        string mainName = go.name;
        //GameObject root = null;
        GameObject hair = Instantiate(go);
        GameObject face = Instantiate(go);
        GameObject body = Instantiate(go);
        BlankTheObjects(hair, "Hair");
        BlankTheObjects(face, "Face");
        BlankTheObjects(body, "Body");
        SplitBody(go, body);
        PrefabThese(hair);
        PrefabThese(face);
        //DestroyImmediate(body);
        //if (go.transform.Find("Root"))
        //    root = go.transform.Find("Root").gameObject;
        //if (go.transform.Find("Hair"))
        //    hair = go.transform.Find("Hair").gameObject;
        //if (go.transform.Find("Face"))
        //    face = go.transform.Find("Face").gameObject;
        //if (go.transform.Find("Body"))
        //    body = go.transform.Find("Body").gameObject;
        ///---------------------------- might need to come back and save Secondary spring colliders with meshes---------------------------------------------
        //GameObject secondary = go.transform.Find("Secondary").gameObject;    
        //root.name = "Root";
        //if (hair != null) {
        //    GameObject newHair = new GameObject();
        //    newHair.name = mainName + "'s Hair";
        //    //Instantiate(root, newHair.transform);
        //    //Instantiate(hair, newHair.transform);
        //    PrefabThese(newHair);
        //}
        //if (face != null) {
        //    GameObject newFace = new GameObject();
        //    newFace.name = mainName + "'s Face";
        //    //Instantiate(root, newFace.transform);
        //    //Instantiate(face, newFace.transform);
        //    PrefabThese(newFace);
        //}
        //
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
        Debug.Log(invisi);
        List<Material> shoeMats = new List<Material>();
        List<Material> topsMats = new List<Material>();
        List<Material> bottomsMats = new List<Material>();

        //body.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBoneWeights(bones);
        Material[] gameObjectMats = new Material[body.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials.Length];
        body.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials.CopyTo(gameObjectMats, 0);
        //body.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetUVs(0,);
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

             Mesh mesh = new Mesh();//
           SkinnedMeshRenderer skinnedMeshRenderer = Top.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            
            mesh.vertices=skinnedMeshRenderer.sharedMesh.vertices;
            //Instantiate(mesh);
            topsMats.RemoveAt(0);
            topsMats.Insert(0,main);

            //Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = topsMats.ToArray();
            Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0] = main;
            
            Top.name = "Tops";
            Humanoid rig = Top.GetComponent<Humanoid>();
            //EditTexture(Top);
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
            //Texture modifiedTexture = Instantiate(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0].mainTexture);
            
            Vector3 center = Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.center;
            GameObject point = Resources.Load("Sphere") as GameObject;
            GameObject points = new GameObject();
            box.isTrigger = true;
            Vector3[] verts=mesh.vertices;
            Debug.Log("Set points position");
            for (int i=0;i< verts.Length;i++) {
                if (!box.bounds.Contains(verts[i])&&mesh.bounds.Contains(verts[i])) {
                    //Vector3 normal = Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.normals[i];
                    //verts[i] += normal * 0.5f;
                    //Vector3 hidePosition = center + (Vector3.forward * (maxDistance + 1f));
                    //verts[i] = hidePosition;
                    verts[i] = rig.Hips.position;
                    //normals[i] = -Camera.main.transform.forward;
                    //verts[i]= new Vector3(int.MaxValue-1, int.MaxValue - 1, int.MaxValue - 1);
                     
                    //GameObject pointinst =Instantiate(point, verts[i].ToVector3(), Quaternion.identity);
                    //pointinst.transform.SetParent(points.transform);
                }
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            
            if (mesh.GetAllBoneWeights().Length > 0) {
                Debug.Log("AHHHHHHHHHHHHHH");
            }
            mesh.bindposes = Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBindposes().ToArray();
            if (mesh.GetBindposes().Length > 0) {
                Debug.Log("OUUU OUUU");
            }

            if (mesh.blendShapeCount > 0) {
                Debug.Log("PAY UP PAY UP");
            }
            skinnedMeshRenderer.sharedMesh = mesh;
                skinnedMeshRenderer.sharedMesh.SetBoneWeights(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            Instantiate(skinnedMeshRenderer.sharedMesh);
            //Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.RecalculateBounds();
            Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = topsMats.ToArray();
            //Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0].mainTexture = modifiedTexture;
            //;
            //Top.AddComponent<Animator>().runtimeAnimatorController = Resources.Load("Pose") as RuntimeAnimatorController;
            //Top.GetComponent<Animator>().avatar = Resources.Load("Tops.avatar") as Avatar;
            //
            ////string hips=rig.Hips.name;
            //rig.Hips = root.transform.Find("J_Bip_C_Hips");
            //rig.Spine = root.transform.Find("J_Bip_C_Hips/J_Bip_C_Spine");
            //rig.RightUpperLeg = root.transform.Find("J_Bip_C_Hips/J_Bip_R_UpperLeg");
            //rig.LeftUpperLeg = root.transform.Find("J_Bip_C_Hips/J_Bip_L_UpperLeg");
            //rig.LeftShoulder = root.transform.Find("J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder");
            //rig.RightShoulder = root.transform.Find("J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder");
            ////rig.AssignBonesFromAnimator();
            ////HumanBodyBones bone1 =(HumanBodyBones) mainBody.GetComponent<Humanoid>().BoneMap;
            ////rig.AssignBones(bone1,rig.Hips);//= ;
            ////rig.AssignBonesFromAnimator();
            //// rig.CreateAvatar();
            //
            //Instantiate(root, Top.transform);
            //GameObject top = Instantiate(body, Top.transform);
            //top.name = "Body";
            //top.GetComponent<SkinnedMeshRenderer>().rootBone = root.transform;
            ////bones = top.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(); //= mainBody.GetComponent<SkinnedMeshRenderer>().sharedMesh.get
            ////top.GetComponent<SkinnedMeshRenderer>().sharedMesh.SetBoneWeights(bones, body.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            //
            PrefabThese(Top);
        }
        if (createBottom) {
            GameObject Bottom = Instantiate(body);
            Mesh mesh = new Mesh();//
            SkinnedMeshRenderer skinnedMeshRenderer = Bottom.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            Humanoid rig = Bottom.GetComponent<Humanoid>();
            //EditTexture(Top);
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
            //Texture modifiedTexture = Instantiate(Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0].mainTexture);

            Vector3 center = Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.center;
            GameObject point = Resources.Load("Sphere") as GameObject;
            GameObject points = new GameObject();
            box.isTrigger = true;
            float maxDistance = 0f;
            Vector3[] verts = mesh.vertices;
            Debug.Log("Set points position");
            for (int i = 0; i < verts.Length; i++) {
                float maskValue = 0;
                if (box.bounds.Contains(verts[i]) && !mesh.bounds.Contains(verts[i])) {
                    float distance = Vector3.Distance(center, verts[1]);
                    if (distance > maxDistance) {
                        maxDistance = distance;
                    }
                    //Vector3 normal = Top.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.normals[i];
                    //verts[i] += normal * 0.5f;
                    //Vector3 hidePosition = center + (Vector3.forward * (maxDistance + 1f));
                    //verts[i] = hidePosition;
                    verts[i] = rig.Hips.position;
                    //normals[i] = -Camera.main.transform.forward;
                    //verts[i]= new Vector3(int.MaxValue-1, int.MaxValue - 1, int.MaxValue - 1);

                    //GameObject pointinst =Instantiate(point, verts[i].ToVector3(), Quaternion.identity);
                    //pointinst.transform.SetParent(points.transform);
                }

                //uvs[i] = new Vector2(maskValue, 1f);
            }
            mesh.vertices = verts;
            mesh.SetBoneWeights(Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(),Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());

            if (mesh.GetAllBoneWeights().Length > 0) {
                Debug.Log("AHHHHHHHHHHHHHH");
            }
            mesh.bindposes = Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBindposes().ToArray();
            if (mesh.GetBindposes().Length > 0) {
                Debug.Log("OUUU OUUU");
            }

            if (mesh.blendShapeCount > 0) {
                Debug.Log("PAY UP PAY UP");
            }
            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMesh.SetBoneWeights(Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
            Instantiate(skinnedMeshRenderer.sharedMesh);
            mesh.vertices = skinnedMeshRenderer.sharedMesh.vertices;
            bottomsMats.RemoveAt(0);
            bottomsMats.Insert(0, main);
            Bottom.name = "Bottoms";
            Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = bottomsMats.ToArray();
            Bottom.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0] = main;

            //
            //Instantiate(root, Bottom.transform);
            //GameObject bottom = Instantiate(body, Bottom.transform);
            //bottom.name = "Body";
            //
            PrefabThese(Bottom);
        }
        if (createShoes) {
            GameObject Shoe = Instantiate(body);
            //shoeMats.RemoveAt(0);
            //shoeMats.Insert(0, main);
            Shoe.name = "Shoes";
            Shoe.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = shoeMats.ToArray();
            //Shoe.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0] = main;
            //
            //Instantiate(root, Shoe.transform);
            //GameObject shoe = Instantiate(body, Shoe.transform);
            //shoe.name = "Body";
            //
            PrefabThese(Shoe);
        }
        DestroyImmediate(body);
    }
    /*private void EditTexture(Texture2D mainTexture,GameObject go) {
        Mesh mesh = go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        Texture2D modifiedTexture = Instantiate(mainTexture);
        Vector3[] vertices= mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            // Example mask based on vertex position along Y-axis
            if()
            float maskValue = vertices[i].y > 0.5f ? 1f : 0f;

            // Apply mask to texture coordinates
            uvs[i] = new Vector2(maskValue, 0f); // Adjust UVs as per your mask
        }

        mesh.uv = uvs;
        go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0].mainTexture = modifiedTexture;
    }*/
}
