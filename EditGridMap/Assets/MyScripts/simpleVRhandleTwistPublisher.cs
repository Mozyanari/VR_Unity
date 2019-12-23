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
        [SerializeField]
        public float angle_limit = 40.0f;

        private float grub_angle = 0.0f;

        public GameObject Head;
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

        //ロボットの移動速度を取得できる関数
        public Vector3 GetTwist()
        {
            Vector3 send_data = new Vector3();
            send_data.x = message.linear.x;
            send_data.z = message.angular.z;

            return send_data;
        }

        private void UpdateMessage()
        {
            //値の初期化
            message.linear.x = 0.0f;
            message.angular.z = 0.0f;
            //OVRDebugConsole.Log("message");

            float linear_speed = max_linear_speed;
            float angle_speed = max_angle_speed;

            //デバック用
            /*
            Vector2 L_analog = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            Vector2 R_analog = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

            if ((R_analog.x > 0.1) || (R_analog.x < -0.1) || (L_analog.y > 0.1) || (L_analog.y < -0.1)) {
                handle_flag = 1;
                message.linear.x = linear_speed * L_analog.y;
                message.angular.z = -angle_speed * R_analog.x;
                //OVRDebugConsole.Log("x" + message.linear.x + "z" + message.angular.z);
                //OVRDebugConsole.Log("x" + L_analog.x + "z" + L_analog.y);
                Publish(message);
            }
            else if (handle_flag == 1)
            {
                //ハンドル操作が終わった後に一回だけ停止コマンドを送る
                handle_flag = 0;
                message.linear.x = 0.0f;
                message.angular.z = 0.0f;
                //OVRDebugConsole.Log("x" + message.linear.x + "z" + message.angular.z);
                Publish(message);
            }
            */

            Vector2 L_analog = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            Vector2 R_analog = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            //両方のコントローラの掴むボタンが押されてるか判定
            if ((OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.0f) && (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.0f))
            {

                handle_flag = 1;
                //コントローラの相対的な角度を計算(-180~180)
                Vector3 vector3_R = R_controller.gameObject.transform.InverseTransformPoint(Head.gameObject.transform.position);
                Vector3 vector3_L = L_controller.gameObject.transform.InverseTransformPoint(Head.gameObject.transform.position);
                
                float diff_x = -(vector3_R.x - vector3_L.x);
                float diff_y = (vector3_R.y - vector3_L.y);
                float controller_rad = Mathf.Atan2(diff_y, diff_x);
                float controller_angle = (controller_rad * 180.0f) / Mathf.PI;

                //angleがangle_limit以下なら0とする
                if (Mathf.Abs(controller_angle) < angle_limit)
                {
                    controller_angle = 0;
                }
                //旋回速度
                message.angular.z = (controller_angle / 180.0f) * angle_speed;
                //前進後退速度
                if ((L_analog.y > 0.1f) || (R_analog.y > 0.1f))
                {
                    message.linear.x = linear_speed;
                }
                else if ((L_analog.y < -0.1f) || (R_analog.y < -0.1f))
                {
                    message.linear.x = -linear_speed;
                    message.angular.z = -message.angular.z;
                }
                OVRDebugConsole.Log("R_x=" + vector3_R.x + "R_y=" + vector3_R.y + "L_x=" + vector3_L.x +"L_y=" + vector3_L.y);
                OVRDebugConsole.Log("rad=" + controller_angle + "diff_x=" + diff_x + "diff_y=" + diff_y);
                //OVRDebugConsole.Log("x" + message.linear.x + "z" + message.angular.z);
                Publish(message);
                /*
                //OVRDebugConsole.Log("grab bottn");
                //コントローラの相対的な角度を計算(-180~180)
                float diff_x = R_controller.gameObject.transform.position.x - L_controller.gameObject.transform.position.x;
                float diff_y = R_controller.gameObject.transform.position.y - L_controller.gameObject.transform.position.y;
                float controller_rad = Mathf.Atan2(diff_y, diff_x);
                float controller_angle = controller_rad * 180 / Mathf.PI;
                //掴むボタンが押され始めたらその時の角度を取得
                if(handle_flag == 0)
                {
                    grub_angle = controller_angle;
                    handle_flag = 1;
                }

                
                float use_angle = controller_angle - grub_angle;
                if(use_angle > 180)
                {
                    use_angle -= 360;
                }else if(use_angle < -180)
                {
                    use_angle += 360;
                }
                OVRDebugConsole.Log(""+use_angle);

                //コントローラのアナログスティックが前進してるか後退か判定
                Vector2 L_analog = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
                Vector2 R_analog = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

                //angleがangle_limit以下なら0とする
                if(Mathf.Abs(use_angle) < angle_limit)
                {
                    use_angle = 0;
                }

                if ((L_analog.y > 0.1f) || (R_analog.y > 0.1f))
                {
                    message.linear.x = linear_speed;
                    message.angular.z = (use_angle / 180.0f) * angle_speed;
                }
                else if ((L_analog.y < -0.1f) || (R_analog.y < -0.1f))
                {
                    message.linear.x = -linear_speed;
                    message.angular.z = (-use_angle / 180.0f) * angle_speed;
                }
                OVRDebugConsole.Log("x" + message.linear.x +"z"+ message.angular.z);
                Publish(message);
                */
            }
            else if(handle_flag == 1)
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
