using Fusion;
using UnityEngine;

public class PrefabSpawner : NetworkBehaviour
{
    [Networked] private TickTimer spawnTimer { get; set; }

    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] NetworkObject prefabToSpawn;
    [SerializeField] Vector3[] SpawnPositions;

    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (spawnTimer.ExpiredOrNotRunning(Runner))
        {
            Vector3 spawnPos;
            if (SpawnPositions == null || SpawnPositions.Length == 0)
            {
                spawnPos = transform.position;
            }
            else
            {
                spawnPos = SpawnPositions[Random.Range(0, SpawnPositions.Length)];
            }
            Runner.Spawn(prefabToSpawn, spawnPos, transform.rotation, Object.InputAuthority);
            spawnTimer = TickTimer.CreateFromSeconds(Runner, timeBetweenSpawns);
        }
    }
}