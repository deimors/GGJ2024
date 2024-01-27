using System.Linq;
using UnityEngine;
using Zenject;

public class EnemiesInitializer : MonoBehaviour
{
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }

	void Start()
	{
		var spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();

		var enemyPositions = spawnPoints
			.ToDictionary(
				_ => EnemyIdentifier.Create(),
				spawnPoint => spawnPoint.transform.position
			);

		foreach (var spawnPoint in spawnPoints)
			spawnPoint.gameObject.SetActive(false);

		EnemyCommands.Initialize(new EnemiesConfig(enemyPositions));
	}
}