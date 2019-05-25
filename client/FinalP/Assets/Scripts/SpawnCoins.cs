using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCoins : MonoBehaviour
{
    public GameObject coin;
    public GameObject spawnPoint;
    public int numberOfCoins;
    [HideInInspector]
    public List<SpawnPoint> coinSpawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i < numberOfCoins; i++)
        {
            var spawnPosition = new Vector3(Random.Range(-16.7f, 16.7f), 0f, Random.Range(-16.7f, 16.7f));
            var spawnRotation = Quaternion.Euler(0f, 0f, 90f);
            SpawnPoint coinSpawnPoint = (Instantiate(spawnPoint,spawnPosition,spawnRotation)as GameObject).GetComponent<SpawnPoint>();
            coinSpawnPoints.Add(coinSpawnPoint);
        }

    }

    public void SpawnsCoins(CoinsJSON coinsJSON)
    {
        foreach(UserJSON coinJSON in coinsJSON.coins)
        {
            Vector3 position = new Vector3(coinJSON.position[0], coinJSON.position[1], coinJSON.position[2]);
            Quaternion rotation = Quaternion.Euler(coinJSON.rotation[0], coinJSON.rotation[1], coinJSON.rotation[2]);
            GameObject newCoin = Instantiate(coin, position, rotation) as GameObject;
            newCoin.name = coinJSON.name;
            
        }
    }
}
