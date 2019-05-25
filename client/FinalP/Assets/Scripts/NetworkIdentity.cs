using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
public class NetworkIdentity : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private bool isControlling;

    private SocketIOComponent socket;

    public void Awake()
    {
        isControlling = false;
    }

    public void SetControllerID(string ID)
    {
        id = ID;
        Debug.LogFormat("seting's ID ({0})", id);
        isControlling = (NetworkManager.ClientID == ID) ? true : false;
    }

    public void SetSocketReference(SocketIOComponent Socket)
    {
        socket = Socket;
    }

    public string GetID()
    {
        return id;
    }

    public bool IsControlling()
    {
        return isControlling;
    }
    public SocketIOComponent GetSocket()
    {
        return socket;
    }
}
