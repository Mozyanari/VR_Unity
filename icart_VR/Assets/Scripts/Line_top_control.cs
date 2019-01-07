using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_top_control : MonoBehaviour {
    [SerializeField]
    private GameObject Cube_1;
    [SerializeField]
    private GameObject Cube_2;

    private GameObject Cube_3 = GameObject.Find("TwistReceiver");

    private new Vector3 set1;

    [SerializeField]
    private LineRenderer _LaserPointerRenderer; // LineRenderer

    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {
        TwistReceiver tw = GetComponent<TwistReceiver>();
        //set1 = tw.getVector3();
        _LaserPointerRenderer.SetPosition(0, Cube_1.transform.position);
        _LaserPointerRenderer.SetPosition(1, Cube_2.transform.position);
    }
}
