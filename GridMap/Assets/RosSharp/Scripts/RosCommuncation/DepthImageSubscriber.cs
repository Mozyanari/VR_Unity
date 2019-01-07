using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class DepthImageSubscriber : Subscriber<Messages.Sensor.Image>
    {
        private int height;
        private int width;
        private int count = 0;

        //field
        private string name;
        private int offset;
        private int datatype;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.Sensor.Image depthimage)
        {
            int size = depthimage.data.Length;
            Debug.Log(size);
            count++;
            Debug.Log(count);
            Debug.Log(depthimage.encoding);
        }
    }
}