using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PathSubscriber : Subscriber<Messages.Navigation.Path>
    {
        private Messages.Navigation.Path path;
        private int receive_flag = 0;

        //Pathに表示するオブジェクト
        private GameObject[] path_mesh;
        public GameObject path_object;

        //表示する結合したメッシュ
        private Mesh[] path_combineMesh;
        
        protected override void Start()
        {
            base.Start();
        }

        private void FixedUpdate()
        {
            if(receive_flag == 1)
            {
                OVRDebugConsole.Log("" + path.poses.Length);
                for(int i = 0; i < path.poses.Length; i++)
                {
                    OVRDebugConsole.Log("x=" + path.poses[i].pose.position.x + "y=" + path.poses[i].pose.position.y);
                }
                
                createpath();
            }
            
        }

        protected override void ReceiveMessage(Messages.Navigation.Path data)
        {
            path = data;
            receive_flag = 1;
        }

        private void createpath()
        {
            OVRDebugConsole.Log("create_map");
            //前回作られた物があれば削除する(メッシュとコライダー)
            var clones = GameObject.FindGameObjectsWithTag("Path");
            if (clones != null)
            {
                foreach (var clone in clones)
                {
                    Destroy(clone);
                }
            }
            receive_flag = 0;
            //使用する壁のmeshを取得
            Mesh path_object_mesh = path_object.GetComponent<MeshFilter>().sharedMesh;
            //meshの頂点の数から一つのCombineInstanceの中に入るmeshの数wall_numberを設定
            int path_number = (65536 / path_object_mesh.vertexCount);
            //メッシュを何個に分割すればいいか計算
            int path_mesh_count = (path.poses.Length / path_number) + 1;

            //path_mesh_countからCombineInstanceを動的生成
            CombineInstance[][] path_combineInstanceAry = new CombineInstance[path_mesh_count][];
            for (int i = 0; i < path_mesh_count; i++)
            {
                path_combineInstanceAry[i] = new CombineInstance[path_number];
            }

            for(int i= 0; i < path.poses.Length; i++)
            {
                /*
                // 10msec以上経過したら次フレームへ
                float goNextFrameTime = Time.realtimeSinceStartup + 0.01f;
                if (Time.realtimeSinceStartup >= goNextFrameTime)
                {
                    yield return null;
                    goNextFrameTime = Time.realtimeSinceStartup + 0.01f;
                }
                */

                Vector3 Position;
                Position.x = path.poses[i].pose.position.x;
                Position.y = path.poses[i].pose.position.y;
                Position.z = -0.09f;
                //ROSの座標系からUnityの座標系に変換
                Position = Position.Ros2Unity();
                //CombineInstanceに情報を入力
                path_combineInstanceAry[i / path_number][i % path_number].mesh = path_object_mesh;
                path_combineInstanceAry[i / path_number][i % path_number].transform = Matrix4x4.TRS(Position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(0.1f, 0.1f, 0.1f));
            }

            //pathの結合したmeshの生成
            Mesh[] path_combineMesh = new Mesh[path_mesh_count];
            path_mesh = new GameObject[path_mesh_count];
            //Instantiate(path_mesh[1]);
            for (int i = 0; i < path_mesh_count; i++)
            {
                path_combineMesh[i] = new Mesh();
                path_combineMesh[i].CombineMeshes(path_combineInstanceAry[i]);
                path_mesh[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(path_mesh[i].GetComponent<Collider>());
                path_mesh[i].GetComponent<MeshRenderer>().material = path_object.GetComponent<Renderer>().sharedMaterial;
                path_mesh[i].GetComponent<MeshFilter>().mesh = path_combineMesh[i];
                path_mesh[i].tag = "Path";
                path_mesh[i].name = "Path" + i;
            }
            OVRDebugConsole.Log("END");


        }
    }
}
