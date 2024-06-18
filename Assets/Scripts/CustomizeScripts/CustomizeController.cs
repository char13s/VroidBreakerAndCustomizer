using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniHumanoid;

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
    List<GameObject> rootSets = new List<GameObject>();
    GameObject currentRoot;
    [SerializeField] private GameObject hairTarget;
    [SerializeField] private GameObject faceTarget;
    [SerializeField] private GameObject topTarget;
    [SerializeField] private GameObject bottomTarget;
    [SerializeField] private GameObject shoeTarget;
    [SerializeField] private GameObject baseTarget;
    [SerializeField] private Transform newArmature;
    [SerializeField] private GameObject neckBone;
    private GameObject hairRef;
    private SkinnedMeshRenderer[] skinnedMeshRenderersList;
    public int HairSetsIndex { get => hairSetsIndex; set => hairSetsIndex = Mathf.Clamp(value, 0, hairSets.Count); }
    public int FaceSetsIndex { get => faceSetsIndex; set => faceSetsIndex = Mathf.Clamp(value, 0, faceSets.Count); }
    public int TopSetsIndex { get => topSetsIndex; set => topSetsIndex = Mathf.Clamp(value, 0, topSets.Count); }
    public int BottomSetsIndex { get => bottomSetsIndex; set => bottomSetsIndex = Mathf.Clamp(value, 0, bottomSets.Count); }
    public int ShoeSetsIndex { get => shoeSetsIndex; set => shoeSetsIndex = Mathf.Clamp(value, 0, shoeSets.Count); }
    public GameObject CurrentHair { get => currentHair; set { currentHair = value; SetHair(); } }
    public GameObject CurrentFace { get => currentFace; set { currentFace = value; SetFace(); } }
    public GameObject CurrentTop { get => currentTop; set { currentTop = value; SetTop(); } }
    public GameObject CurrentBottom { get => currentBottom; set { currentBottom = value; SetBottom(); } }
    public GameObject CurrentShoe { get => currentShoe; set { currentShoe = value; SetShoe(); } }
    private void Start() {
        GetSets();
        SetDefaults();
    }
    private void SetDefaults() {
        if (topSets[topSetsIndex])
            CurrentTop = topSets[topSetsIndex];
        if (hairSets[hairSetsIndex])
            CurrentHair = hairSets[hairSetsIndex];
        if (faceSets[faceSetsIndex])
            CurrentFace = faceSets[faceSetsIndex];
        
        if (bottomSets[bottomSetsIndex])
            CurrentBottom = bottomSets[bottomSetsIndex];
        if (shoeSets[shoeSetsIndex])
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
    private void ResetAnimation() {
        if (hairRef != null) {
            baseTarget.GetComponent<Animator>().applyRootMotion = true;
            baseTarget.GetComponent<Animator>().Play("Idle");
            baseTarget.GetComponent<Animator>().Play("Idle 0");
            hairRef.GetComponent<Animator>().applyRootMotion = true;
            hairRef.GetComponent<Animator>().Play("Idle 0");
            hairRef.GetComponent<Animator>().Play("Idle");
        }
        else {
            baseTarget.GetComponent<Animator>().applyRootMotion = true;
            baseTarget.GetComponent<Animator>().Play("Idle");
            baseTarget.GetComponent<Animator>().Play("Idle 0");
        }
    }
    private void SetHair() {
        if (baseTarget.transform.Find("Hair")) {
            Destroy(baseTarget.transform.Find("Hair").gameObject);
        }
        GameObject top = Instantiate(currentHair);
        hairRef = top;
        top.GetComponentInChildren<SkinnedMeshRenderer>().rootBone = top.transform.Find("Root");
        top.transform.SetParent(baseTarget.transform);
        top.name = "Hair";
        top.GetComponent<Animator>().runtimeAnimatorController = baseTarget.GetComponent<Animator>().runtimeAnimatorController;
        top.GetComponent<Animator>().enabled = true;
        ResetAnimation();
    }
    private void SetFace() {
        if (baseTarget.transform.Find("Face")) {
            GameObject face = baseTarget.transform.Find("Face").gameObject;
            DestroyImmediate(face);
        }
        GameObject top = Instantiate(currentFace);
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>(), "Face", baseTarget);
        ResetAnimation();
        Destroy(top);
    }
    private void SetBottom() {
        if (baseTarget.transform.Find("Bottom")) {
            GameObject bottoms = baseTarget.transform.Find("Bottom").gameObject;
            DestroyImmediate(bottoms);
        }
        GameObject top = Instantiate(currentBottom);
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>(), "Bottom", baseTarget);
        ResetAnimation();
        Destroy(top);
    }
    private void SetTop() {
        GameObject oldBase = baseTarget;
        baseTarget = Instantiate(currentTop);
        newArmature = baseTarget.transform.Find("Root");
        baseTarget.GetComponent<Animator>().enabled = true;
        if (currentFace != null) {
            CurrentHair = hairSets[hairSetsIndex];
            CurrentFace = faceSets[faceSetsIndex];
            CurrentShoe = shoeSets[shoeSetsIndex];
            CurrentBottom = bottomSets[bottomSetsIndex];
        }
        baseTarget.name = "Player";
        DestroyImmediate(oldBase);
    }
    private void SetShoe() {
        if (baseTarget.transform.Find("Shoes")) {
            GameObject shoe = baseTarget.transform.Find("Shoes").gameObject;
            DestroyImmediate(shoe);
        }
        GameObject top = Instantiate(currentShoe);
        TransferSkinnedMeshes(top.GetComponentInChildren<SkinnedMeshRenderer>(), "Shoes", baseTarget);
        ResetAnimation();
        Destroy(top);
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
    private void TransferSkinnedMeshes(SkinnedMeshRenderer skin, string name, GameObject parent) {
        string cachedRootBoneName = skin.rootBone.name;
        var newBones = new Transform[skin.bones.Length];
        for (var x = 0; x < skin.bones.Length; x++) {
            foreach (var newBone in newArmature.GetComponentsInChildren<Transform>()) {
                if (newBone.name == skin.bones[x].name && newBone.name != "Root") {
                    newBones[x] = newBone;
                }

            }
        }
        Transform matchingRootBone = GetRootBoneByName(newArmature, cachedRootBoneName);
        skin.rootBone = matchingRootBone != null ? matchingRootBone : newArmature;
        skin.bones = newBones;
        Transform transform;
        (transform = skin.transform).SetParent(parent.transform);
        transform.gameObject.name = name;
    }
    static Transform GetRootBoneByName(Transform parentTransform, string name) {
        return parentTransform.GetComponentsInChildren<Transform>().FirstOrDefault(transformChild => transformChild.name == name);
    }
}
