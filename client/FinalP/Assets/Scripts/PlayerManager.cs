using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;

namespace Project.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private float speed = 5;

        [Header("Class References")]
        [SerializeField]
        private NetworkIdentity networkIdentity;

     
        public void Update()
        {
            if (networkIdentity.IsControlling())
            {
                checkMovement();
            }
        }

        private void checkMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.position += new Vector3(horizontal,0 , vertical) * speed * Time.deltaTime;
        }
    }
}

