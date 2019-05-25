using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public bool isLocalPlayer = false;

    Vector3 oldPosition;
    Vector3 currentPosition;
    public int speed;
    public int point;
    int count;

    // Use this for initialization
    void Start()
    {
        NetworkManager.instance.inGame.gameObject.SetActive(true);
        NetworkManager.instance.endGame.gameObject.SetActive(false);
        oldPosition = transform.position;
        currentPosition = oldPosition;
        count = 0;
        NetworkManager.instance.countText.text = "Point : " + count.ToString(); 
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Translate(x, 0, z);

        currentPosition = transform.position;

        if (currentPosition != oldPosition)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position);
            oldPosition = currentPosition;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            count += point;
            SetCountText();
            other.gameObject.SetActive(false);
        }

    }
    void SetCountText()
    {
        NetworkManager.instance.countText.text = "Point : " + count.ToString();
        if (count >= 20)
        {
            
            NetworkManager.instance.inGame.gameObject.SetActive(false);
            NetworkManager.instance.endGame.gameObject.SetActive(true);
            NetworkManager.instance.winnerText.text = "You Win!";
            Destroy(gameObject);
        }
    }
}


