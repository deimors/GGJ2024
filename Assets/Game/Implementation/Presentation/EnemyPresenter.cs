using UniRx;
using UnityEngine;
using Zenject;

public class EnemyPresenter : MonoBehaviour
{
	[Inject] public EnemyIdentifier EnemyId { private get; set; }

	[Inject] public IEnemyEvents EnemyEvents { private get; set; }

	void Start()
	{
		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyTurnStarted>()
			.Where(started => started.EnemyId == EnemyId)
			.Subscribe(_ => StartTurn())
			.AddTo(this);
	}

	private void StartTurn()
	{
		Debug.Log($"Enemy {EnemyId} turn started");
	}
}