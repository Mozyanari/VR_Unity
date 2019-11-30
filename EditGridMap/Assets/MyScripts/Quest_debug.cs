using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_debug : MonoBehaviour {
    public GameObject head;
    public GameObject right;
    public GameObject left;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //OVRDebugConsole.Log("head x"+head.gameObject.transform.position.x);
        //OVRDebugConsole.Log("head y" + head.gameObject.transform.position.y);
        //OVRDebugConsole.Log("head z" + head.gameObject.transform.position.z);
        Vector3 vector3 = left.gameObject.transform.InverseTransformPoint(head.gameObject.transform.position);
        OVRDebugConsole.Log("x = " + vector3.x);
        OVRDebugConsole.Log("y = " + vector3.y);
        OVRDebugConsole.Log("z = " + vector3.z);
    }
}
