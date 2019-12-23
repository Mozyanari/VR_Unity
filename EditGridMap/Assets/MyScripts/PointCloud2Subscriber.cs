using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace RosSharp.RosBridgeClient
{
    public class PointCloud2Subscriber : Subscriber<Messages.Sensor.PointCloud2>
    {
        //参照するオブジェクト
        public GameObject PointCloud_Object;
        //PointCloudを取得した時のロボットの位置
        public GameObject RobotPosition;
        //PointCloudを取得した時の頭の向き
        public GameObject HeadRotation;

        //取得した点群データ
        private Messages.Sensor.PointCloud2 pointcloud;
        //反映させるmeshデータ
        private Mesh mesh;

        private int receive_flag = 0;
        private int before_length = 0;

        //点群データ
        Vector3[] points;
        int[] indecies;
        Color[] colors;

        //点群のパラメータ
        //点群数
        int numPoints;
        //データ取得の際のオフセット
        int x_offset;
        int y_offset;
        int z_offset;
        int rgb_offset;
        int data_step;


        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            mesh = new Mesh();
            PointCloud_Object.GetComponent<MeshFilter>().mesh = mesh;
            PointCloud_Object.GetComponent<Renderer>().material.SetFloat("_PointSize", 5.0f);
            //GetComponent<MeshFilter>().mesh = mesh;
        }

        protected override void ReceiveMessage(Messages.Sensor.PointCloud2 data)
        {
            pointcloud = data;
            Debug.Log(pointcloud.data.Length);
            if(pointcloud.data.Length != 0)
            {
                //フラグをON
                receive_flag = 1;
                //パラメータを取得
                numPoints = pointcloud.width * pointcloud.height;
                x_offset = pointcloud.fields[0].offset;
                y_offset = pointcloud.fields[1].offset;
                z_offset = pointcloud.fields[2].offset;
                rgb_offset = pointcloud.fields[3].offset;
                data_step = pointcloud.point_step;
                Debug.Log(numPoints);
                //点群データを取得
                points = new Vector3[numPoints];
                indecies = new int[numPoints];
                colors = new Color[numPoints];
                
                for (int i = 0; i < numPoints; i++)
                {
                    //位置
                    //デプスカメラは前方がz，右がx，下がy軸なので，それに合わせてUnityに表示
                    //また，ROS側ではcm単位でデータが届くので100で割ってmに戻す
                    points[i].x = BitConverter.ToInt16(pointcloud.data, i * data_step + x_offset) / 100.0f;
                    points[i].y = -BitConverter.ToInt16(pointcloud.data, i * data_step + y_offset) / 100.0f;
                    points[i].z = BitConverter.ToInt16(pointcloud.data, i * data_step + z_offset) / 100.0f;
                    //色
                    colors[i].r = pointcloud.data[i * data_step + rgb_offset + 2] / 255.0f;
                    colors[i].g = pointcloud.data[i * data_step + rgb_offset + 1] / 255.0f;
                    colors[i].b = pointcloud.data[i * data_step + rgb_offset + 0] / 255.0f;
                    colors[i].a = 0.5f;
                    /*
                    colors[i].r = pointcloud.data[i * data_step + rgb_offset + 0]/255.0f;
                    colors[i].g = pointcloud.data[i * data_step + rgb_offset + 1]/255.0f;
                    colors[i].b = pointcloud.data[i * data_step + rgb_offset + 2]/255.0f;
                    colors[i].a = 1.0f;
                    */
                    /*
                    colors[i].r = 255.0f;
                    colors[i].g = 0.0f;
                    colors[i].b = 0.0f;
                    colors[i].a = 1.0f;
                    */
                    indecies[i] = i;

                    //Debug.Log(i);
                    //Debug.Log(points[i]);
                    //Debug.Log(colors[i]);
                }
                Debug.Log(points[0]);
                //Debug.Log(colors[0]);
            }
        }

        private void create_pointcloud2()
        {
            //PointCloudを表示する原点を計算
            PointCloud_Object.transform.position = RobotPosition.transform.position + new Vector3(0.0f,0.1f,-0.0f);
            PointCloud_Object.transform.rotation = RobotPosition.transform.rotation;
            //PointCloudを表示する向きを計算
            //roll,pitch軸の回転は無効にする
            /*
            Vector3 vec3 = HeadRotation.transform.rotation.eulerAngles;
            vec3.z = 0.0f;
            //vec3.x = 0.0f;
            PointCloud_Object.transform.rotation = Quaternion.Euler(vec3);
            */
            

            //meshを更新してPointCloudを表示
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = points;
            mesh.colors = colors;
            mesh.SetIndices(indecies, MeshTopology.Points, 0);
        }

        private void FixedUpdate()
        {
            //受信フラグが立っていたら実行
            if(receive_flag == 1)
            {
                create_pointcloud2();
                receive_flag = 0;
            }
        }
    }

}
