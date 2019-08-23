using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class PolygonPublisher : Publisher<Messages.Geometry.Polygon>
    {
        private Messages.Geometry.Polygon message;
        private bool send_flag = false;
        [SerializeField]
        public Quest_Laser Quest_Laser;

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
            message = new Messages.Geometry.Polygon();
            /*
            message.points = new Messages.Geometry.Point32[1];

            message.points[0] = new Messages.Geometry.Point32();
            */
        }

        private void FixedUpdate()
        {
            UpdateMessage();
            /*
            if (send_flag)
            {
                send_flag = false;
            }

            //message = Quest_Laser.GetSelectCubePolygon();
            if (message != null)
            {
                message.points[0].x = 100f;
                message.points[0].y = 100f;
                message.points[0].z = 254f;
                Publish(message);
                //Quest_Laser.ResetSelectCubePolygon();
                OVRDebugConsole.Log("Published");
            }
            else
            {
                OVRDebugConsole.Log("PolygonNull");
            }
            */
        }

        private void UpdateMessage()
        {
            List < RosSharp.RosBridgeClient.Messages.Geometry.Point32 > select_cubes = Quest_Laser.GetSelectCubeList();
            if(select_cubes.Count > 10)
            {
                message.points = new Messages.Geometry.Point32[select_cubes.Count];
                for(int i = 0;i< select_cubes.Count; i++)
                {
                    message.points[i] = new Messages.Geometry.Point32();
                }
                message.points = select_cubes.ToArray();
                Publish(message);
                OVRDebugConsole.Log("Published");
                Quest_Laser.ResetSelectCubeList();
            }
            /*
            message.points[0].x = 100.0f;
            message.points[0].y = 100.0f;
            message.points[0].z = 254.0f;
            */
            
            
            

        }
    }
}
