using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[ExecuteInEditMode]

public class VroidBreaker : EditorWindow
{   
    private GameObject vroidCharacter;
    [MenuItem("Tools/Vroid Breaker")]
    // Start is called before the first frame update
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
            //TakeInCharacter(go);
        }  
    }
    private void ScanObject(GameObject go) {
        GameObject root = go.transform.Find("Root").gameObject;
        GameObject hair = go.transform.Find("Hair").gameObject;
        GameObject face = go.transform.Find("Face").gameObject;
        GameObject body = go.transform.Find("Body").gameObject;
        GameObject newHair = new GameObject();
        newHair.name = "Hair";
        GameObject newFace = new GameObject();
        newFace.name = "Face";
        GameObject newBody = new GameObject();
        newBody.name = "Body";
        Instantiate(root, newHair.transform);
        Instantiate(hair, newHair.transform);
        Instantiate(root, newFace.transform);
        Instantiate(face, newFace.transform);
        Instantiate(root, newBody.transform);
        Instantiate(body, newBody.transform);
        //root.transform.SetParent(newHair.transform);
        //hair.transform.SetParent(newHair.transform);
        //Instantiate(newHair, new Vector3(0, 0, 0), Quaternion.identity);
        //root.transform.SetParent(newFace.transform);
        //face.transform.SetParent(newFace.transform);
        //Instantiate(newFace, new Vector3(0, 0, 0), Quaternion.identity);
        //root.transform.SetParent(newBody.transform);
        //body.transform.SetParent(newBody.transform);
        //Instantiate(newBody, new Vector3(0, 0, 0), Quaternion.identity);
        // GameObject hair = go.transform.Find("Hair").gameObject;
    }
    /*public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);
    }*/
}
