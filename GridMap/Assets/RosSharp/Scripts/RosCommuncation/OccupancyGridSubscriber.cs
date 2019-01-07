using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OccupancyGridSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        //public Transform PublishedTransform;
        private int data_length;
        private float resolution;
        private float height;
        private float width;

        int receive_flag = 0;

        //private float yaw;

        //private float[] grids;

        private GameObject[] GridPoint;
        private Vector3[] GridPosition;

        //障害物の数
        private int count = 0;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (receive_flag == 1)
            {
                createmap();
            }
            
        }

        private void createmap()
        {
            //得られたデータから地図の位置にCubeを置く
            //オブジェクトの初期化
            for (int i = 0; i < count; i++)
            {
                //Cubeのオブジェクトを生成
                GridPoint[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //衝突判定をなくすためにColliderを削除
                DestroyImmediate(GridPoint[i].GetComponent<Collider>());
                GridPoint[i].name = "GridPoint";
                //このスクリプトが張られたオブジェクトを親にして，今回生成したオブジェクトが子になる
                GridPoint[i].transform.parent = transform;
                //マテリアルをParticles/Additiveにする
                GridPoint[i].GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Additive"));
            }

            //オブジェクトの設定
            for (int i = 0; i < count; i++)
            {
                //GridPoint[i].SetActive(ranges[i] != 0);
                GridPoint[i].GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                //GridPoint[i].transform.localScale = objectWidth * Vector3.one;
                GridPoint[i].transform.localPosition = GridPosition[i];
            }

            receive_flag = 0;
        }


        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid gridmap)
        {
            Debug.Log(gridmap.data.Length);
            //障害物の数をリセット
            count = 0;

            //地図のパラメータを取得
            resolution = gridmap.info.resolution;
            height = gridmap.info.height;
            width = gridmap.info.width;
            //dataの取得
            //grids = new float[gridmap.data.Length];
            //grids[i]が100以上なら壁なのでその数を計算
            /*
            for (int i = 0; i < gridmap.data.Length; i++)
            {
                Debug.Log(i);
                if (gridmap.data[i] > 99)
                {
                    count++;
                }
                //grids[i] = gridmap.data[i];
            }
            //Debug.Log(grids[1]);

            //データの数だけオブジェクトを生成
            GridPoint = new GameObject[count];
            GridPosition = new Vector3[count];


            //障害物のポジションを取得
            int j = 0;
            int height_number = (int)(height / resolution);
            int width_number = (int)(width / resolution);

            for (int i = 0; i < gridmap.data.Length; i++)
            {
                if (gridmap.data[i] > 99)
                {
                    //行の番号
                    int line = (i / width_number) + 1;
                    //列の番号
                    int raw = (i % width_number) + 1;
                    //行の位置の計算
                    GridPosition[j].x = (resolution / 2) + (line - 1) * resolution;
                    //列の位置の計算
                    GridPosition[j].y = (resolution / 2) + (raw - 1) * resolution;

                    j++;
                    if (j > count)
                    {
                        Debug.Log("countERR");
                    }
                }
                //grids[i] = gridmap.data[i];
            }

            receive_flag = 1;
            */








        }
    }
}
