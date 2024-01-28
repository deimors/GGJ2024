using UniRx;
using UnityEngine;
using Zenject;

public class TeamMemberDeadPresenter : MonoBehaviour
{
	[SerializeField] private GameObject Panel;

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Start()
	{
		Panel.SetActive(false);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberSelected>()
			.Subscribe(selected => Panel.SetActive(selected.isDead))
			.AddTo(this);
	}
}