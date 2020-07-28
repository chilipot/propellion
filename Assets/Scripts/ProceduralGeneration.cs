using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class ProceduralGeneration : MonoBehaviour
{
    // Unit cubes should evenly subdivide chunk, and chunks should evenly subdivide level.
    public Vector3Int levelDimensions;
    public int chunkSize;
    public int unitCubeSize;
    public float asteroidDensity = 0.5f;
    public Vector2 asteroidSizeRange;
    public float viewDistance = 500f;
    
    // public GameObject blackHolePrefab;
    // public GameObject exitPortalPrefab;
    public GameObject asteroidPrefab;

    private GameObject asteroidsCollection;
    
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

    private void Awake()
    {
        asteroidsCollection = GameObject.FindGameObjectWithTag("Asteroids");

        numChunksInLevel = levelDimensions / chunkSize;
        numUnitCubesInChunk = Vector3Int.one * (chunkSize / unitCubeSize);

        // var asteroidsPerChunk = 1; TODO: Make this more extendable
        maxEntities = numChunksInLevel.x * numUnitCubesInChunk.x * numChunksInLevel.y * numUnitCubesInChunk.y * numChunksInLevel.z * numUnitCubesInChunk.z  * 1; // 1 asteroid per unit cube right now
        Debug.Log($"Max Entities: {maxEntities}");
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

        StartCoroutine(nameof(GenerateAsteroidField));
        StartCoroutine(nameof(UpdateCulledObjectBounds));
    }

    public void AddEntity(GameObject obj)
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

    private IEnumerator GenerateAsteroidField()
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
        Debug.Log("Finished Generating!!!");
    }

    private void GenerateChunk(Vector3Int chunkStart)
    {
        for (var x = 0; x < numUnitCubesInChunk.x; x++)
        {
            for (var y = 0; y < numUnitCubesInChunk.y; y++)
            {
                for (var z = 0; z < numUnitCubesInChunk.z; z++)
                {
                    var unitCubeOffset = chunkStart - Vector3.one * (chunkSize / 2 - unitCubeSize / 2);
                    var unitCubeCenter = (new Vector3Int(x, y, z) * unitCubeSize) + unitCubeOffset;
                    GenerateChunkAsteroids(unitCubeCenter);
                }
            }
        }
    }
    
    private void GenerateChunkAsteroids(Vector3 unitCubeCenter)
    {
        if (!(Random.value < asteroidDensity)) return;
        
        var asteroidSize = Random.Range(asteroidSizeRange[0], asteroidSizeRange[1]);
        var placementVariationRange = (unitCubeSize - asteroidSize) / 2f;
        var placementVariation = new Vector3(
            Random.Range(-placementVariationRange, placementVariationRange),
            Random.Range(-placementVariationRange, placementVariationRange),
            Random.Range(-placementVariationRange, placementVariationRange)
        );

        var asteroidPosition = unitCubeCenter + placementVariation;
        var asteroid = Instantiate(asteroidPrefab, asteroidPosition, Quaternion.identity, asteroidsCollection.transform);
        asteroid.transform.localScale *= asteroidSize;
        asteroid.SetActive(false);
        AddEntity(asteroid);
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
        if (cullGroup == null) return;
        cullGroup.Dispose();
        cullGroup = null;
    }
}