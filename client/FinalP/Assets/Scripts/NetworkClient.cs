using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using Project.Utility;
using System;

namespace Project.Networking
{
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefabs;

        public static string ClientID { get; private set; }
        private Dictionary<string, NetworkIdentity> serverObjects;
        private SocketIOComponent sc;
        public override void Start()
        {
            base.Start();
            initialize();
            setupEvent();
        }

        public override void Update()
        {
            base.Update();
        }

        private void initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void setupEvent()
        {
            On("open", (E) =>
            {
                Debug.Log("Connection to server..");
            });

            On("register", (E) =>
            {
                ClientID = E.data["id"].ToString();
                Debug.LogFormat("Your Client's ID ({0})", ClientID);
            });

            On("spawn", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = Instantiate(playerPrefabs, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                serverObjects.Add(id, ni);
            });

            On("disconnected", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObjects[id].gameObject;
                Destroy(go);
                serverObjects.Remove(id);
            });

            On("updatePosition", (E) =>
            {
                string id = E.data["id"].ToString();
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                float z = E.data["position"]["z"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, z);

            });
            On("playerWin", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                gameStop(id);

            });
        }

        public void gameStop(string id)
        {
            //canvas
            sc.Emit("reset");
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

    }
}

