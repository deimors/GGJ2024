using UniRx;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TeamMemberAnimationPresenter : MonoBehaviour
{
	private Animator _animator;
	[SerializeField] private FirstPersonController _controller;

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
	}
}