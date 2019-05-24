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
        public override void Start()
        {
            base.Start();
            initialize();
            setupEven();
        }

        public override void Update()
        {
            base.Update();
        }

        private void initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void setupEven()
        {
            On("open", (E) =>
            {
                Debug.Log("Connection to server..");
            });

            On("register", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();
                Debug.LogFormat("Your Client's ID ({0})", ClientID);
            });

            On("spawn", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = Instantiate(playerPrefabs, networkContainer);
                go.name = string.Format("Plyer ({0})", id);
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
                string id = E.data["id"].ToString().RemoveQuotes();
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                float z = E.data["position"]["z"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, z);

            });

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

