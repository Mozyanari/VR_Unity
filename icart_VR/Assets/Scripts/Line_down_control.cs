using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_down_control : MonoBehaviour
{
    [SerializeField]
    private GameObject Cube_1;
    [SerializeField]
    private GameObject Cube_2;

    [SerializeField]
    private LineRenderer _LaserPointerRenderer; // LineRenderer

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        _LaserPointerRenderer.SetPosition(0, Cube_1.transform.position);
        _LaserPointerRenderer.SetPosition(1, Cube_2.transform.position);
    }
}
