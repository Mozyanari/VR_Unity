using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandle : MonoBehaviour {

    //コントローラ
    public GameObject R_controller;
    public GameObject L_controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //コントローラの相対的な角度を計算
        float diff_x = R_controller.gameObject.transform.position.x - L_controller.gameObject.transform.position.x;
        float diff_y = R_controller.gameObject.transform.position.y - L_controller.gameObject.transform.position.y;
        float controller_rad = Mathf.Atan2(diff_y, diff_x);
        float controller_angle = controller_rad * 180 / Mathf.PI;
        OVRDebugConsole.Log("angle " + controller_angle);
        if(controller_angle > 0)
        {
            MeshRenderer meshRenderer = R_controller.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }
        else
        {
            MeshRenderer meshRenderer = R_controller.GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }
    }
}
