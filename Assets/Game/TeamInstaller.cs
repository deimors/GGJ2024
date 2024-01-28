using Assets.Game.Implementation.Domain;
using Assets.Plugins.Zenject.UniRx.Extensions;
using UnityEngine;
using Zenject;

public class TeamInstaller : MonoInstaller
{
	[SerializeField] private GameObject TeamMemberPrefab;
	[SerializeField] private Transform TeamMembersParent;

	[SerializeField] private GameObject PipImagePrefab;
	[SerializeField] private Transform PipImageParent;

	public override void InstallBindings()
	{
		Container.BindModel<TeamAggregate>();

		Container.BindPrefabFactory<TeamMemberParams>(
			TeamMemberPrefab, TeamMembersParent, 
			(container, teamMember) => container.BindInstance(teamMember.TeamMemberId),
			teamMember => teamMember.Position
		);

		Container.BindEvent<TeamEvent, TeamEvent.TeamMemberCreated>()
			.ToFactory(created => new TeamMemberParams(created.TeamMemberId, created.Position));

		Container.Bind<TeamCameras>().ToSelf().AsSingle();
		Container.BindInterfacesAndSelfTo<TeamPositions>().AsSingle().NonLazy();

		Container.BindEvent<EnemyEvent, EnemyEvent.EnemiesTurnEnded>()
			.ToCommand<ITeamCommands, TeamError>((_, team) => team.StartTurn());

		Container.BindPrefabFactory<PipImageParams>(
			PipImagePrefab, PipImageParent,
			(container, pipImage) =>
			{
				container.BindInstance(pipImage.TeamMemberId);
				container.BindInstance(pipImage.Texture);
			},
			_ => Vector3.zero
		);
	}
}