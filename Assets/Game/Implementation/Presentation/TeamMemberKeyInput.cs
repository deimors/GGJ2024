using System.Linq;
using Assets.Game.Implementation.Domain;
using UnityEngine;
using Zenject;

public class TeamMemberKeyInput : MonoBehaviour
{
	[SerializeField] private KeyCode[] TeamMemberKeys;
	[SerializeField] private KeyCode EndTurnKey = KeyCode.Space;

	[Inject] public ITeamCommands TeamCommands { private get; set; }

	void Update()
	{
		foreach (var (keyCode, teamMemberId) in TeamMemberKeys.Select((keyCode, teamMemberId) => (keyCode, teamMemberId)))
		{
			if (Input.GetKeyDown(keyCode))
				TeamCommands.SelectTeamMember(new TeamMemberIdentifier(teamMemberId));
		}

		if (Input.GetKeyDown(EndTurnKey))
			TeamCommands.EndTurn();
	}

	void FixedUpdate()
	{
		var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (targetVelocity.magnitude > Mathf.Epsilon)
		{
			TeamCommands.MoveTeamMember(targetVelocity, Time.fixedDeltaTime);
		}
	}
}