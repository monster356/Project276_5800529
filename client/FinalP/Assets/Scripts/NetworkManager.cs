using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    public InputField playerNameInput;
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
    public void JoinGame()
    {
        StartCoroutine(ConnectToServer());
    }
    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

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

    #region JSONMessageClasses
    [System.Serializable]
    public class PlayerJSON
    {
        public string name;
        public List<PointJSON> playerSpawnPoints;
        public List<PointJSON> coinSpawnPoints;
        public PlayerJSON(string _name, List<SpawnPoint> _playerSpawnPoints, List<SpawnPoint> _coinSpawnPoints)
        {
            playerSpawnPoints = new List<PointJSON>();
            coinSpawnPoints = new List<PointJSON>();
            name = _name;
            foreach (SpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(playerSpawnPoint);
                playerSpawnPoints.Add(pointJSON);
            }
            foreach (SpawnPoint coinSpawnPoint in _coinSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(coinSpawnPoint);
                coinSpawnPoints.Add(pointJSON);
            }
        }
    }
    [System.Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;
        public PointJSON(SpawnPoint spawnPoint)
        {
            position = new float[]{
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y,
                spawnPoint.transform.position.z
            };
            rotation = new float[]{
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };
        }
    }

    [System.Serializable]
    public class PositionJSON
    {
        public float[] position;

        public PositionJSON(Vector3 _position)
        {
            position = new float[] {_position.x,_position.y,_position.z};
        }
    }

    [System.Serializable]
    public class RotationJSON
    {
        public float[] rotation;

        public RotationJSON(Vector3 _rotation)
        {
            rotation = new float[] {_rotation.x,_rotation.y,_rotation.z};
        }
    }

    [System.Serializable]
    public class UserJSON
    {
        public string name;
        public float[] position;
        public float[] rotation;
        public int count;

        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }

    [System.Serializable]
    public class CountCoinJSON
    {
        public string name;
        public int count;
        public string from;

        public CountCoinJSON(string _name,int _count,string _from)
        {
            name = _name;
            count = _count;
            from = _from;
        }

        [System.Serializable]
        public class CoinJSON
        {
            public List<UserJSON> coins;
            public static CoinJSON CreateFromJSON(string data)
            {
                return JsonUtility.FromJson<CoinJSON>(data);
            }
        }
    }
    #endregion
}
