using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public float speed = 5;
    public int point=0;
    public Text countText;
    public Canvas ingame, endgame;
    public Text Win;

    [SerializeField]
    private NetworkIdentity networkIdentity;

    public void Start()
    {
            
            point = 0;
    }

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

        transform.position += new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
    }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                point += 1;
                other.gameObject.SetActive(false);
            }
        }
        void SetCountText()
     {
        countText.text = "Point: " + point.ToString();
        if (point >= 20)
        {
            Win.text = "You Win!";
        }
    }

}


