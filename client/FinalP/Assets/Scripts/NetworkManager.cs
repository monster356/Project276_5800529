using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    public SocketIOComponent socket;
    public Canvas canvas;
    public InputField playerNameInput;
    public GameObject player;
    public Transform playerSpawnPoint;

    public static string ClientID{ get; private set; }

    private Dictionary<string, NetworkIdentity> serverObj;
  
    void Start()
    {
        serverObj = new Dictionary<string, NetworkIdentity>();
        socket.On("open", OnOpen);
        socket.On("register", OnRegisters);
        socket.On("spawn", OnSpawn);
        socket.On("updatePosition", OnUpdatePosition);
        socket.On("updatePoint", OnUpdatePoint);
        socket.On("disconnected", OnDisconnected);

    }
    void OnOpen(SocketIOEvent E)
    {
        Debug.Log("ConnectToServer");
    }
    void OnRegisters(SocketIOEvent E)
    {
        ClientID = E.data["id"].ToString();
        Debug.LogFormat("Our Client's ID ({0})", ClientID);
    }

    void OnSpawn(SocketIOEvent E)
    {
        string id = E.data["id"].ToString();

        GameObject go = Instantiate(player, playerSpawnPoint);
        go.name = string.Format("Player ({0})", id);
        
        NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
        ni.SetControllerID(id);
        ni.SetSocketReference(socket);
        serverObj.Add(id, ni);
    }

    void OnUpdatePosition(SocketIOEvent E)
    {
        string id = E.data["id"].ToString();
        float x = E.data["position"]["x"].f;
        float y = E.data["position"]["y"].f;
        float z = E.data["position"]["z"].f;

        NetworkIdentity ni = serverObj[id];
        ni.transform.position = new Vector3(x,y,z);
    }

    void OnUpdatePoint(SocketIOEvent E)
    {

    }
    void OnDisconnected(SocketIOEvent E)
    {
        string id = E.data["id"].ToString();

        GameObject go = serverObj[id].gameObject;
        Destroy(go);
        serverObj.Remove(id);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

   
}
[Serializable]
public class Player
{
    public string id;
    public Position position;
}
[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}
