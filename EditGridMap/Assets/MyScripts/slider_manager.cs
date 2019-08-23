using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slider_manager : MonoBehaviour {

    Vector3 start_position;
    public float value;
	// Use this for initialization
	void Start () {
        var rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
        //最初の位置を取得
        start_position = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        value = start_position.z - this.transform.position.z;
	}
    public float GetValue()
    {
        return value;
    }
}
