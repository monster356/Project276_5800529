using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public Canvas ingame, Endgame;
    public Text countText, winnerText;
    public Text playerName;
    public bool isLocalPlayer = false;

    Vector3 oldPosition;
    Vector3 currentPosition;
    public int speed;
    public int point;
    public int count;

    // Use this for initialization
    void Start()
    {
        ingame.gameObject.SetActive(true);
        Endgame.gameObject.SetActive(false);
        oldPosition = transform.position;
        currentPosition = oldPosition;
        count = 0;
        playerName.text = NetworkManager.instance.playerNameInput.text;
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
        countText.text = "Point : " + count.ToString();
        if (count >= 20)
        {
            ingame.gameObject.SetActive(false);
            Endgame.gameObject.SetActive(true);
            winnerText.text = "You Win!";
            NetworkManager.instance.Endgame();
        }
    }
}


