using Assets.Game.Implementation.Domain;
using Functional;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(FirstPersonController), typeof(Collider), typeof(Rigidbody))]
public class TeamMemberPresenter : MonoBehaviour, ITeamMember
{
	private FirstPersonController _controller;
	private Collider _collider;
	private Rigidbody _rigidbody;
	private bool _isKilled;

	[Inject] public TeamMemberIdentifier TeamMemberId { get; set; }

	[Inject] public ITeamCommands TeamCommands { private get; set; }
	[Inject] public ITeamEvents TeamEvents { private get; set; }
	[Inject] public TeamCameras TeamCameras { private get; set; }

	void Awake()
	{
		_controller = GetComponent<FirstPersonController>();
		_collider = GetComponent<Collider>();
		_rigidbody = GetComponent<Rigidbody>();
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
				_controller.cameraCanMove = !_isKilled && isMoving;
			})
			.AddTo(this);

		TeamCameras.Add(TeamMemberId, _controller.playerCamera);

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

		_controller.CharacterMoved
			.Subscribe(moveAmount => TeamCommands.ReduceRemainingMove(TeamMemberId, moveAmount))
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberKilled>()
			.Where(killed => killed.TeamMemberId == TeamMemberId)
			.Subscribe(_ =>
			{
				_isKilled = true;
				_collider.enabled = false;
				_rigidbody.isKinematic = true;
				_controller.cameraCanMove = false;
				TeamCameras.Remove(TeamMemberId);
			})
			.AddTo(this);
	}
}