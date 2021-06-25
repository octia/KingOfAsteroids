using System.Collections.Generic;
using UnityEngine;


public class Asteroid
{
    public Vector2 position = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    private GridCell host;
    public bool instantinated = false;
    private bool toReplaceHost = false;

    public Asteroid(GridCell hostGrid)
    {
        host = hostGrid;
        hostGrid.asteroids.Add(this);
        position = (Vector2)hostGrid.position * GridCell.map.gridSize;
        SetRandomVelocity(GridCell.map.asteroidMinSpeed, GridCell.map.asteroidMaxSpeed);
        
    }

    private int selectedNeighbour;
    private Vector2 distFromCenter;
    public void Move(float stepSize)
    {
        position += velocity * stepSize;

        
        
        distFromCenter = position - host.realPos;

        /*Cell neighbour numbers are:
        0 1 2
        3 4 5
        6 7 8

        4 is the number of the current Cell
        */

        
        float innerGridRadius = GridCell.map.gridRadius - GridCell.map.asteroidRadius;
        float outerGridRadius = GridCell.map.gridRadius + GridCell.map.asteroidRadius;
        if (distFromCenter.sqrMagnitude > innerGridRadius* innerGridRadius)
        {
            #region removing asteroid if outside of cell range
            if (distFromCenter.magnitude > outerGridRadius)
            {
                toReplaceHost = true;
            }
            #endregion

            #region adding asteroid to another grid cell
            selectedNeighbour = 4;
            if (distFromCenter.y > innerGridRadius)
            {
                selectedNeighbour -= 3;
            }
            else
            {
                if (distFromCenter.y < -innerGridRadius)
                {
                    selectedNeighbour += 3;
                }
            }
            if (distFromCenter.x > innerGridRadius)
            {
                selectedNeighbour += 1;
            }
            else
            {
                if (distFromCenter.x < -innerGridRadius)
                {
                    selectedNeighbour -= 1;
                }
            }
            if (selectedNeighbour >= 4)
            {
                selectedNeighbour--;
            }
            host.neighbours[selectedNeighbour].AddAsteroid(this);
            if (toReplaceHost)
            {
                host.RemoveAsteroid(this);
                host = host.neighbours[selectedNeighbour];
            }
            #endregion

        }

        CheckOverflow();

    }

    private void CheckOverflow()
    {
        if (position.x < 0)
        {
            position.x += GridCell.map.realMapSize;
            instantinated = false;
        }
        if (position.y < 0)
        {
            position.y += GridCell.map.realMapSize;
            instantinated = false;
        }
        if (position.x > GridCell.map.realMapSize)
        {
            position.x -= GridCell.map.realMapSize;
            instantinated = false;
        }
        if (position.y > GridCell.map.realMapSize)
        {
            position.y -= GridCell.map.realMapSize;
            instantinated = false;
        }
    }

    private void SetRandomVelocity(float minSpeed, float maxSpeed)
    {
        velocity = Random.insideUnitCircle.normalized * Random.Range(minSpeed, maxSpeed);
    }

}