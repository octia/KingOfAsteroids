using System.Collections.Generic;
using UnityEngine;


public class Asteroid
{
    public Vector2 position = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    private GridCell host;
    public bool instantinated = false;



    public Asteroid(GridCell hostGrid)
    {
        host = hostGrid;
        position = hostGrid.position;
        SetRandomVelocity(GridCell.map.asteroidMinSpeed, GridCell.map.asteroidMaxSpeed);
    }

    public void Move(float stepSize)
    {
        position += velocity * stepSize;

        float gridRadius = GridCell.map.gridSize / 2;
        
        Vector2 distFromCenter = ((Vector2)host.position * gridRadius) - position;

        /*Cell neighbour numbers are:
        1 2 3
        4 5 6
        7 8 9

        5 is the number of the current Cell
        */

        int selectedNeighbour = 5;

        if (distFromCenter.sqrMagnitude > gridRadius * gridRadius)
        {
            if (distFromCenter.y > gridRadius)
            {
                selectedNeighbour -= 3;
            }
            else
            {
                if (distFromCenter.y < -gridRadius)
                {
                    selectedNeighbour += 3;
                }
            }
            if (distFromCenter.x > gridRadius)
            {
                selectedNeighbour -= 3;
            }
            else
            {
                if (distFromCenter.x < -gridRadius)
                {
                    selectedNeighbour += 3;
                }
            }
        }
    }


    private void SetRandomVelocity(float minSpeed, float maxSpeed)
    {
        velocity = Random.insideUnitCircle.normalized * Random.Range(minSpeed, maxSpeed);
    }

}