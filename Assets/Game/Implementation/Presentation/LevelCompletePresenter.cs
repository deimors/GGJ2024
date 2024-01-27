using UniRx;
using UnityEngine;
using Zenject;

public class LevelCompletePresenter : MonoBehaviour
{
	[SerializeField] private GameObject LevelCompletePanel;

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Start()
	{
		LevelCompletePanel.SetActive(false);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamExited>()
			.Subscribe(_ => LevelCompletePanel.SetActive(true))
			.AddTo(this);
	}
}