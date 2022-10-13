using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishBowlGame.Entities
{
    public class FoodItem : MonoBehaviour
    {
        public Rigidbody rigidBody;
        public float Energy = 10f;
        // Start is called before the first frame update
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y < -11)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {

            var fish = collider.GetComponentInParent<Fish>();
            if (fish != null && fish.Consume(this))
            {
                Debug.Log("Collision with fish");
                Destroy(gameObject);
            }
        }
    }
}