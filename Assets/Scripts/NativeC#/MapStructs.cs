using UnityEngine;
using System;

[Serializable]
public struct SaveableAsteroidData
{
    public float posX;
    public float posY;
    public float velX;
    public float velY;
    public int hPosX;
    public int hPosY;
}

public struct AsteroidData
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2Int hostPosition;
}

public struct MoveResult
{
    public int moveToNeighbour;
    public int toReplaceHost;
    public int isVisible;
}

public struct PotentialCollisions
{
    public int asteroidNum;
}

public struct CollisionCheck
{
    public int asteroidCount;
}

public struct CollisionResult
{
    public float collisionHappened;
}

public enum Visibility // must be kept synchronised with defined visibility values in the compute shader
{
    invisible = 0, turnedToInvisible = 1, defaultVisibility = 2, turnedToVisible = 3, visible = 4,
}

