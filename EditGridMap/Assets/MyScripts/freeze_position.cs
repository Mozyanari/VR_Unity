using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freeze_position : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
