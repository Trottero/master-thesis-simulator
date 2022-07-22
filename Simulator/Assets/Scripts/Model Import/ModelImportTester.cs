using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator.Testing
{
    public class ModelImportTester : MonoBehaviour
    {
        public float Speed = 1f;
        public float MaxDistance = 2f;

        void FixedUpdate()
        {
            transform.position = transform.position + transform.forward * Speed * Time.deltaTime;


            if (transform.position.magnitude > MaxDistance)
            {
                transform.position = Vector3.zero;
            }
        }
    }
}