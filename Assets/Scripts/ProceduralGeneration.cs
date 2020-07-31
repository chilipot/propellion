using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public enum EntityType
{
    Asteroid = 0,
    MedicalCanister = 1,
    Alien = 2
}

public class ProceduralGeneration : MonoBehaviour
{
    // Unit cubes should evenly subdivide chunk, and chunks should evenly subdivide level.
    public Vector3Int levelDimensions;
    public int chunkSize;
    public int unitCubeSize;
    
    // The probability that given a unit cube will have an asteroid on whether the asteroid is actually placed
    public float asteroidDensity = 0.5f; 
    
    // Indices match up to values of EntityType enum
    public float[] spawnProbability;
    public Vector2 asteroidSizeRange;
    public float viewDistance = 500f;
    
    // public GameObject blackHolePrefab;
    // public GameObject exitPortalPrefab;
    public GameObject asteroidPrefab;
    public GameObject medicalCanisterPrefab;

    private GameObject asteroidsCollection;
    private GameObject medicalCanisterCollection;
    
    private Vector3Int numChunksInLevel;
    private Vector3Int numUnitCubesInChunk;

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

    private void Awake()
    {
        asteroidsCollection = GameObject.FindGameObjectWithTag("AsteroidCollection");
        medicalCanisterCollection = GameObject.FindGameObjectWithTag("MedicalCanisterCollection");
        
        canisterRadius = medicalCanisterPrefab.GetComponent<Renderer>().bounds.size.x;

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

    private void AddEntity(GameObject obj)
    {
        var id = obj.GetInstanceID();
        var tf = obj.transform;
        var radius = obj.GetComponent<Renderer>().bounds.size.x;
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

    private IEnumerator GenerateEntities()
    {
        for (var x = 0; x < numChunksInLevel.x; x++)
        {
            for (var y = 0; y < numChunksInLevel.y; y++)
            {
                for (var z = 0; z < numChunksInLevel.z; z++)
                {
                    var chunkStart = new Vector3Int(x, y, z) * chunkSize;
                    GenerateChunk(chunkStart);
                    yield return null;
                }
            }
        }
        FinishedGenerating = true;
        // TODO: Remove when there's a load screen
        Debug.Log($"Finished Generating: Up to {maxEntities} Entities"); 
    }

    private void GenerateChunk(Vector3Int chunkStart)
    {
        for (var x = 0; x < numUnitCubesInChunk.x; x++)
        {
            for (var y = 0; y < numUnitCubesInChunk.y; y++)
            {
                for (var z = 0; z < numUnitCubesInChunk.z; z++)
                {
                    // The offset from the center of the chunk to the center of the first unit cube,
                    // in the top left corner of the chunk
                    var unitCubeOffset = chunkStart - Vector3.one * (chunkSize / 2 - unitCubeSize / 2);
                    var unitCubeCenter = (new Vector3Int(x, y, z) * unitCubeSize) + unitCubeOffset;

                    // TODO: Clean this up
                    var rands = new float[spawnProbability.Length];
                    for (var i = 0; i < spawnProbability.Length; i++)
                    {
                        rands[i] = Random.value;
                    }

                    var actions = new List<Action>()
                    {
                        () => GenerateChunkAsteroids(unitCubeCenter),
                        () => GenerateChunkMedicalCanisters(unitCubeCenter)
                    };
                    for (var i = 0; i < spawnProbability.Length; i++)
                    {
                        var probability = spawnProbability[i];
                        var rand = rands[i];
                        var action = actions[i];
                        if (rand <= probability)
                        {
                            action();
                        }
                    }
                }
            }
        }
    }

    private void GenerateChunkMedicalCanisters(Vector3 unitCubeCenter)
    {
        var canisterPosition = RandomEntityPosition(unitCubeCenter, canisterRadius);
        PlaceEntity(canisterPosition, medicalCanisterPrefab, medicalCanisterCollection.transform);
    }
    
    private void GenerateChunkAsteroids(Vector3 unitCubeCenter)
    {
        if (Random.value > asteroidDensity) return;
        
        var asteroidSize = Random.Range(asteroidSizeRange[0], asteroidSizeRange[1]);
        var asteroidPosition = RandomEntityPosition(unitCubeCenter, asteroidSize);
        var asteroid = PlaceEntity(asteroidPosition, asteroidPrefab, asteroidsCollection.transform);
        asteroid.transform.localScale *= asteroidSize;
    }

    private GameObject PlaceEntity(Vector3 position, GameObject prefab, Transform parent)
    {
        var obj = Instantiate(prefab, position, Random.rotation, parent);
        obj.SetActive(false);
        AddEntity(obj);
        return obj;
    }

    private Vector3 RandomEntityPosition(Vector3 unitCubeCenter, float size)
    {
        var placementVariationRange = (unitCubeSize - size) / 2f;
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

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (cullGroup == null) return;
        cullGroup.Dispose();
        cullGroup = null;
    }
}