using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Slider))]
public class RemainingMovementPresenter : MonoBehaviour
{
	private Slider _slider;

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Awake()
	{
		_slider = GetComponent<Slider>();
	}

	void Start()
	{
		_slider.value = 1f;

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoved>()
			.Subscribe(moved => _slider.value = moved.RemainingMovePercent)
			.AddTo(this);
	}
}