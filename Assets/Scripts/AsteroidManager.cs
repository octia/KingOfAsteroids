using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AsteroidManager : MonoBehaviour
{

    public GameObject playerGO;

    [Range(0.1f, 10f)]
    public float asteroidSize = 1f;

    // size of a singular grid cell
    [Range(0.1f, 20f)]
    public float gridCellSize = 5f;

    // map size, in gridCells
    [Range(4, 512)]
    public int mapSize = 160;

    // map size, in gridCells
    [Range(0f, 100f)]
    public float asteroidMaxSpeed = 20f;

    // map size, in gridCells
    [Range(0f, 100f)]
    public float asteroidMinSpeed = 3f;

    // dimensions of a rectangle in which objects will be rendered
    private float playerVisionX;
    private float playerVisionY;

    private Vector2 playerSpeed;
    private Vector2 playerRotation;

    private Map map;

    private AsteroidSpawner asteroidSpawner;
    private PlayerManager player;

    private bool simulate;

    // Start is called before the first frame update
    void Start()
    {
        asteroidSpawner = GetComponent<AsteroidSpawner>();
        player = playerGO.GetComponent<PlayerManager>();

        map = new Map(mapSize, asteroidSize, gridCellSize);

        player.transform.position = Vector2.one * mapSize * gridCellSize / 2;

    }

    private void FixedUpdate()
    {
        map.NextStep(Time.fixedDeltaTime);
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                foreach (Asteroid ast in map.map[(int)(mapSize / 2) + x, (int)(mapSize/2) + y].asteroids)
                {
                    if (ast.instantinated == false)
                    {
                        ast.instantinated = true;
                        asteroidSpawner.SpawnAsteroid(ast, gridCellSize);
                    }
                }
            }
        }
    }

    private void PositionOverflow(Asteroid asteroid)
    {

    }


}


