using Assets.Game.Implementation.Domain;
using Assets.Plugins.Zenject.UniRx.Extensions;
using UnityEngine;
using Zenject;

public class TeamInstaller : MonoInstaller
{
	[SerializeField] private GameObject TeamMemberPrefab;
	[SerializeField] private Transform TeamMembersParent;

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
	}
}