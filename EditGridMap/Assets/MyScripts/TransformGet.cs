using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformGet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject gameObject = this.gameObject;
        OVRDebugConsole.Log(gameObject.name + "x " + gameObject.transform.position.x);
        OVRDebugConsole.Log(gameObject.name + "y " + gameObject.transform.position.y);
        OVRDebugConsole.Log(gameObject.name + "z " +gameObject.transform.position.z);
    }
}
