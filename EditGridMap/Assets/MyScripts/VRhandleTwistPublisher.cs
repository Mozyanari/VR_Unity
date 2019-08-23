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
    public class VRhandleTwistPublisher : Publisher<Messages.Geometry.Twist>
    {
        //public Transform PublishedTransform;
        //コントローラ(OculusTouchForQuestAndRiftSModel)
        [SerializeField]
        public float x_range = 1.0f;
        [SerializeField]
        public float y_range = 1.0f;
        [SerializeField]
        public float z_range = 1.0f;
        [SerializeField]
        public float max_linear_speed = 0.2f;
        [SerializeField]
        public float max_angle_speed = 1.0f;

        public GameObject Head_set;
        public GameObject R_controller;
        public GameObject L_controller;

        public GameObject icart_model;
        public GameObject handle_model;

        public slider_manager vel;
        public slider_manager angle;

        public Text vel_text;
        public Text angle_text;

        private MeshRenderer R_mesh;
        private MeshRenderer L_mesh;

        private Messages.Geometry.Twist message;
        /*
        private float previousRealTime;
        private Vector3 previousPosition = Vector3.zero;
        private Quaternion previousRotation = Quaternion.identity;
        */

        protected override void Start()
        {
            base.Start();
            InitializeMessage();

            R_mesh = R_controller.GetComponent<MeshRenderer>();
            L_mesh = L_controller.GetComponent<MeshRenderer>();


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

            float linear_speed = (vel.GetValue() + 0.5f) * max_linear_speed;
            float angle_speed = (angle.GetValue() + 0.5f) * max_angle_speed;

            vel_text.text = "linear_max" + linear_speed.ToString("f2") + "m/s";
            angle_text.text = "angle_max" + angle_speed.ToString("f2") + "deg/s";

            //コントローラが範囲内にあるか判定
            if ((Mathf.Abs(R_controller.gameObject.transform.position.x - Head_set.gameObject.transform.position.x) < x_range) && (Mathf.Abs(R_controller.gameObject.transform.position.y - Head_set.gameObject.transform.position.y) < y_range) && (Mathf.Abs(R_controller.gameObject.transform.position.z - Head_set.gameObject.transform.position.z) < z_range) && (Mathf.Abs(L_controller.gameObject.transform.position.x - Head_set.gameObject.transform.position.x) < x_range) && (Mathf.Abs(L_controller.gameObject.transform.position.y - Head_set.gameObject.transform.position.y) < y_range) && (Mathf.Abs(L_controller.gameObject.transform.position.z - Head_set.gameObject.transform.position.z) < z_range))
            {
                //両方のコントローラの掴むボタンが押されてるか判定
                if((OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.0f) && (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0.0f))
                {
                    //コントローラの表示を消して，ハンドルを表示
                    //todo

                    //コントローラの相対的な角度を計算(-180~180)
                    float diff_x = R_controller.gameObject.transform.position.x - L_controller.gameObject.transform.position.x;
                    float diff_y = R_controller.gameObject.transform.position.y - L_controller.gameObject.transform.position.y;
                    float controller_rad = Mathf.Atan2(diff_y, diff_x);
                    float controller_angle = controller_rad * 180 / Mathf.PI;

                    //コントローラのアナログスティックが前進してるか後退か判定
                    Vector2 L_analog = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
                    Vector2 R_analog = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
                    if((L_analog.y > 0.1f) || (R_analog.y > 0.1f))
                    {
                        message.linear.x = linear_speed;
                        message.angular.z = (controller_angle/180.0f) * angle_speed;
                        icart_model.transform.rotation = Quaternion.Euler(0, -controller_angle, 0);
                        handle_model.transform.rotation = Quaternion.Euler(0, 0, controller_angle);
                    }
                    else if((L_analog.y < -0.1f) || (R_analog.y < -0.1f))
                    {
                        message.linear.x = -linear_speed;
                        message.angular.z = (-controller_angle/180.0f) * angle_speed;
                        icart_model.transform.rotation = Quaternion.Euler(0, -controller_angle, 0);
                        handle_model.transform.rotation = Quaternion.Euler(0, 0, controller_angle);
                    }
                    else
                    {
                        stop_motion();
                    }
                }
                else
                {
                    stop_motion();
                }
            }
            else
            {
                stop_motion();
            }
            
            OVRDebugConsole.Log("x " + message.linear.x);
            OVRDebugConsole.Log("w " + message.angular.z);

            /*
            float deltaTime = Time.realtimeSinceStartup - previousRealTime;

            Vector3 linearVelocity = (PublishedTransform.position - previousPosition) / deltaTime;
            Vector3 angularVelocity = (PublishedTransform.rotation.eulerAngles - previousRotation.eulerAngles) / deltaTime;

            message.linear = GetGeometryVector3(linearVelocity.Unity2Ros());
            message.angular = GetGeometryVector3(-angularVelocity.Unity2Ros());

            previousRealTime = Time.realtimeSinceStartup;
            previousPosition = PublishedTransform.position;
            previousRotation = PublishedTransform.rotation;
            */
            Publish(message);
        }

        private static Messages.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            Messages.Geometry.Vector3 geometryVector3 = new Messages.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }

        private void stop_motion()
        {
            //モデルの角度初期化
            icart_model.transform.rotation = Quaternion.Euler(0, 0, 0);
            handle_model.transform.rotation = Quaternion.Euler(0, 0, 0);

            //コントローラを表示
            //R_mesh.enabled = true;
            //L_mesh.enabled = true;
        }
    }
}
