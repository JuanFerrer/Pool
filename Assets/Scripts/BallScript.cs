using UnityEngine;
using System.Collections;

namespace Pool
{
    public class BallScript : MonoBehaviour
    {

        [HideInInspector]
        public bool isMoving;

        // Use this for initialization
        void Awake()
        {
            isMoving = false;
        }

        void FixedUpdate()
        {
            if (!isMoving && GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                isMoving = true;
            }
        }
    }
}
