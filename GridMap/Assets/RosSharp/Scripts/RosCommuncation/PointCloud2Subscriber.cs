using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PointCloud2Subscriber : Subscriber<Messages.Sensor.PointCloud2>
    {
        private int height;
        private int width;
        private int count=0;

        //field
        private string name;
        private int offset;
        private int datatype;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.Sensor.PointCloud2 pointcloud2)
        {
            //高さと幅情報を代入
            height =  pointcloud2.height;
            width = pointcloud2.width;
            Debug.Log(width);
            count++;
            Debug.Log(count);
            double field_length = pointcloud2.fields.Length;
            for(int i = 0; i < field_length; i++)
            {
                //Debug.Log(pointcloud2.fields[i].name);
                //Debug.Log(pointcloud2.fields[i].datatype);
                //Debug.Log(pointcloud2.fields[i].count);
                //Debug.Log(pointcloud2.fields[i].offset);
                
            }
            //Debug.Log(pointcloud2.data.Length);
            //Debug.Log(pointcloud2.point_step);

        }
    }
}