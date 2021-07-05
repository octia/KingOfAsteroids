using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject asteroidManagerGO;

    public Vector2 playerVel = Vector2.zero;
    public float playerRotationSpeed = 2f;
    public float playerMaxVelocity = 2f;
    public float bulletCooldown = 0.5f;
    private float bulletCDPassed = 0.0f;
    public float bulletSpeed = 1f;
    public float bulletLifetime = 3f;

    private AsteroidManager manager;

    private float rotateAmount;
    private float velocity;

    public void Start()
    {
        bulletCDPassed = bulletCooldown;
        manager = asteroidManagerGO.GetComponent<AsteroidManager>();
    }


    public void Update()
    {
        rotateAmount = Input.GetAxis("Horizontal");
        velocity = Input.GetAxis("Vertical");
        if (Input.touchSupported)
        {
            if (Input.touchCount > 0)
            {
                rotateAmount = Input.GetTouch(0).position.x - Screen.width / 2;
                rotateAmount /= Screen.width / 2;
                velocity = Input.GetTouch(0).position.y - Screen.height / 2;
                velocity /= Screen.height / 2;
            }
        }
    }

    public void FixedUpdate()
    {
        bulletCDPassed -= Time.deltaTime;
        if (bulletCDPassed < 0)
        {
            bulletCDPassed = bulletCooldown;
            SpawnProjectile();
        }
        
        if (rotateAmount != 0)
        {
            transform.Rotate(-Vector3.forward, rotateAmount* playerRotationSpeed);
        }
        playerVel = -transform.up * velocity * playerMaxVelocity;
    }

    private void SpawnProjectile()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab, transform.position + transform.up, Quaternion.identity);
        spawnedBullet.transform.rotation = transform.rotation;
        spawnedBullet.transform.Rotate(Vector3.forward, 90);
        spawnedBullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
        spawnedBullet.GetComponent<SpawnedBullet>().player = this;
        Destroy(spawnedBullet, bulletLifetime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Asteroid")
        {
            manager.PlayerDeath();
        }
    }
}
