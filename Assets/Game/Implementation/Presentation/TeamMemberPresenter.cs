using Assets.Game.Implementation.Domain;
using Functional;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(FirstPersonController))]
public class TeamMemberPresenter : MonoBehaviour, ITeamMember
{
	private FirstPersonController _controller;

	[Inject] public TeamMemberIdentifier TeamMemberId { get; set; }

	[Inject] public ITeamCommands TeamCommands { private get; set; }
	[Inject] public ITeamEvents TeamEvents { private get; set; }
	[Inject] public TeamCameras TeamCameras { private get; set; }

	void Awake()
	{
		_controller = GetComponent<FirstPersonController>();
	}

	void Start()
	{
		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoved>()
			.Where(moved => moved.TeamMemberId == TeamMemberId)
			.Subscribe(moved => _controller.MovePlayer(moved.TargetVelocity))
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberSelected>()
			.Select(selected => selected.TeamMemberId == TeamMemberId)
			.Subscribe(isMoving =>
			{
				_controller.playerCamera.enabled = isMoving;
				_controller.cameraCanMove = isMoving;
			})
			.AddTo(this);

		TeamCameras.Add(_controller.playerCamera);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamExited>()
			.Subscribe(_ =>
			{
				_controller.playerCanMove = false;
				_controller.cameraCanMove = false;
			})
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoveEnded>()
			.Where(ended => ended.TeamMemberId == TeamMemberId)
			.Subscribe(_ =>
			{
				_controller.StopMoving();
				TeamCommands.DeclareTeamMemberPosition(TeamMemberId, transform.position);
			})
			.AddTo(this);
	}
}