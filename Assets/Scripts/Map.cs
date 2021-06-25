using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public GridCell[,] map;
    private Asteroid[] asteroids;
    public float asteroidSize = 0.0f;
    public float asteroidSizeSqr = 0.0f;
    public float asteroidMaxSpeed = 20f;
    public float asteroidMinSpeed = 3f;
    public float gridSize = 5f;
    public int mapSize = 0;
    private int asteroidCount = 0;

    public Map(int mapLength, float asteroidRadius, float gridCellSize)
    {
        GridCell.map = this;
        mapSize = mapLength;
        asteroidSize = asteroidRadius;
        gridSize = gridCellSize;
        asteroidSizeSqr = asteroidRadius * asteroidRadius;
        asteroidCount = mapLength * mapLength;

        asteroids = new Asteroid[mapSize * mapSize];
        map = new GridCell[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int asteroidNum = x * mapSize + y;
                map[x, y] = new GridCell(x,y);
                asteroids[asteroidNum] = new Asteroid(map[x, y]);
                map[x, y].asteroids.Add(asteroids[asteroidNum]);
            }
        }
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                map[x, y].SetNeighbours(this);
            }
        }
    }

    public void NextStep(float timePassed)
    {
        int asteroidNum;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                asteroidNum = x * mapSize + y;
                asteroids[asteroidNum].Move(timePassed);
                map[x, y].CheckAsteroids();
            }
        }

    }



}
