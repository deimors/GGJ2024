using Functional;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerPresenter : MonoBehaviour
{
	private FirstPersonController _controller;

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

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoveEnded>()
			.Subscribe(_ => _controller.playerCanMove = false)
			.AddTo(this);
	}
}
