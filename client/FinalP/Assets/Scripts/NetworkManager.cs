using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    static SocketIOComponent socket;
    public GameObject playerPrefab;
    float x,z;

    void Start()
    {

        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);

        socket.On("spawn", OnSpawn);

        socket.On("move", OnMove);
    }

    void Update()
    {
        
    }
    void OnConnected(SocketIOEvent e)
    {
        print("connected");

        socket.Emit("move");

    }
    void OnSpawn(SocketIOEvent e)
    {
        Vector3 position = new Vector3(0f, 1f, 0f);
        print("spawned");

        Instantiate(playerPrefab);
        //socket.Emit("move");

    }
    void OnMove(SocketIOEvent e)
    {
        print("player is moving " + e.data);
    }
}
