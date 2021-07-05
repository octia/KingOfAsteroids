using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedBullet : MonoBehaviour
{
    [HideInInspector] public PlayerManager player;

    private void FixedUpdate() // adjusting position to match asteroid movement
    {
        transform.position += (Vector3)player.playerVel * Map.map.timeStep;
    }

    private void Update()
    {
        if (!player.gameObject.activeSelf)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            Destroy(gameObject);
        }
    }
}
