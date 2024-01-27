using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyVisibilityDetector))]
public class EnemyPresenter : MonoBehaviour
{
	private EnemyVisibilityDetector _visibilityDetector;

	[Inject] public EnemyIdentifier EnemyId { private get; set; }

	[Inject] public IEnemyEvents EnemyEvents { private get; set; }

	void Awake()
	{
		_visibilityDetector = GetComponent<EnemyVisibilityDetector>();
	}

	void Start()
	{
		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyTurnStarted>()
			.Where(started => started.EnemyId == EnemyId)
			.Subscribe(_ => StartTurn())
			.AddTo(this);
	}

	private void StartTurn()
	{
		var isVisible = _visibilityDetector.CanBeSeenByTeamCamera();

		Debug.Log($"{EnemyId} turn started; is visible = {isVisible}");
	}
}