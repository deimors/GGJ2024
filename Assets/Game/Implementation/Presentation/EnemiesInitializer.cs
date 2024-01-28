using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemiesInitializer : MonoBehaviour
{
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }
	public int NumberofEnemies;

	void Start()
	{
		// var spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
		var spawnPoints = GetRandomSpawnPoints();
		
		// deactivate all unused spawns
		

		var enemyPositions = spawnPoints
			.ToDictionary(
				_ => EnemyIdentifier.Create(),
				spawnPoint => spawnPoint.transform.position
			);

		foreach (var spawnPoint in spawnPoints)
			spawnPoint.gameObject.SetActive(false);

		EnemyCommands.Initialize(new EnemiesConfig(enemyPositions));
	}

	private IEnumerable<EnemySpawnPoint> GetRandomSpawnPoints()
	{
		var spawnPoints = GetComponentsInChildren<EnemySpawnPoint>().Where(e => e.gameObject.activeSelf).ToArray();
		var spawnPointsUpperBound = spawnPoints.Length - 1;

		var randomSpawnIndexList = new List<int>();
		for (var i = 0; i < NumberofEnemies; i++)
		{
			var newSpawnPointFound = false;
			while (!newSpawnPointFound)
			{
				var possibleSpawnPoint = Random.Range(0, spawnPointsUpperBound);
				if (randomSpawnIndexList.Contains(possibleSpawnPoint)) continue;
				
				randomSpawnIndexList.Add(possibleSpawnPoint);
				newSpawnPointFound = true;
			}
		}
		
		// Deactivate unused spawns
		for (var i = 0; i <= spawnPointsUpperBound; i++)
		{
			if (randomSpawnIndexList.Contains(i)) continue;
				spawnPoints[i].gameObject.SetActive(false);
			// Destroy(spawnPoints[i]);
		}

		return randomSpawnIndexList.Select(spawnIndex => spawnPoints[spawnIndex]).ToList();
	}
}