using Assets.Game.Implementation.Domain;
using Functional;
using System.Linq;
using UnityEngine;
using Zenject;

public class TeamInitializer : MonoBehaviour
{
	[Inject] public ITeamCommands TeamCommands { private get; set; }

	void Start()
	{
		var spawnPoints = gameObject.GetComponentsInChildren<TeamMemberSpawnPoint>();

		var teamPositions = spawnPoints.ToDictionary(
			_ => TeamMemberIdentifier.Create(),
			spawnPoint => spawnPoint.transform.position
		);

		var config = new TeamConfig(teamPositions);

		foreach (var spawnPoint in spawnPoints)
			spawnPoint.gameObject.SetActive(false);

		TeamCommands.Initialize(config).DoOnFailure(Debug.LogError);
	}
}