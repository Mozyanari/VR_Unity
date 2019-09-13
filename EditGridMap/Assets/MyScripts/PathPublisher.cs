using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class PathPublisher : Publisher<Messages.Navigation.Path>
    {
        private Messages.Navigation.Path message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();

        }

        private void InitializeMessage()
        {
            /*
            message = new Messages.Geometry.Polygon
            {
                points = 
            };
            */
            message = new Messages.Navigation.Path();
            /*
            message.points = new Messages.Geometry.Point32[1];

            message.points[0] = new Messages.Geometry.Point32();
            */
        }

        private void FixedUpdate()
        {
            UpdateMessage();

        }

        private void UpdateMessage()
        {
            message.poses = new Messages.Geometry.PoseStamped[3];
            for(int i = 0; i < 3; i++)
            {
                message.header = new Messages.Standard.Header();
                message.poses[i] = new Messages.Geometry.PoseStamped();
            }
            message.header.Update();
            message.poses[0].pose.position.x = 1.0f;
            message.poses[0].pose.position.y = 1.0f;
            message.poses[0].pose.orientation.w = 1.0f;

            message.poses[1].pose.position.x = 2.0f;
            message.poses[1].pose.position.y = 2.0f;
            message.poses[1].pose.orientation.w = 1.0f;

            message.poses[2].pose.position.x = 3.0f;
            message.poses[2].pose.position.y = 3.0f;
            message.poses[2].pose.orientation.w = 1.0f;

            Publish(message);

        }
    }
}
