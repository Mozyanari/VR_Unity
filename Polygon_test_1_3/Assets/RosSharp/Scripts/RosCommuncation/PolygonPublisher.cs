using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PolygonPublisher : Publisher<Messages.Geometry.Polygon>
    {
        Messages.Geometry.Polygon message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Geometry.Polygon { points = new Messages.Geometry.Point[1] };
            message.points[0] = new Messages.Geometry.Point();
        }

        private void UpdateMessage()
        {
            message.points[0].x = Random.Range(0f,10f);
            message.points[0].y = Random.Range(0f, 10f);
            message.points[0].z = Random.Range(0f, 10f);

            Publish(message);
        }

        private void Update()
        {
            UpdateMessage();
        }
    }
}

