using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //右のコントローラの位置を取得
        Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote);
        float x = vector.x;
        float y = vector.y;
        //Debug.Log(x);
        //Debug.Log(y);

        //カメラを移動
        this.transform.position += new Vector3(x*100, 0.0f, y*100);
    }
}
