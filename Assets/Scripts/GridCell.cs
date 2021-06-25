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

    void TransferAsteroid(Asteroid toMove, GridCell target)
    {
        target.asteroids.Add(toMove);
        asteroids.Remove(toMove);
    }

    public void SetNeighbours(Map map)
    {
        int i = 0;
        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x != 0 && y != 0)
                {
                    int nPosX = position.x + x;
                    int nPosY = position.y + y;
                    if (nPosX < 0 || nPosX == map.mapSize)
                    {
                        nPosX = map.mapSize - Mathf.Abs(nPosX);
                    }
                    if (nPosY < 0 || nPosY == map.mapSize)
                    {
                        nPosY = map.mapSize - Mathf.Abs(nPosY);
                    }
                    neighbours[i] = map.map[nPosX, nPosY];
                    i++;
                }
            }
        }
    }

    public void CheckAsteroids()
    {
        foreach (GridCell nGrid in neighbours)
        {
            foreach (Asteroid a1 in nGrid.asteroids)
            {
                foreach (Asteroid a2 in asteroids)
                {
                    if (a1 == a2)
                    {
                        continue;
                    }
                    if ((a1.position - a2.position).sqrMagnitude < map.asteroidSizeSqr)
                    {
                        Debug.Log("Boom");
                    }
                }
            }
        }
    }
}




