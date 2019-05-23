using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coin;
    public GameObject spawnPoint;
    public int numberOfCoins;
    [HideInInspector]
    public List<SpawnPoint> CoinSpawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< numberOfCoins;i++)
        {
            var spawnPosition = new Vector3(Random.Range(-20f,20f),0f,Random.Range(-20f,20f));
            var spwanRotation = Quaternion.Euler(0f,0f,90f);
            SpawnPoint CoinSpawnPoint = (Instantiate(spawnPoint,spawnPosition,spwanRotation) as GameObject).GetComponent<SpawnPoint>();
            CoinSpawnPoints.Add(CoinSpawnPoint);
        }
        //SpawnCoin();
    }
    public void SpawnCoin(){

        int i=0;
        foreach (SpawnPoint sp in CoinSpawnPoints)
        {
            Vector3 position = sp.transform.position;
            Quaternion rotation = sp.transform.rotation;
            GameObject newCoin = Instantiate(coin, position, rotation) as GameObject;
            coin.name = i+"";
            i++;
        }
    }

}
