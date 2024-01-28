using Assets.Game.Implementation.Domain;
using UniRx;
using UnityEngine;
using Zenject;

public class TeamMemberDeadPresenter : MonoBehaviour
{
	private TeamMemberIdentifier _selectedTeamMember;
	
	[SerializeField] private GameObject Panel;

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Start()
	{
		Panel.SetActive(false);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberSelected>()
			.Subscribe(selected =>
			{
				_selectedTeamMember = selected.TeamMemberId;
				Panel.SetActive(selected.isDead);
			})
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberKilled>()
			.Where(killed => killed.TeamMemberId == _selectedTeamMember)
			.Subscribe(_ => Panel.SetActive(true))
			.AddTo(this);
	}
}