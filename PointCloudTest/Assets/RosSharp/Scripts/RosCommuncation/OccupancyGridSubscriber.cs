using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OccupancyGridSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        private float resolution;
        private float height;
        private float width;

        //private float yaw;

        private float[] grids;

        private GameObject[] GridPoint;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        
        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid gridmap)
        {
            //地図のパラメータを取得
            resolution = gridmap.info.resolution;
            height = gridmap.info.height;
            width = gridmap.info.width;
            //地図の作成
            //まずはgridmapの取得
            grids = new float[gridmap.data.Length];
            for(int i = 0; i < gridmap.data.Length; i++)
            {
                grids[i] = gridmap.data[i];
            }
            //得られたデータから地図の位置にCubeを置く



        }
    }
}
