using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ProceduralGeneration : MonoBehaviour
{
    public int levelSize = 1024;
    public int chunkSize = 256;
    public int unitCubeSize = 64;

    public GameObject chunkPrefab;
    public GameObject blackHolePrefab;
    public GameObject exitPortalPrefab;
    public GameObject asteroidPrefab;

    private Dictionary<int, Vector3Int> chunkLookup;
    private GameObject[,,] levelChunks;
    private int numChunksInLevel;
    private int numUnitCubesInChunk;
    
    private int occupiedChunkId;
    
    private void Start()
    {
        var asteroidField = GameObject.FindGameObjectWithTag("AsteroidField");
        numChunksInLevel = levelSize / chunkSize;
        numUnitCubesInChunk = unitCubeSize / chunkSize;
        chunkLookup = new Dictionary<int, Vector3Int>();
        levelChunks = new GameObject[numChunksInLevel, numChunksInLevel, numChunksInLevel];
        GenerateAsteroidField(asteroidField);
    }

    private void GenerateAsteroidField(GameObject asteroidField)
    {
        for (var x = 0; x < numChunksInLevel; x++)
        {
            for (var y = 0; y < numChunksInLevel; y++)
            {
                for (var z = 0; z < numChunksInLevel; z++)
                {
                    var chunkIndex = new Vector3Int(x, y, z);
                    var chunk = GenerateChunk(asteroidField, chunkIndex);
                    levelChunks[x, y, z] = chunk;
                    chunkLookup[chunk.GetInstanceID()] = chunkIndex;
                }
            }
        }
    }

    private GameObject GenerateChunk(GameObject asteroidField, Vector3 indexInAsteroidField)
    {
        var chunk = Instantiate(chunkPrefab, indexInAsteroidField * chunkSize, Quaternion.identity, asteroidField.transform);
        for (var x = 0; x < numUnitCubesInChunk; x++)
        {
            for (var y = 0; y < numUnitCubesInChunk; y++)
            {
                for (var z = 0; z < numUnitCubesInChunk; z++)
                {
                    GenerateUnitCube(chunk, new Vector3(x, y, z));
                }
            }
        }

        return chunk;
    }

    private void GenerateUnitCube(GameObject chunk, Vector3 indexInChunk)
    {
        var unitCube = Instantiate(new GameObject(), indexInChunk * unitCubeSize, Quaternion.identity, chunk.transform);
        Instantiate(asteroidPrefab, unitCube.transform);
    }

    public void SetOccupiedChunk(GameObject occupiedChunk)
    {
        var newOccupiedChunkId = occupiedChunk.GetInstanceID();

        var oldActiveChunkIndices = AdjacentChunkIndices(chunkLookup[occupiedChunkId]);
        var newActiveChunkIndices = AdjacentChunkIndices(chunkLookup[newOccupiedChunkId]);

        var chunkIndicesToDisable = oldActiveChunkIndices.FindAll(v => !newActiveChunkIndices.Contains(v));
        foreach (var chunkIndex in chunkIndicesToDisable)
        {
            levelChunks[chunkIndex.x, chunkIndex.y, chunkIndex.z].SetActive(false);
        }
        
        var chunkIndicesToEnable = newActiveChunkIndices.FindAll(v => !oldActiveChunkIndices.Contains(v));
        foreach (var chunkIndex in chunkIndicesToEnable)
        {
            levelChunks[chunkIndex.x, chunkIndex.y, chunkIndex.z].SetActive(true);
        }

        occupiedChunkId = newOccupiedChunkId;
    }

    private List<Vector3Int> AdjacentChunkIndices(Vector3Int startIndex)
    {
        var adjacentChunkIndices = new List<Vector3Int>();
        for (var xDelta = -1; xDelta < 2; xDelta++)
        {
            for (var yDelta = -1; yDelta < 2; yDelta++)
            {
                for (var zDelta = -1; zDelta < 2; zDelta++)
                {
                    var index = startIndex + new Vector3Int(xDelta, yDelta, zDelta);
                    if (!adjacentChunkIndices.Contains(index) && PointLiesInRegion(index, Vector3Int.one * numChunksInLevel))
                    {
                        adjacentChunkIndices.Add(index);
                    }
                }
            }
        }

        return adjacentChunkIndices;
    }

    private bool PointLiesInRegion(Vector3Int point, Vector3Int region)
    {
        return point.x >= 0f && point.y >= 0f && point.z >= 0f &&
               point.x < region.x && point.y < region.y && point.z < region.z;
    }
}