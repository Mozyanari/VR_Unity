using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yazirusi_control : MonoBehaviour {

    //simpleVRhandleTwistPublisherからロボットに与えている速度を取得する
    public RosSharp.RosBridgeClient.simpleVRhandleTwistPublisher twist;
    public GameObject yazisuri;
    MeshRenderer mesh;

    // Use this for initialization
    void Start () {
        mesh = yazisuri.GetComponent<MeshRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 speed = twist.GetTwist();
        if(speed.x == 0.0f && speed.z == 0.0f)
        {
            //動かないなら非表示
            mesh.material.color = new Color(255.0f, 255.0f, 0.0f, 0.0f);
        }
        else
        {
            //動くなら表示
            mesh.material.color = new Color(255.0f, 255.0f, 0.0f, 1.0f);
            float degree = speed.z * 180 / Mathf.PI;
            if (speed.x > 0)
            {
                this.transform.localRotation = Quaternion.AngleAxis(-degree * 5, new Vector3(0, 1, 0));
            }
            else
            {
                this.transform.localRotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
                this.transform.localRotation = Quaternion.AngleAxis((-degree * 5) + 180, new Vector3(0, 1, 0));
            }
            
        }
		
	}
}
