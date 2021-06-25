using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public GridCell[,] map;
    private Asteroid[] asteroids;
    public float asteroidRadius = 0.0f;
    public float asteroidRadiusSqr = 0.0f;
    public float asteroidMaxSpeed = 20f;
    public float asteroidMinSpeed = 3f;
    public float gridSize = 5f;
    public float gridRadius = 0.0f;
    public float gridRadiusSqr = 0.0f;
    public int mapSize = 0;
    public float realMapSize;
    private int asteroidCount = 0;

    public Map(int mapLength, float asteroidRadius, float gridCellSize)
    {
        GridCell.map = this;
        mapSize = mapLength;
        this.asteroidRadius = asteroidRadius;
        gridSize = gridCellSize;
        gridRadius = gridSize / 2;
        gridRadiusSqr = gridRadius * gridRadius;
        asteroidRadiusSqr = asteroidRadius * asteroidRadius;
        asteroidCount = mapLength * mapLength;
        realMapSize = GridCell.map.gridSize * GridCell.map.mapSize;

        asteroids = new Asteroid[mapSize * mapSize];
        map = new GridCell[mapSize, mapSize];

        int asteroidNum = 0;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                
                map[x, y] = new GridCell(x,y);
                asteroids[asteroidNum] = new Asteroid(map[x, y]);
                asteroidNum++;
            }
        }
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                map[x, y].SetNeighbours();
            }
        }
    }

    public void NextStep(float timePassed)
    {
        int asteroidNum = 0;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                
                asteroids[asteroidNum].Move(timePassed);
                map[x, y].CheckAsteroids();
                asteroidNum++;
            }
        }

    }



}
