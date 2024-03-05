using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeController : MonoBehaviour
{
    List<GameObject> hairSets = new List<GameObject>();
    int hairSetsIndex;
    GameObject currentHair;
    List<GameObject> faceSets = new List<GameObject>();
    int faceSetsIndex;
    GameObject currentFace;
    List<GameObject> topSets = new List<GameObject>();
    int topSetsIndex;
    GameObject currentTop;
    List<GameObject> bottomSets = new List<GameObject>();
    int bottomSetsIndex;
    GameObject currentBottom;
    List<GameObject> shoeSets = new List<GameObject>();
    int shoeSetsIndex;
    GameObject currentShoe;
    [SerializeField] private GameObject hairTarget;
    [SerializeField] private GameObject faceTarget;
    [SerializeField] private GameObject topTarget;
    [SerializeField] private GameObject bottomTarget;
    [SerializeField] private GameObject shoeTarget;
    [SerializeField] private GameObject baseTarget;
    [SerializeField] private Transform newArmature;
    private SkinnedMeshRenderer[] skinnedMeshRenderersList;
    public int HairSetsIndex { get => hairSetsIndex; set => hairSetsIndex = Mathf.Clamp(value, 0, hairSets.Count); }
    public int FaceSetsIndex { get => faceSetsIndex; set => faceSetsIndex = Mathf.Clamp(value, 0, faceSets.Count); }
    public int TopSetsIndex { get => topSetsIndex; set => topSetsIndex = Mathf.Clamp(value, 0, topSets.Count); }
    public int BottomSetsIndex { get => bottomSetsIndex; set => bottomSetsIndex = Mathf.Clamp(value, 0, bottomSets.Count); }
    public int ShoeSetsIndex { get => shoeSetsIndex; set => shoeSetsIndex = Mathf.Clamp(value, 0, shoeSets.Count); }
    public GameObject CurrentHair { get => currentHair; set {  currentHair = value; SetHair(); } }
    public GameObject CurrentFace { get => currentFace; set {  currentFace = value; SetFace(); } }
    public GameObject CurrentTop { get => currentTop; set {  currentTop = value; SetTop(); } }
    public GameObject CurrentBottom { get => currentBottom; set { DestroyImmediate(currentBottom); currentBottom = value; SetBottom(); } }
    public GameObject CurrentShoe { get => currentShoe; set { DestroyImmediate(currentShoe); currentShoe = value; SetShoe(); } }

    private void Start() {
        GetSets();
        SetDefaults();
    }
    private void SetDefaults() {
        CurrentHair = hairSets[hairSetsIndex];
        CurrentFace = faceSets[faceSetsIndex];
        CurrentTop = topSets[topSetsIndex];
        CurrentBottom = bottomSets[bottomSetsIndex];
        CurrentShoe = shoeSets[shoeSetsIndex];
    }
    private void GetSets() {
        foreach (GameObject go in Resources.LoadAll("MeshParts/Tops")) {
            topSets.Add(go);
        }
        foreach (GameObject go in Resources.LoadAll("MeshParts/Bottoms")) {
            bottomSets.Add(go);
        }
        foreach (GameObject go in Resources.LoadAll("MeshParts/Hairs")) {
            hairSets.Add(go);
        }
        foreach (GameObject go in Resources.LoadAll("MeshParts/Faces")) {
            faceSets.Add(go);
        }
        foreach (GameObject go in Resources.LoadAll("MeshParts/Shoes")) {
            shoeSets.Add(go);
        }
    }
    private void SetHair() {
        if (hairTarget.transform.childCount > 0)
            DestroyImmediate(hairTarget.transform.GetChild(0).gameObject);
        //Instantiate(currentHair, );'
        Instantiate(currentHair,hairTarget.transform);
        hairTarget.transform.position = new Vector3(0,0,0);
        //top.transform.Find("secondary").gameObject.transform.SetParent(top.transform.Find("Hair"));
        //TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>());
    }
    private void SetFace() {
        if (faceTarget.transform.childCount > 0)
            DestroyImmediate(faceTarget.transform.GetChild(0).gameObject);
        GameObject top = Instantiate(currentFace, faceTarget.transform);
        top.name = "Face";
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>());
    }
    private void SetBottom() {
        if (bottomTarget.transform.childCount > 0)
            DestroyImmediate(bottomTarget.transform.GetChild(0).gameObject);
        GameObject top = Instantiate(currentBottom, bottomTarget.transform);
        top.name = "Bottom";
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>());
    }
    private void SetTop() {
        if (topTarget.transform.childCount > 0)
            DestroyImmediate(topTarget.transform.GetChild(0).gameObject);
        GameObject top = Instantiate(currentTop);
        top.name = "Top";
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>());

        //currentTop.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices= topTarget.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        //currentTop.GetComponent<SkinnedMeshRenderer>().rootBone = topTarget.GetComponent<SkinnedMeshRenderer>().rootBone;
        //currentTop.GetComponent<SkinnedMeshRenderer>().sharedMesh.SetBoneWeights(topTarget.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBonesPerVertex(), topTarget.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetAllBoneWeights());
        //currentTop.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes = topTarget.GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBindposes().ToArray();
    }
    private void SetShoe() {
        if (shoeTarget.transform.childCount > 0)
            DestroyImmediate(shoeTarget.transform.GetChild(0).gameObject);
        Instantiate(currentShoe, shoeTarget.transform);
    }
    public void CycleThruHair(int val) {
        HairSetsIndex += val;
        if (HairSetsIndex == hairSets.Count) {
            HairSetsIndex = 0;
        }
        CurrentHair = hairSets[hairSetsIndex];
    }
    public void CycleThruFace(int val) {
        FaceSetsIndex += val;
        if (FaceSetsIndex == faceSets.Count) {
            FaceSetsIndex = 0;
        }
        CurrentFace = faceSets[faceSetsIndex];
    }
    public void CycleThruTop(int val) {
        TopSetsIndex += val;
        if (TopSetsIndex == topSets.Count) {
            TopSetsIndex = 0;
        }
        CurrentTop = topSets[topSetsIndex];
    }
    public void CycleThruBottom(int val) {
        BottomSetsIndex += val;
        if (BottomSetsIndex == bottomSets.Count) {
            BottomSetsIndex = 0;
        }
        CurrentBottom = bottomSets[bottomSetsIndex];
    }
    public void CycleThruShoe(int val) {
        ShoeSetsIndex += val;
        if (ShoeSetsIndex == shoeSets.Count) {
            ShoeSetsIndex = 0;
        }
        CurrentShoe = shoeSets[shoeSetsIndex];
    }
    private void TransferSkinnedMeshes(SkinnedMeshRenderer skin) {

        string cachedRootBoneName = skin.rootBone.name;
        var newBones = new Transform[skin.bones.Length];
        for (var x = 0; x < skin.bones.Length; x++)
            foreach (var newBone in newArmature.GetComponentsInChildren<Transform>())
                if (newBone.name == skin.bones[x].name&&newBone.name!="HairTarget") {
                    newBones[x] = newBone;
                }

        Transform matchingRootBone = GetRootBoneByName(newArmature, cachedRootBoneName);
        skin.rootBone = matchingRootBone != null ? matchingRootBone : newArmature.transform;
        skin.bones = newBones;
        Transform transform;
        (transform = skin.transform).SetParent(baseTarget.transform);
        //transform.localPosition = Vector3.zero;


    }
    static Transform GetRootBoneByName(Transform parentTransform, string name) {
        return parentTransform.GetComponentsInChildren<Transform>().FirstOrDefault(transformChild => transformChild.name == name);
    }
}
