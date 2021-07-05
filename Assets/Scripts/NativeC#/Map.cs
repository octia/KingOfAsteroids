using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Map
{

    public static Map map;


    public float asteroidRadius;
    public float asteroidRadiusSqr;

    

    public float asteroidVelMax;
    public float asteroidVelMin;


    public int mapSize;
    public int gridCount;

    private float visibleSizeX = 5;
    private float visibleSizeY = 5;

    private int maxCollisionsPerGrid = 7;

    private float asteroidCD = 1f;

    public float timeStep = Time.fixedDeltaTime;


    private Asteroid[] asteroids;
    private AsteroidData[] asteroidData;

    private MoveResult[] moveResults;
    private CollisionCheck[] collisionData;
    private CollisionResult[] collisionResults;
    private PotentialCollisions[] potentialCollisions;


    private AsteroidManager manager;


    private ComputeShader asteroidCompute;

    private ComputeBuffer asteroidDataBuffer;
    private ComputeBuffer moveResultBuffer;
    private ComputeBuffer collisionBuffer;
    private ComputeBuffer colResultBuffer;
    private ComputeBuffer potColBuffer;

    private int MoveKernel;
    private int CollideKernel;

    private int asteroidToDestroy = -1; // todo: change to array


    public Map(AsteroidManager asteroidManager)
    {
        map = this;

        CopyVarsFromManager(asteroidManager);

        #region Initializing arrays
        asteroids = new Asteroid[gridCount];
        asteroidData = new AsteroidData[gridCount];

        moveResults = new MoveResult[gridCount];
        collisionData = new CollisionCheck[gridCount];
        collisionResults = new CollisionResult[gridCount];
        potentialCollisions = new PotentialCollisions[gridCount * maxCollisionsPerGrid];
        #endregion


        #region Setting up compute buffers for the compute shader

        int vec2Size = sizeof(float) * 2;
        int intSize = sizeof(int);
        int asteroidSize = vec2Size * 2 + intSize * 2;
        int moveResultSize = intSize + intSize + intSize;
        int collisionSize = intSize * 8;
        int colResultSize = intSize;

        asteroidDataBuffer = new ComputeBuffer(gridCount, asteroidSize);
        asteroidDataBuffer.name = "asteroidDataBuffer";
        moveResultBuffer = new ComputeBuffer(gridCount, moveResultSize);
        moveResultBuffer.name = "moveResultBuffer";
        collisionBuffer = new ComputeBuffer(gridCount, collisionSize);
        collisionBuffer.name = "collisionBuffer";
        colResultBuffer = new ComputeBuffer(gridCount, colResultSize);
        colResultBuffer.name = "colResultBuffer";
        potColBuffer = new ComputeBuffer(gridCount * maxCollisionsPerGrid, intSize);
        potColBuffer.name = "potColBuffer";
        #endregion

        MoveKernel = asteroidCompute.FindKernel("Move");
        CollideKernel = asteroidCompute.FindKernel("Collide");

        #region setting compute shader variables
        asteroidCompute.SetFloat("stepSize", Time.fixedDeltaTime);
        asteroidCompute.SetFloat("visibleSizeX", visibleSizeX);
        asteroidCompute.SetFloat("visibleSizeY", visibleSizeY); 
        asteroidCompute.SetFloat("minSpeed", asteroidVelMin); 
        asteroidCompute.SetFloat("maxSpeed", asteroidVelMax);

        asteroidCompute.SetFloat("asteroidSize", asteroidRadius);
        asteroidCompute.SetFloat("respawnCooldown", asteroidCD);
        asteroidCompute.SetFloats("playerSpeed", 0.0f, 0.0f);
        asteroidCompute.SetInt("asteroidCount", gridCount);
        asteroidCompute.SetInt("maxCollisions", maxCollisionsPerGrid);
        asteroidCompute.SetInt("mapSize", mapSize);
        asteroidCompute.SetInt("destroyedAsteroid", -1);
        #endregion

        int asteroidNum = 0;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                moveResults[asteroidNum].isVisible = (int)Visibility.invisible;
                moveResults[asteroidNum].moveToNeighbour = 4;
                moveResults[asteroidNum].toReplaceHost = 4;
                asteroids[asteroidNum] = new Asteroid(x, y, asteroidNum, ref asteroidData[asteroidNum]); 
                if (x == mapSize / 2 && y == mapSize / 2)
                {
                    asteroidData[asteroidNum].position = Vector2.one * -100;
                    asteroidData[asteroidNum].velocity = Vector2.zero;
                }
                for (int i = 0; i < maxCollisionsPerGrid; i++)
                {
                    potentialCollisions[asteroidNum * maxCollisionsPerGrid + i].asteroidNum = -1;
                }
                collisionData[asteroidNum].asteroidCount = 0;
                collisionResults[asteroidNum].collisionHappened = -1.0f;
                asteroidNum++;
            }
        }

        #region preparing buffers for dispatch
        asteroidDataBuffer.SetData(asteroidData);
        colResultBuffer.SetData(collisionResults);
        collisionBuffer.SetData(collisionData);
        potColBuffer.SetData(potentialCollisions);
        moveResultBuffer.SetData(moveResults);

        asteroidCompute.SetBuffer(CollideKernel, "asteroids", asteroidDataBuffer);
        asteroidCompute.SetBuffer(CollideKernel, "collisionChecks", collisionBuffer);
        asteroidCompute.SetBuffer(CollideKernel, "collisionResults", colResultBuffer);
        asteroidCompute.SetBuffer(CollideKernel, "potentialCollisions", potColBuffer);
        asteroidCompute.SetBuffer(MoveKernel, "asteroids", asteroidDataBuffer);
        asteroidCompute.SetBuffer(MoveKernel, "moveResults", moveResultBuffer);
        asteroidCompute.SetBuffer(MoveKernel, "collisionChecks", collisionBuffer);
        asteroidCompute.SetBuffer(MoveKernel, "collisionResults", colResultBuffer);
        asteroidCompute.SetBuffer(MoveKernel, "potentialCollisions", potColBuffer);
        #endregion

        asteroidCompute.Dispatch(CollideKernel, gridCount / 32, 1, 1);


    }

    public void DisposeBuffers()
    {
        asteroidDataBuffer.Dispose();
        moveResultBuffer.Dispose();
        collisionBuffer.Dispose();
        colResultBuffer.Dispose();
        potColBuffer.Dispose();
    }

    public void NextStep(float timePassed)
    {
        colResultBuffer.GetData(collisionResults); 
        if (asteroidToDestroy != -1)
        {
            asteroidCompute.SetInt("destroyedAsteroid", asteroidToDestroy);
            collisionResults[asteroidToDestroy].collisionHappened = 1;
            asteroidToDestroy = -1;
        }
        asteroidCompute.SetFloats("playerSpeed", manager.player.playerVel.x, manager.player.playerVel.y);
        
        // TODO: reduce amount of SetData calls, for improved preformance (collapse collisionData & potentialcollisions into one array)
        potColBuffer.SetData(potentialCollisions);
        collisionBuffer.SetData(collisionData);
        asteroidCompute.Dispatch(MoveKernel, gridCount / 32, 1, 1);
        moveResultBuffer.GetData(moveResults);
        asteroidDataBuffer.GetData(asteroidData);

        asteroidCompute.Dispatch(CollideKernel, gridCount / 32, 1, 1);
        for (int i = 0; i < gridCount; i++)
        {
            if (moveResults[i].isVisible >= (int)Visibility.defaultVisibility)
            {
                if (collisionResults[i].collisionHappened > 0)
                {
                    manager.DestroyAsteroid(i);
                    moveResults[i].isVisible = (int)Visibility.turnedToInvisible;
                }
            }


            if (collisionResults[i].collisionHappened <= 0)
            {
                if (moveResults[i].isVisible >= (int)Visibility.turnedToVisible)
                {
                    if (i != asteroidToDestroy)
                    {
                        manager.SpawnAsteroid(i, asteroidData[i]);
                    }
                }
                if (moveResults[i].isVisible == (int)Visibility.turnedToInvisible)
                {
                    manager.DestroyAsteroid(i);
                }
            }
            else
            {
                if (moveResults[i].isVisible == (int)Visibility.turnedToInvisible)
                {
                    manager.DestroyAsteroid(i);
                }
            }

        }

    }

    private void CopyVarsFromManager(AsteroidManager asteroidManager)
    {
        float scale = asteroidManager.simToWorldScale;

        manager = asteroidManager;
        asteroidCompute = asteroidManager.asteroidCompute;

        mapSize = asteroidManager.mapSize;
        asteroidRadius = asteroidManager.asteroidRadius / scale;

        asteroidRadiusSqr = asteroidRadius * asteroidRadius;
        gridCount = mapSize * mapSize;

        asteroidVelMax = asteroidManager.asteroidVelMax / scale;
        asteroidVelMin = asteroidManager.asteroidVelMin / scale;

        visibleSizeX = asteroidManager.renderRangeX;
        visibleSizeY = asteroidManager.renderRangeY;

    }

    public void DestroyAsteroid(int id)
    {
        asteroidToDestroy = id;
    }

    public bool ReturnFinalAsteroidArray(out AsteroidData[] asteroidDataToReturn)
    {
        asteroidDataBuffer.GetData(asteroidData);
        asteroidDataToReturn = asteroidData;
        return true;
    }

    public void LoadAsteroidData(AsteroidData[] toLoad)
    {
        toLoad.CopyTo(asteroidData, 0);
        asteroidDataBuffer.SetData(asteroidData);
    }

}
