using Assets.Game.Implementation.Domain;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PipImagePresenter : MonoBehaviour
{
	private RawImage _rawImage;
	[SerializeField] private Slider _slider;
	[SerializeField] private Image _borderImage;

	[Inject] public TeamMemberIdentifier TeamMemberId { private get; set; }
	[Inject] public RenderTexture Texture { private get; set; }

	[Inject] public ITeamEvents TeamEvents { private get; set; }

	void Awake()
	{
		_rawImage = GetComponent<RawImage>();
	}

	void Start()
	{
		_rawImage.texture = Texture;

		_slider.value = 1;

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberMoved>()
			.Where(moved => moved.TeamMemberId == TeamMemberId)
			.Subscribe(moved => _slider.value = moved.RemainingMovePercent)
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamTurnStarted>()
			.Subscribe(_ => _slider.value = 1)
			.AddTo(this);

		TeamEvents.OfType<TeamEvent, TeamEvent.TeamMemberSelected>()
			.Subscribe(selected => _borderImage.enabled = selected.TeamMemberId == TeamMemberId)
			.AddTo(this);
	}
}