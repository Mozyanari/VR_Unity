using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var WebCamdevices : WebCamDevice[] = WebCamTexture.devices;
        if (WebCamdevices.length > 0)
        {
            var myWebcamTexture : WebCamTexture = WebCamTexture(WebCamdevices[0].name);
            var cube:GameObject = gameObject.Find("Cube");
            var sphere:GameObject = gameObject.Find("Sphere");
            var plane:GameObject = gameObject.Find("Plane 1");
            cube.GetComponent(Renderer).material.mainTexture = myWebcamTexture;
            sphere.GetComponent(Renderer).material.mainTexture = myWebcamTexture;
            plane.GetComponent(Renderer).material.mainTexture = myWebcamTexture;
            myWebcamTexture.Play();
        }
        else
        {
            Debug.LogError("Webカメラが検出できませんでした。");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
