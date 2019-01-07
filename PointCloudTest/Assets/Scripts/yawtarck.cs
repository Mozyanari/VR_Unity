using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yawtarck : MonoBehaviour {

    public GameObject gameobject; // OculusGoの姿勢の参照用
    public Transform target; // 初期方向の参照用

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target);
        var trans = gameobject.transform;
        //trans.position = new Vector3(trans.position.x, (trans.position.y - 1), (trans.position.z + 2));
        //trans.position.Set(trans.position.x, (trans.position.y-2), (trans.position.z + 3));
        transform.Rotate(0.0f, trans.eulerAngles.y, 0.0f, Space.World);
        transform.position = trans.position;
        //transform.position.Set(transform.position.x, 0, transform.position.z +5);
        //transform.position = new Vector3(transform.position.x, transform.position.y -1, transform.position.z +2);
    }
}
