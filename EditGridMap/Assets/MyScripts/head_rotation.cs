using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class head_rotation : MonoBehaviour {

    public GameObject head;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.rotation = head.gameObject.transform.rotation;
	}
}
