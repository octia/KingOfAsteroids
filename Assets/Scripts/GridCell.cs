using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public ISet<Asteroid> asteroids;
    public GridCell[] neighbours;
    public static Map map;
    public Vector2Int position = Vector2Int.zero;
    public Vector2 realPos = Vector2.zero;

    public GridCell(int x, int y)
    {
        asteroids = new HashSet<Asteroid>();
        neighbours = new GridCell[8];
        position.x = x;
        position.y = y;
        realPos = (Vector2)position * map.gridSize;
    }

    public void AddAsteroid(Asteroid toAdd)
    {
        if (!asteroids.Contains(toAdd))
        {
            asteroids.Add(toAdd);
        }
    }

    public void RemoveAsteroid(Asteroid toRemove)
    {
        asteroids.Remove(toRemove);
    }

    public void SetNeighbours()
    {
        int i = 0;
        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x != 0 || y != 0)
                {
                    int nPosX = position.x + x;
                    int nPosY = position.y + y;
                    if (nPosX < 0)
                    {
                        nPosX += map.mapSize;
                    }
                    if (nPosX >= map.mapSize)
                    {
                        nPosX -= map.mapSize;
                    }
                    if (nPosY < 0)
                    {
                        nPosY += map.mapSize;
                    }
                    if (nPosY >= map.mapSize)
                    {
                        nPosY -= map.mapSize;
                    }

                    neighbours[i] = map.map[nPosX, nPosY];
                    i++;
                }
            }
        }
    }

    public void CheckAsteroids()
    {
        //foreach (Asteroid a in asteroids)
        {
            
        }

    }
}




