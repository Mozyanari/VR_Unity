using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.SensorVisualization
{
    public class GetComponentTest : MonoBehaviour
    {
        private LaserScanVisualizer[] laserScanVisualizers;
        private Collider[] cols;
        // Use this for initialization
        void Start()
        {
            laserScanVisualizers = GetComponents<LaserScanVisualizer>();
        }

    // Update is called once per frame
    void Update()
    {
            //laserScanVisualizers = GetComponents<LaserScanVisualizer>();
            //Debug.Log(laserScanVisualizers);
            cols = GetComponents<Collider>();
            foreach (var col in cols)
            {
                Debug.Log(col);
            }
            Debug.Log(cols);

        }
}
}

