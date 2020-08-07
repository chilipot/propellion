using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public enum EntityType
{
    None = 0,
    Asteroid = 1,
    MedicalCanister = 2,
    Alien = 3,
    AtmosphericAsteroid = 4
}

public class ProceduralGeneration : MonoBehaviour
{
    public bool drawGizmos = false; // DEBUG ONLY
    
    // Unit cubes should evenly subdivide chunk, and chunks should evenly subdivide level.
    public Vector3Int levelDimensions;
    public int chunkSize;
    public int unitCubeSize;

    [Tooltip("Number of unit cubes between black hole and player start")]
    public int blackHoleBufferLength = 5;
    [Tooltip("Number of unit cubes that exist past the exit portal")]
    public int exitBufferLength = 5;

    // Indices match up to values of EntityType enum
    public float[] spawnProbability;
    [Tooltip("Will there be a black hole to chase the player")]
    public bool chaseLevel = true;
    
    public Vector2 asteroidSizeRange;
    public float viewDistance = 500f;

    public GameObject blackHolePrefab;
    public GameObject exitPortalPrefab;
    public GameObject asteroidPrefab;
    public GameObject atmosphericAsteroidPrefab;
    public GameObject medicalCanisterPrefab;

    private GameObject asteroidsCollection;
    private GameObject atmosphericAsteroidsCollection;
    private GameObject medicalCanisterCollection;
    
    private Vector3Int numChunksInLevel;
    private Vector3Int numUnitCubesInChunk;

    // index of the unit cube containing the player's start position
    private Vector3Int playerStartUnitCube;

    private CullingGroup cullGroup;
    private BoundingSphere[] bounds;
    private Dictionary<int, GameObject> entityLookup;
    private int EntityCount => entityLookup.Count;

    private Dictionary<int, Transform> entityTransforms;
    
    private Dictionary<int, int> entityIdToIndex;
    private Dictionary<int, int> entityIndexToId;
    
    public static bool FinishedGenerating = false;
    private int maxEntities;

    private float canisterRadius;
    private float asteroidBaseRadius;

    private UIManager ui;

