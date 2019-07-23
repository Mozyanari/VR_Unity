using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FPSChecker : MonoBehaviour {

    private Text text;

    // Use this for initialization
    void Start () {
        //貼り付けたオブジェクトのtextを取得
        text = this.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = Convert.ToString(OVRPlugin.GetAppFramerate());

    }
}
