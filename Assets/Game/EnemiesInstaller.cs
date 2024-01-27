using Assets.Game.Implementation.Domain;
using Assets.Game.Implementation.Presentation;
using Assets.Plugins.Zenject.UniRx.Extensions;
using UnityEngine;
using Zenject;

public class EnemiesInstaller : MonoInstaller
{
	[SerializeField] private GameObject EnemyPrefab;
	[SerializeField] private Transform EnemiesParent;

	public override void InstallBindings()
	{
		Container.BindModel<EnemiesAggregate>();

		Container.BindPrefabFactory<EnemyParams>(
			EnemyPrefab, EnemiesParent,
			(container, enemy) => container.BindInstance(enemy.EnemyId),
			enemy => enemy.Position
		);

		Container.BindEvent<EnemyEvent, EnemyEvent.EnemyCreated>()
			.ToFactory(created => new EnemyParams(created.EnemyId, created.Position));

		Container.BindEvent<TeamEvent, TeamEvent.TeamTurnEnded>()
			.ToCommand<IEnemyCommands, EnemyError>((_, enemies) => enemies.StartTurn());
	}
}