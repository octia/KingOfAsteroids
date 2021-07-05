using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class AsteroidManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject asteroidPrefab;


    [Header("GameObjects")]
    public GameObject playerGO;
    public GameObject scoreGO;
    public GameObject deathScreenGO;


    public ComputeShader asteroidCompute;

    public string saveFile = "saveFile.save";

    [HideInInspector] public PlayerManager player;
    private DeathScreenManager dsManager;
    private ScoreManager scoreManager;

    [Header("Simulation parameters")]

    [Range(4, 512)]
    public int mapSize = 160; // map height/length, in gridCells

    [Range(0.1f, 5f)]
    public float asteroidRadius = 1f;


    [Header("World parameters")]

    [Range(0.1f, 20f)]
    public float simToWorldScale = 5f;

    [Range(0f, 20f)]
    public float asteroidVelMax = 20f;

    [Range(0f, 10f)]
    public float asteroidVelMin = 3f;



    // dimensions of a rectangle in which objects will be rendered
    public float renderRangeX;
    public float renderRangeY;

    private Dictionary<int, GameObject> spawnedAsteroids;

    private Map map;

    private bool simulate = true;

    // Start is called before the first frame update
    void Start()
    {
        player = playerGO.GetComponent<PlayerManager>();

        map = new Map(this);
        LoadGame();

        player.transform.parent.transform.position += (Vector3)Vector2.one *simToWorldScale* mapSize/2;

        spawnedAsteroids = new Dictionary<int, GameObject>();

        scoreManager = scoreGO.GetComponent<ScoreManager>();
        dsManager = deathScreenGO.GetComponent<DeathScreenManager>();
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.orientation = ScreenOrientation.Landscape;
        }

    }

    private void FixedUpdate()
    {
        if (simulate)
        {
            map.NextStep(Time.fixedDeltaTime);
        }

    }


    public void SpawnAsteroid(int id, AsteroidData data)
    {
        GameObject spawnedAsteroid;
        if (!spawnedAsteroids.ContainsKey(id))
        {
            spawnedAsteroid = Instantiate(asteroidPrefab, transform);
            SpawnedAsteroid spawnedAsteroidComp = spawnedAsteroid.GetComponent<SpawnedAsteroid>();
            spawnedAsteroidComp.asteroidNumRef = id;
            spawnedAsteroidComp.asteroidManager = this;
            spawnedAsteroids.Add(id, spawnedAsteroid);
        }
        else
        {
            spawnedAsteroid = spawnedAsteroids[id];
        }
        spawnedAsteroid.transform.position = data.position * simToWorldScale;

    }

    public void DestroyAsteroid(int id, bool mapCommand = true, int pointsToAdd = 0)
    {
        if (spawnedAsteroids.ContainsKey(id))
        {
            Destroy(spawnedAsteroids[id]);
            spawnedAsteroids.Remove(id);
            if (!mapCommand)
            {
                Map.map.DestroyAsteroid(id);
            }
            if (pointsToAdd != 0)
            {
                scoreManager.IncreaseScore(pointsToAdd);
            }
        }
    }
    public void OnDestroy()
    {
        simulate = false;
        SaveGame();
        map.DisposeBuffers();
    }

    public void PlayerDeath()
    {
        simulate = false;
        map.NextStep(Time.fixedDeltaTime);
        deathScreenGO.SetActive(true);
        playerGO.SetActive(false);
    }

    public void PlayerResurrect()
    {
        simulate = true;
        scoreManager.ResetScore();
        deathScreenGO.SetActive(false); 
        playerGO.SetActive(true);
    }


    #region Converting asteroidData to serializable substitute
    // TODO: change AsteroidData into a serializable struct, eliminate the need for SaveableAsteroidData
    private SaveableAsteroidData[] AsteroidDataToSaveable(AsteroidData[] toConvert)
    {
        SaveableAsteroidData[] toReturn = new SaveableAsteroidData[toConvert.Length];
        for (int i = 0; i < toConvert.Length; i++)
        {
            toReturn[i].posX = toConvert[i].position.x;
            toReturn[i].posY = toConvert[i].position.y;
            toReturn[i].velX = toConvert[i].velocity.x;
            toReturn[i].velY = toConvert[i].velocity.y;
            toReturn[i].hPosX = toConvert[i].hostPosition.x;
            toReturn[i].hPosY = toConvert[i].hostPosition.y;
        }
        return toReturn;
    }
    private AsteroidData[] SavableToAsteroidData(SaveableAsteroidData[] toConvert)
    {
        AsteroidData[] toReturn = new AsteroidData[toConvert.Length];
        for (int i = 0; i < toConvert.Length; i++)
        {
            toReturn[i].position = Vector2.right * toConvert[i].posX + Vector2.up * toConvert[i].posY;
            toReturn[i].velocity = Vector2.right * toConvert[i].velX + Vector2.up * toConvert[i].velY;
            toReturn[i].hostPosition.x = toConvert[i].hPosY;
            toReturn[i].hostPosition.y = toConvert[i].hPosX;
        }
        return toReturn;
    }
    #endregion


    public void SaveGame()
    {
        AsteroidData[] data;
        Map.map.ReturnFinalAsteroidArray(out data);
        SaveableAsteroidData[] dataToSave = AsteroidDataToSaveable(data);
        using (Stream stream = File.Open(saveFile, FileMode.OpenOrCreate))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, dataToSave);
        }
        PlayerPrefs.SetInt("GameSaved", 1);
    }


    public bool LoadGame()
    {
        if (PlayerPrefs.GetInt("GameSaved",-1) != -1)
        {
            if (File.Exists(saveFile))
            {
                AsteroidData[] data;
                SaveableAsteroidData[] loadedData;

                using (Stream stream = File.Open(saveFile, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    loadedData = (SaveableAsteroidData[])binaryFormatter.Deserialize(stream);
                }
                data = SavableToAsteroidData(loadedData);
                if (data.Length == 0)
                {
                    Debug.LogError("SaveFile " + saveFile + " not found!");
                    PlayerPrefs.GetInt("GameSaved", 0);
                }
                map.LoadAsteroidData(data);
                return true;
            }
            else
            {
                PlayerPrefs.GetInt("GameSaved", 0);
                return false;
            }
        }
        else
        {
            return false;
        }
        
    }

}


