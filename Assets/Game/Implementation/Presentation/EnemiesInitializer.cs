using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemiesInitializer : MonoBehaviour
{
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }
	public int NumberofEnemies;

	void Start()
	{
		var spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
		// var spawnPoints = GetRandomSpawnPoints();

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
		var spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
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

		foreach (var spawnIndex in randomSpawnIndexList)
		{
			yield return spawnPoints[spawnIndex];
		}
	}
}