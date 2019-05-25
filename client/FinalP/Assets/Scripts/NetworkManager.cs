using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public SocketIOComponent socket;
    public Canvas canvas,inGame,endGame;
    public Text winnerText, countText,playerNameText;
    public InputField playerNameInput;
    public GameObject player;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {   
        
        socket.On("coin", OnCoin);
        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("other player disconnected", OnOtherPlayerDisconnect);
        
    }
    public void Endgame()
    {
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }


    public void JoinGame()
    {
        StartCoroutine(ConnectToServ());
    }

    IEnumerator ConnectToServ()
    {
        yield return new WaitForSeconds(0.5f);
        socket.Emit("player connect");
        yield return new WaitForSeconds(1f);
        string playerName = playerNameInput.text;
        List<SpawnPoint> playerSpawnPoints = GetComponent<SpawnPlayer>().playerSpawnPoints;
        List<SpawnPoint> coinSpawnPoints = GetComponent<SpawnCoins>().coinSpawnPoints;
        PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints, coinSpawnPoints);
        string data = JsonUtility.ToJson(playerJSON);
        socket.Emit("play", new JSONObject(data));
        playerNameText.text = playerNameInput.text;
        canvas.gameObject.SetActive(false);
    }
    public void CommandMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new PositionJSON(vec3));
        socket.Emit("player move", new JSONObject(data));
    }
    
    void OnCoin(SocketIOEvent socketIOEvent)
    {
        CoinsJSON coinsJSON = CoinsJSON.CreateFromJSON(socketIOEvent.data.ToString());
        SpawnCoins es = GetComponent<SpawnCoins>();
        es.SpawnsCoins(coinsJSON);
    }
    void OnOtherPlayerConnected(SocketIOEvent socketIOEvent)
    {
        print("Someone else joined");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        GameObject o = GameObject.Find(userJSON.name) as GameObject;
        if (o != null)
        {
            return;
        }
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        PlayerManager pc = p.GetComponent<PlayerManager>();
        pc.isLocalPlayer = false;
        p.name = userJSON.name;

    }
    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you joined");
        string data = socketIOEvent.data.ToString();
        UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);
        Vector3 position = new Vector3(currentUserJSON.position[0], currentUserJSON.position[1], currentUserJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(currentUserJSON.rotation[0], currentUserJSON.rotation[1], currentUserJSON.rotation[2]);
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        PlayerManager pc = p.GetComponent<PlayerManager>();
        pc.isLocalPlayer = true;
        p.name = currentUserJSON.name;

    }
    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        // if it is the current player exit
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.position = position;
        }
    }

    void OnOtherPlayerDisconnect(SocketIOEvent socketIOEvent)
    {
        print("user disconnected");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
    }

}
[Serializable]
public class PointJSON
{
    public float[] position;
    public float[] rotation;
    public PointJSON(SpawnPoint spawnPoint)
    {
        position = new float[] {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y,
                spawnPoint.transform.position.z
            };
            rotation = new float[] {
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };
    }
}
[Serializable]
public class PositionJSON
{
    public float[] position;

    public PositionJSON(Vector3 _position)
    {
        position = new float[] { _position.x, _position.y, _position.z };
    }
}

[Serializable]
public class RotationJSON
{
    public float[] rotation;

    public RotationJSON(Quaternion _rotation)
    {
        rotation = new float[] { _rotation.eulerAngles.x,
                _rotation.eulerAngles.y,
                _rotation.eulerAngles.z };
    }
}
[Serializable]
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

[Serializable]
public class CoinsJSON
{
    public List<UserJSON> coins;

    public static CoinsJSON CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<CoinsJSON>(data);
    }
}
[Serializable]
public class UserJSON
{
    public string name;
    public float[] position;
    public float[] rotation;
    public int point;

    public static UserJSON CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<UserJSON>(data);
    }
}
