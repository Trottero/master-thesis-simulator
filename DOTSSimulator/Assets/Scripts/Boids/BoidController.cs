using UnityEngine;

namespace Simulator.Boids
{

    public class BoidController : MonoBehaviour
    {
        [SerializeField] public Mesh BoidMesh;
        [SerializeField] public UnityEngine.Material BoidMaterial;

        public static BoidController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}