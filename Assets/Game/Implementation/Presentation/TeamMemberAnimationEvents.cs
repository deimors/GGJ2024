using UnityEngine;
using UnityEngine.Events;

public class TeamMemberAnimationEvents : MonoBehaviour
{
	public UnityEvent OnHit = new UnityEvent();
	public UnityEvent OnShoot = new UnityEvent();
	public UnityEvent OnFootR = new UnityEvent();
	public UnityEvent OnFootL = new UnityEvent();
	public UnityEvent OnLand = new UnityEvent();
	public UnityEvent OnWeaponSwitch = new UnityEvent();

	public AnimatorMoveEvent OnMove = new AnimatorMoveEvent();
	
	public void Hit() => OnHit.Invoke();
	public void Shoot() => OnShoot.Invoke();
	public void FootR() => OnFootR.Invoke();
	public void FootL() => OnFootL.Invoke();
	public void Land() => OnLand.Invoke();

	public void WeaponSwitch() => OnWeaponSwitch.Invoke();

	// Used for animations that contain root motion to drive the character�s
	// position and rotation using the �Motion� node of the animation file.
	void OnAnimatorMove()
	{
		/*
		if (!animator) { return; }

		// Not used when using Navmesh Navigation.
		if (rpgCharacterController.isNavigating) { return; }

		OnMove.Invoke(animator.deltaPosition, animator.rootRotation);
		*/
	}
}
