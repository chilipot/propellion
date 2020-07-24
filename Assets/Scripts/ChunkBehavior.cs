using System;
using UnityEngine;

public class ChunkBehavior : MonoBehaviour
{
    public ProceduralGeneration chunkManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chunkManager.SetOccupiedChunk(gameObject);
        }
    }
}