using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChunkSpawner : MonoBehaviour
{
    public GameObject[] levelChunksToSpawn; // Array of different LevelChunks to spawn
    public float spawnRate = 2.0f;          // The rate at which LevelChunks are spawned
    public float moveSpeed = 5.0f;          // The speed at which the spawned LevelChunks move along the z-axis
    public Vector3[] preSpawnPositions;     // Array of positions where LevelChunks will be pre-spawned

    private float nextSpawnTime;

    void Start()
    {
        // Pre-spawn LevelChunks at the specified positions
        PreSpawnLevelChunks();
    }

    void Update()
    {
        // Check if it's time to spawn the next LevelChunk
        if (Time.time >= nextSpawnTime)
        {
            SpawnLevelChunk(transform.position);
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void PreSpawnLevelChunks()
    {
        foreach (Vector3 position in preSpawnPositions)
        {
            SpawnLevelChunk(position);
        }
    }

    void SpawnLevelChunk(Vector3 position)
    {
        // Randomly select a LevelChunk from the array
        int randomIndex = Random.Range(0, levelChunksToSpawn.Length);
        GameObject selectedLevelChunk = levelChunksToSpawn[randomIndex];

        // Instantiate the LevelChunk at the given position and spawner's rotation
        GameObject spawnedLevelChunk = Instantiate(selectedLevelChunk, position, transform.rotation);

        // Start moving the spawned LevelChunk
        spawnedLevelChunk.AddComponent<LevelChunkMover>().moveSpeed = moveSpeed;
    }
}

public class LevelChunkMover : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        // Move the LevelChunk along the z-axis
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
