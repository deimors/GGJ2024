using Assets.Game.Implementation.Domain;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class TeamMemberAnimationPresenter : MonoBehaviour
{
	private Animator _animator;
	[SerializeField] private FirstPersonController _controller;

	[Inject] public TeamMemberIdentifier TeamMemberId { private get; set; }

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	void Start()
	{
		_controller.CharacterMoving.DistinctUntilChanged()
			.Subscribe(isMoving =>
			{
				if (isMoving)
				{
					Debug.Log("Set Moving");
					_animator.SetTrigger("Moving");
				}
				else
				{
					Debug.Log("Reset Moving");
					_animator.ResetTrigger("Moving");
				}
			})
			.AddTo(this);

		_controller.CharacterVelocity
			.Subscribe(velocity =>
			{
				_animator.SetFloat("Velocity X", velocity.x);
				_animator.SetFloat("Velocity Z", velocity.z);
			})
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberKilled>()
			.Where(killed => killed.TeamMemberId == TeamMemberId)
			.Subscribe(_ =>
			{
				_animator.SetBool("Trigger", true);
				_animator.SetInteger("TriggerNumber", 27);
			})
			.AddTo(this);
	}
}