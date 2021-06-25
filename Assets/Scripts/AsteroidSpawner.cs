using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    public GameObject asteroidPrefab;
    [Range(10, 1000)]
    public int mapSize = 160;
    [Range(1f, 10f)]
    public float asteroidDistance = 1;

    public void SpawnAsteroid(Asteroid toSpawn, float gridCellSize)
    {
        GameObject spawnedAsteroid;
        spawnedAsteroid = Instantiate(asteroidPrefab, transform);
        spawnedAsteroid.transform.position = toSpawn.position * gridCellSize;
        spawnedAsteroid.GetComponent<Rigidbody2D>().velocity = toSpawn.velocity;
        Destroy(spawnedAsteroid, 10);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        /*GameObject spawnedAsteroid;
        Vector2 spawnPos;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                spawnedAsteroid = Instantiate(asteroidPrefab, transform);
                spawnPos = new Vector2(x, y) * asteroidDistance;
                spawnedAsteroid.transform.position = spawnPos;
                spawnedAsteroid.GetComponent<Rigidbody2D>().velocity = Vector2.one*3 * ((x+y)%4 - 2);
            }
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(1/Time.deltaTime);
    }
}
