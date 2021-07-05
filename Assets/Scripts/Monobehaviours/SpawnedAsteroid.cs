using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAsteroid : MonoBehaviour
{

    public GameObject explosion;
    [HideInInspector] public AsteroidManager asteroidManager;
    [HideInInspector] public int asteroidNumRef = -1;
    private bool spawnExplosion = false;
    private byte addPoints;
    private bool destroyed;
    // Start is called before the first frame update
    void Start()
    {
        destroyed = false;
        if (asteroidNumRef == -1)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!destroyed)
        {
            destroyed = true;
            addPoints = 0;
            if (collision.tag == "Bullet")
            {
                addPoints = 1;
            }
        }
        gameObject.SetActive(false);
        asteroidManager.DestroyAsteroid(asteroidNumRef, false, addPoints);
    }

    private void OnDestroy()
    {
        if (spawnExplosion)
        {
            //Instantiate(explosion, transform.position, Quaternion.identity);
        }
    }

}
