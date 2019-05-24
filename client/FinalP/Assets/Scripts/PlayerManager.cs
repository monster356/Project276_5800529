using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public float speed = 5;

        public void Update()
        {
             float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.position += new Vector3(horizontal,0 , vertical) * speed * Time.deltaTime;
        }
        
    }
}

