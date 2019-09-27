/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class simpleVRhandleTwistPublisher : Publisher<Messages.Geometry.Twist>
    {
        //public Transform PublishedTransform;
        //コントローラ(OculusTouchForQuestAndRiftSModel)
        [SerializeField]
        public float max_linear_speed = 0.2f;
        [SerializeField]
        public float max_angle_speed = 1.0f;
        
        public GameObject R_controller;
        public GameObject L_controller;

        private Messages.Geometry.Twist message;
        int handle_flag = 0;
        /*
        private float previousRealTime;
        private Vector3 previousPosition = Vector3.zero;
        private Quaternion previousRotation = Quaternion.identity;
        */

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Geometry.Twist();
            message.linear = new Messages.Geometry.Vector3();
            message.angular = new Messages.Geometry.Vector3();
        }
        private void UpdateMessage()
        {
            //値の初期化
            message.linear.x = 0.0f;
            message.angular.z = 0.0f;
            //OVRDebugConsole.Log("message");

            float linear_speed = max_linear_speed;
            float angle_speed = max_angle_speed;
            //両方のコントローラの掴むボタンが押されてるか判定
            if ((OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.0f) && (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.0f))
            {
                OVRDebugConsole.Log("grab bottn");
                handle_flag = 1;
                //コントローラの相対的な角度を計算(-180~180)
                float diff_x = R_controller.gameObject.transform.position.x - L_controller.gameObject.transform.position.x;
                float diff_y = R_controller.gameObject.transform.position.y - L_controller.gameObject.transform.position.y;
                float controller_rad = Mathf.Atan2(diff_y, diff_x);
                float controller_angle = controller_rad * 180 / Mathf.PI;

                //コントローラのアナログスティックが前進してるか後退か判定
                Vector2 L_analog = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
                Vector2 R_analog = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
                if ((L_analog.y > 0.1f) || (R_analog.y > 0.1f))
                {
                    message.linear.x = linear_speed;
                    message.angular.z = (controller_angle / 180.0f) * angle_speed;
                }
                else if ((L_analog.y < -0.1f) || (R_analog.y < -0.1f))
                {
                    message.linear.x = -linear_speed;
                    message.angular.z = (-controller_angle / 180.0f) * angle_speed;
                }
                Publish(message);
            }else if(handle_flag == 1)
            {
                //ハンドル操作が終わった後に一回だけ停止コマンドを送る
                handle_flag = 0;
                message.linear.x = 0.0f;
                message.angular.z = 0.0f;
                Publish(message);
            }

            
            //OVRDebugConsole.Log("x " + message.linear.x);
            //OVRDebugConsole.Log("w " + message.angular.z);
        }
    }
}
