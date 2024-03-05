using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniHumanoid;
public class HeightReader : MonoBehaviour
{
    Humanoid rig;
    [SerializeField] private GameObject highPoint;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Humanoid>();
        print(Vector3.Distance(rig.RightFoot.position,highPoint.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
