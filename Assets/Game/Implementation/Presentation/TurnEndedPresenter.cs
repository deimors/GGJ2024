using UniRx;
using UnityEngine;
using Zenject;

public class TurnEndedPresenter : MonoBehaviour
{
	[SerializeField] private GameObject TurnEndedPanel;

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Start()
	{
		TurnEndedPanel.SetActive(false);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamTurnEnded>()
			.Subscribe(_ => TurnEndedPanel.SetActive(true))
			.AddTo(this);
	}
}