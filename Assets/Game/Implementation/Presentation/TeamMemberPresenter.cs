using Functional;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(FirstPersonController))]
public class TeamMemberPresenter : MonoBehaviour
{
	private FirstPersonController _controller;

	[SerializeField] private int TeamMemberId;

	[Inject] public ITeamCommands TeamCommands { private get; set; }
	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Awake()
	{
		_controller = GetComponent<FirstPersonController>();
	}

	void Start()
	{
		_controller.CharacterMoved
			.Subscribe(amount => TeamCommands.MoveTeamMember(amount).DoOnFailure(Debug.LogError))
			.AddTo(this);

		var hasRemainingMove = TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoveEnded>()
			.Where(moveEnded => moveEnded.TeamMemberId == TeamMemberId)
			.Select(_ => false)
			.Merge(Observable.Return(true));

		var isMovingMember = TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberSelected>()
			.Select(selected => selected.TeamMemberId == TeamMemberId);

		isMovingMember
			.CombineLatest(hasRemainingMove, (isMoving, hasRemaining) => hasRemaining && isMoving)
			.Subscribe(canMove => _controller.playerCanMove = canMove)
			.AddTo(this);
	}
}