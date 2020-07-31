using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public enum EntityType
{
    None = 0,
    Asteroid = 1,
    MedicalCanister = 2,
    Alien = 3
}

public class ProceduralGeneration : MonoBehaviour
{
    public bool drawGizmos = false; // DEBUG ONLY
    
    // Unit cubes should evenly subdivide chunk, and chunks should evenly subdivide level.
    public Vector3Int levelDimensions;
    public int chunkSize;
    public int unitCubeSize;

    // Indices match up to values of EntityType enum
    public float[] spawnProbability;
    public Vector2 asteroidSizeRange;
    public float viewDistance = 500f;
    
    // public GameObject blackHolePrefab;
    // public GameObject exitPortalPrefab;
    public GameObject asteroidPrefab;
    public GameObject medicalCanisterPrefab;
    public GameObject alienPrefab;

    private GameObject asteroidCollection;
    private GameObject medicalCanisterCollection;
    private GameObject alienCollection;
    
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
    private float asteroidBaseRadius;
    private float alienRadius;

    private void Awake()
    {
        asteroidCollection = GameObject.FindGameObjectWithTag("AsteroidCollection");
        medicalCanisterCollection = GameObject.FindGameObjectWithTag("MedicalCanisterCollection");
        alienCollection = GameObject.FindGameObjectWithTag("AlienCollection");
        
        canisterRadius = medicalCanisterPrefab.GetComponent<Renderer>().bounds.size.x / 2;
        asteroidBaseRadius = asteroidPrefab.GetComponent<Renderer>().bounds.size.x / 2;
        alienRadius = alienPrefab.GetComponentInChildren<Renderer>().bounds.size.y / 2; // TODO: verify this is the longest axis of the alien prefab

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
        var radius = obj.GetComponentInChildren<Renderer>().bounds.size.x / 2;
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
                    // The offset to the top-left of the chunk and to the center of the first unit cube,
                    // in the top left corner of the chunk
                    var unitCubeOffset = chunkStart + Vector3.one * (unitCubeSize / 2f);
                    var unitCubeCenter = (new Vector3Int(x, y, z) * unitCubeSize) + unitCubeOffset;

                    // TODO: Clean this up
                    var rands = new float[spawnProbability.Length];
                    for (var i = 0; i < spawnProbability.Length; i++)
                    {
                        rands[i] = Random.value;
                    }

                    var actionMap = new Dictionary<EntityType, Action>()
                    {
                        [EntityType.None] = () => {}, // Nothing
                        [EntityType.Asteroid] = () => GenerateChunkAsteroids(unitCubeCenter),
                        [EntityType.MedicalCanister] = () => GenerateChunkMedicalCanisters(unitCubeCenter),
                        [EntityType.Alien] = () => GenerateChunkAliens(unitCubeCenter)
                    };
                    for (var i = 0; i < rands.Length; i++)
                    {
                        var probability = spawnProbability[i];
                        var rand = rands[i];
                        var action = actionMap[(EntityType)i];
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
        var asteroidSize = Random.Range(asteroidSizeRange[0], asteroidSizeRange[1]);
        var asteroidPosition = RandomEntityPosition(unitCubeCenter, asteroidSize * asteroidBaseRadius);
        PlaceEntity(asteroidPosition, asteroidPrefab, asteroidCollection.transform, asteroidSize);
    }
    
    private void GenerateChunkAliens(Vector3 unitCubeCenter)
    {
        var alienPosition = RandomEntityPosition(unitCubeCenter, alienRadius);
        PlaceEntity(alienPosition, alienPrefab, alienCollection.transform);
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