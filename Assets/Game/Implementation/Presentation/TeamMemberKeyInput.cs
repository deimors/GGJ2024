using System.Linq;
using UnityEngine;
using Zenject;

public class TeamMemberKeyInput : MonoBehaviour
{
	[SerializeField] private KeyCode[] TeamMemberKeys;

	[Inject] public ITeamCommands TeamCommands { private get; set; }

	void Update()
	{
		foreach (var (keyCode, teamMemberId) in TeamMemberKeys.Select((keyCode, teamMemberId) => (keyCode, teamMemberId)))
		{
			if (Input.GetKeyDown(keyCode))
				TeamCommands.SelectTeamMember(teamMemberId);
		}
	}
}