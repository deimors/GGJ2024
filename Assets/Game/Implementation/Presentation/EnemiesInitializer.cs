using System.Linq;
using UnityEngine;
using Zenject;

public class EnemiesInitializer : MonoBehaviour
{
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }

	void Start()
	{
		var enemyPositions = GetComponentsInChildren<EnemySpawnPoint>()
			.ToDictionary(
				_ => EnemyIdentifier.Create(),
				spawnPoint => spawnPoint.transform.position
			);

		EnemyCommands.Initialize(new EnemiesConfig(enemyPositions));
	}
}