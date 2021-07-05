using System.Collections.Generic;
using UnityEngine;



public class Asteroid
{
    public int aId;

    public Asteroid(int x, int y, int asteroidNum, ref AsteroidData data)
    {
        aId = asteroidNum;
        data.position = new Vector2(x, y);
        data.hostPosition = new Vector2Int(x, y);
        SetRandomVelocity(ref data, Map.map.asteroidVelMin, Map.map.asteroidVelMax);
        
    }

    public static void SetRandomVelocity(ref AsteroidData toSet, float minVel, float maxVel)
    {
        toSet.velocity = Random.insideUnitCircle.normalized * Random.Range(minVel, maxVel);
    }

}