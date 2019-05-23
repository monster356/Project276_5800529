using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    public Canvas canvas;
    public InputField playerNameInput;
    static SocketIOComponent socket;
    public GameObject playerPrefab;
    float x,z;

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("coins", OnSpawnCoins);
        socket.On("other player connected",OnOtherPlayerConnected);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player turn", OnPlayerTurn);
		socket.On("other player disconnected", OnOtherPlayerDisconnect);
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
        socket.Emit("player connect");
        yield return new  WaitForSeconds(1.5f);
        string playerName = playerNameInput.text;
		List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoints;
		List<SpawnPoint> coinSpawnPoints = GetComponent<CoinSpawner>().CoinSpawnPoints;
		PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints, coinSpawnPoints);
		string data = JsonUtility.ToJson(playerJSON);
		socket.Emit("play", new JSONObject(data));
		canvas.gameObject.SetActive(false);    
    }
    void OnSpawnCoins(SocketIOEvent e)
    {

    }
    void OnOtherPlayerConnected(SocketIOEvent e)
    {
print("Someone else joined");
		string data = e.data.ToString();
		UserJSON userJSON = UserJSON.CreateFromJSON(data);
		Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
		Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
		GameObject o = GameObject.Find(userJSON.name) as GameObject;
		if (o != null)
		{
			return;
		}
		GameObject p = Instantiate(playerPrefab, position, rotation) as GameObject;
		// here we are setting up their other fields name and if they are local
		PlayerController pc = p.GetComponent<PlayerController>();
		
    }
    void OnPlay(SocketIOEvent e)
    {

    }
    void OnPlayerMove(SocketIOEvent e)
    {

    }
    void OnPlayerTurn(SocketIOEvent e)
    {

    }
    void OnOtherPlayerDisconnect(SocketIOEvent e)
    {

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
