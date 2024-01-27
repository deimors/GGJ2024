using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Pathfinding;
using Zenject;

[RequireComponent(typeof(EnemyPathfinder))]
public class EnemyTurnPathfinder : MonoBehaviour
{
	private EnemyPathfinder _pathfinder;

	[Inject] public EnemyIdentifier EnemyId { private get; set; }

	[Inject] public IEnemyEvents EnemyEvents { private get; set; }

	void Awake()
	{
		_pathfinder = GetComponent<EnemyPathfinder>();
	}

    // Start is called before the first frame update
    void Start()
    {
		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyTurnStarted>()
			.Where(started => started.EnemyId == EnemyId)
			.Subscribe(_ => StartTurn())
			.AddTo(this);
    }

	private void StartTurn()
	{
		_pathfinder.StartPathCalculation(OnPathComplete);
	}

	private void OnPathComplete(Path p)
	{
		Debug.Log($"Path found. Total length: {p.GetTotalLength()}");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
