using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishBowlGame.Player
{
    public class FirstPersonController : MonoBehaviour
    {
        public float lookSpeed { get; private set; } = 2.0f;
        public float lookXLimit { get; private set; } = 85.0f;
        public float rotationX { get; private set; } = 0;
        public GameObject BulletPrefab;
        private Camera playerCamera;

        // Start is called before the first frame update
        void Start()
        {
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.parent.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            var q = playerCamera.transform.rotation * Quaternion.Euler(-45f, 0, 0);
            // var q = Quaternion.Euler(playerCamera.transform.rotation.x - 20f, playerCamera.transform.rotation.y, playerCamera.transform.rotation.z);
            var bulletPos = transform.position + transform.forward * 1f + new Vector3(0, 0, 0);
            var bullet = Instantiate(BulletPrefab, bulletPos, q);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 500f);
        }
    }
}