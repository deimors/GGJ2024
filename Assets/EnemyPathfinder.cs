using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyVisibilityDetector))]
public class EnemyPathfinder : MonoBehaviour
{
	// https://arongranberg.com/astar/docs/custom_movement_script.html

	[Inject] public EnemyIdentifier EnemyId { private get; set; }

	[Inject] public TeamPositions TeamPositions { private get; set; }
	[Inject] public IEnemyEvents EnemyEvents { private get; set; }
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }

	private Path _currentShortestPath;

	private readonly Queue<Vector3> _playerPositionsToCheck = new();

	private readonly Queue<Vector3> _pathToFollow = new();
	private EnemyVisibilityDetector _visibilityDetector;
	private Vector3 _lastNode;

	void Awake()
	{
		_visibilityDetector = GetComponent<EnemyVisibilityDetector>();
	}

	void Start()
	{
		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyTurnStarted>()
			.Where(started => started.EnemyId == EnemyId)
			.Subscribe(_ => StartPathCalculation())
			.AddTo(this);
	}

	void Update()
	{
		if (!_pathToFollow.TryDequeue(out var nextNode))
			return;

		nextNode.y = transform.position.y;
		transform.position = nextNode;

		if (_visibilityDetector.CanBeSeenByTeamCamera())
		{
			Debug.Log("Seen!");
			transform.position = _lastNode;
			_pathToFollow.Clear();

			EnemyCommands.EndEnemyTurn(EnemyId);
		}

		_lastNode = nextNode;
	}

	public void StartPathCalculation()
    {
	    if (_visibilityDetector.CanBeSeenByTeamCamera())
	    {
			Debug.Log("Already Seen!");
			return;
	    }

		if (AstarPath.active == null)
		{
            Debug.LogError("AstarPath has not been initialized!");
            return;
		}

		_playerPositionsToCheck.Clear();

		foreach (var position in TeamPositions.Values) 
			_playerPositionsToCheck.Enqueue(position);

		_currentShortestPath = null;

		// TODO - launch multiple paths in parallel, one for each player camera, and take the result which is closest to our origin.
        // https://arongranberg.com/astar/documentation/dev_4_1_5_3cb9f189/calling-pathfinding.php - " Calling AstarPath directly"
        // (Note - free version is apparently limited to 1 thread anyway)
		StartFindNextPlayerPath();
		//Debug.LogWarning($"Enemy position: {transform.position}; Player position: {player.transform.position}");
	}

	private void StartFindNextPlayerPath()
	{
		if (!_playerPositionsToCheck.Any())
		{
			Debug.Log($"Shortest path: {_currentShortestPath}");

			//_followingPath = _currentShortestPath;

			foreach (var node in _currentShortestPath.vectorPath)
				_pathToFollow.Enqueue(node);
		}
		else
		{
			var position = _playerPositionsToCheck.Dequeue();

			var pathToPlayer = ABPath.Construct(transform.position, position, OnSinglePathComplete);

			AstarPath.StartPath(pathToPlayer);
		}
	}

	private void OnSinglePathComplete(Path path)
	{
		if (_currentShortestPath == null)
			_currentShortestPath = path;
		else if (path != null && path.GetTotalLength() < _currentShortestPath.GetTotalLength())
			_currentShortestPath = path;

		StartFindNextPlayerPath();
	}
}
