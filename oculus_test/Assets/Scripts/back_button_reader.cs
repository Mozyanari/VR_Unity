using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class back_button_reader : MonoBehaviour
    {
        public string Name;

        public bool Read()
        {
            return Input.GetKeyDown(Name);
        }
    }
}