    private void Awake()
    {
        FinishedGenerating = false;
        asteroidsCollection = GameObject.FindGameObjectWithTag("AsteroidCollection");
        medicalCanisterCollection = GameObject.FindGameObjectWithTag("MedicalCanisterCollection");
        atmosphericAsteroidsCollection = GameObject.FindGameObjectWithTag("AtmosphericAsteroidsCollection");
        
        canisterRadius = medicalCanisterPrefab.GetComponent<Renderer>().bounds.size.x / 2;
        asteroidBaseRadius = asteroidPrefab.GetComponent<Renderer>().bounds.size.x / 2;

        numChunksInLevel = levelDimensions / chunkSize;
        numUnitCubesInChunk = Vector3Int.one * (chunkSize / unitCubeSize);

        // var entitiesPerChunk = 1; TODO: Make this more extendable
        maxEntities = numChunksInLevel.x * numUnitCubesInChunk.x * numChunksInLevel.y * numUnitCubesInChunk.y * numChunksInLevel.z * numUnitCubesInChunk.z  * 1; // 1 entity per unit cube right now

        // Culling Setup
        var mainCam = Camera.main;
        cullGroup = new CullingGroup {targetCamera = mainCam};
        cullGroup.SetDistanceReferencePoint(mainCam.transform);
        cullGroup.SetBoundingDistances(new[] {viewDistance, float.PositiveInfinity});
        bounds = new BoundingSphere[maxEntities];
        cullGroup.SetBoundingSpheres(bounds);
        cullGroup.SetBoundingSphereCount(0);
        cullGroup.onStateChanged += CullingStateChanged;
        entityLookup = new Dictionary<int, GameObject>();
        entityTransforms = new Dictionary<int, Transform>();
        entityIdToIndex = new Dictionary<int, int>();
        entityIndexToId = new Dictionary<int, int>();

        StartCoroutine(nameof(GenerateEntities));
        StartCoroutine(nameof(UpdateCulledObjectBounds));
    }

    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        ui.SetLevelStatus(UIManager.LevelStatus.Loading);
    }

    private void AddEntity(GameObject obj)
    {
        var id = obj.GetInstanceID();
        var tf = obj.transform;
        var radius = obj.GetComponent<Renderer>().bounds.size.x / 2;
        entityLookup.Add(id, obj);
        var index = EntityCount - 1;
        bounds[index] = new BoundingSphere {position = tf.position, radius = radius};
        entityTransforms.Add(id, tf);
        entityIndexToId.Add(index, id);
        entityIdToIndex.Add(id, index);
        cullGroup.SetBoundingSphereCount(EntityCount);
    }

    public void RemoveEntity(GameObject obj)
    {
        var id = obj.GetInstanceID();
        var index = entityIdToIndex[id];
        entityIdToIndex.Remove(id);
        entityIndexToId.Remove(index);
        entityTransforms.Remove(id);
        
        var moveIndex = EntityCount - 1;
        
        // If the index isn't the last one in the array,
        // move the last element into the index of the deleted entity
        if (index != moveIndex)
        {
            bounds[index] = bounds[moveIndex];
            var moveId = entityIndexToId[moveIndex];
            entityIdToIndex[moveId] = index;
            entityIndexToId.Remove(moveIndex);
            entityIndexToId.Add(index, moveId);
        }
        entityLookup.Remove(id);
        cullGroup.SetBoundingSphereCount(EntityCount);
    }

    private void SetupBaseObjects()
    {
        var blackHolePosition = new Vector3(levelDimensions.x / 2f, levelDimensions.y / 2f, unitCubeSize / 2f);
        if (chaseLevel)
        {
            Instantiate(blackHolePrefab, blackHolePosition, Quaternion.identity);
        }

        var player = GameObject.FindWithTag("Player");
        var playerStartPosition = blackHolePosition + blackHoleBufferLength * unitCubeSize * Vector3.forward;
        player.transform.position = playerStartPosition;
        var playerStartUnitCubeAsFloat = playerStartPosition / unitCubeSize;
        playerStartUnitCube = new Vector3Int(
            Mathf.FloorToInt(playerStartUnitCubeAsFloat.x), 
            Mathf.FloorToInt(playerStartUnitCubeAsFloat.y), 
            Mathf.FloorToInt(playerStartUnitCubeAsFloat.z)
        );
        
        var exitPortalPosition = new Vector3(levelDimensions.x / 2f, levelDimensions.y / 2f, levelDimensions.z - exitBufferLength * unitCubeSize - unitCubeSize / 2f);
        Instantiate(exitPortalPrefab, exitPortalPosition, Quaternion.identity);
    }
    
    private IEnumerator GenerateEntities()
    {
        SetupBaseObjects();
        yield return null;
        for (var x = 0; x < numChunksInLevel.x; x++)
        {
            for (var y = 0; y < numChunksInLevel.y; y++)
            {
                for (var z = 0; z < numChunksInLevel.z; z++)
                {
                    GenerateChunk(new Vector3Int(x, y, z));
                    yield return null;
                }
            }
        }
        FinishedGenerating = true;
        ui.SetLevelStatus(UIManager.LevelStatus.Playing);
        // TODO: Remove when there's a load screen
        Debug.Log($"Finished Generating: Up to {maxEntities} Entities"); 
    }

    private void GenerateChunk(Vector3Int chunkIndex)
    {
        var unitCubeIndexOffset = chunkIndex * numUnitCubesInChunk;
        var chunkStart = chunkIndex * chunkSize;
        for (var x = 0; x < numUnitCubesInChunk.x; x++)
        {
            for (var y = 0; y < numUnitCubesInChunk.y; y++)
            {
                for (var z = 0; z < numUnitCubesInChunk.z; z++)
                {
                    var unitCubeIndex = new Vector3Int(x, y, z);
                    if (unitCubeIndex + unitCubeIndexOffset == playerStartUnitCube) continue;
                    
                    // The offset to the top-left of the chunk and to the center of the first unit cube,
                    // in the top left corner of the chunk
                    var unitCubeOffset = chunkStart + Vector3.one * (unitCubeSize / 2f);
                    var unitCubeCenter = unitCubeIndex * unitCubeSize + unitCubeOffset;

                    RandomlyGenerateUnitCubeEntity(unitCubeCenter);
                }
            }
        }
    }

    private void RandomlyGenerateUnitCubeEntity(Vector3 unitCubeCenter)
    {
        // TODO: Clean this up
        var actionMap = new Dictionary<EntityType, Action>
        {
            [EntityType.None] = () => {}, // Nothing
            [EntityType.Asteroid] = () => GenerateChunkAsteroids(unitCubeCenter),
            [EntityType.MedicalCanister] = () => GenerateChunkMedicalCanisters(unitCubeCenter),
            [EntityType.AtmosphericAsteroid] = () => GenerateChunkAtmosphericAsteroids(unitCubeCenter)
        };

        var choices = new WeightedRandomBag<EntityType>();
        foreach (var mapping in actionMap)
        {
            var entity = mapping.Key;
            var index = (int) entity;
            var weight = spawnProbability[index];
            choices.AddEntry(entity, weight);
        }

        var action = actionMap[choices.GetRandom()];
        action();
    }

    private void GenerateChunkMedicalCanisters(Vector3 unitCubeCenter)
    {
        var canisterPosition = RandomEntityPosition(unitCubeCenter, canisterRadius);
        PlaceEntity(canisterPosition, medicalCanisterPrefab, medicalCanisterCollection.transform);
    }
    
    private void GenerateChunkAsteroids(Vector3 unitCubeCenter)
    {
        var asteroidSize = Random.Range(asteroidSizeRange[0], asteroidSizeRange[1]);
        var asteroidPosition = RandomEntityPosition(unitCubeCenter, asteroidSize * asteroidBaseRadius);
        PlaceEntity(asteroidPosition, asteroidPrefab, asteroidsCollection.transform, asteroidSize);
    }    
    
    private void GenerateChunkAtmosphericAsteroids(Vector3 unitCubeCenter)
    {
        var asteroidSize = asteroidSizeRange[1];//Random.Range(asteroidSizeRange[0], asteroidSizeRange[1]);
        var asteroidPosition = RandomEntityPosition(unitCubeCenter, asteroidSize * asteroidBaseRadius);
        PlaceEntity(asteroidPosition, atmosphericAsteroidPrefab, atmosphericAsteroidsCollection.transform, asteroidSize);
    }

    private GameObject PlaceEntity(Vector3 position, GameObject prefab, Transform parent, float size = 1f)
    {
        var obj = Instantiate(prefab, position, Random.rotation, parent);
        obj.transform.localScale *= size;
        obj.SetActive(false);
        AddEntity(obj);
        return obj;
    }

    private Vector3 RandomEntityPosition(Vector3 unitCubeCenter, float radius)
    {
        var placementVariationRange = (unitCubeSize / 2f)  - radius;
        var placementVariation = new Vector3(
            Random.Range(-placementVariationRange, placementVariationRange),
            Random.Range(-placementVariationRange, placementVariationRange),
            Random.Range(-placementVariationRange, placementVariationRange)
        );
        var position = unitCubeCenter + placementVariation;
        return position;
    }

    private void CullingStateChanged(CullingGroupEvent evt)
    {
        var entity = entityLookup[entityIndexToId[evt.index]];
        if (evt.currentDistance <= 0 && !entity.activeInHierarchy)
        {
            entity.SetActive(true);
        }
        else if (evt.currentDistance > 0 && entity.activeInHierarchy)
        {
            entity.SetActive(false);
        }
    }

    private IEnumerator UpdateCulledObjectBounds()
    {
        while (true)
        {
            for (var i = 0; i < EntityCount; i++)
            {
                bounds[i].position = entityTransforms[entityIndexToId[i]].position;
                if (i % 25 == 0)
                {
                    yield return null;
                }
            }
            yield return new WaitForSecondsRealtime(2);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        Gizmos.color = Color.red;
        for (var i = 0; i < EntityCount; i++)
        {
            var boundingSphere = bounds[i];
            Gizmos.DrawWireSphere(boundingSphere.position, boundingSphere.radius);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (cullGroup == null) return;
        cullGroup.Dispose();
        cullGroup = null;
    }
}