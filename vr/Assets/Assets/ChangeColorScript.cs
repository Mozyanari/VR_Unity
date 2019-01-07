using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorScript : MonoBehaviour {
    public Material color1;
    public Material color2;
    private GameObject obj;

    // Use this for initialization
    void Start () {
        obj = GameObject.Find("Sphere");
    }
	
	// Update is called once per frame
	void Update () {
        var material = obj.GetComponent<Renderer>().material;
        if (Input.GetMouseButton(0))
        {

            if (material.color == color1.color)
            {
                material.color = color2.color;
            }
            else
            {
                material.color = color1.color;
            }
        }
    }
}
