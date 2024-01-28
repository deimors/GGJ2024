using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;
using Functional;

[RequireComponent(typeof(EnemyVisibilityDetector))]
public class EnemyPathfinder : MonoBehaviour
{
	// https://arongranberg.com/astar/docs/custom_movement_script.html

	[SerializeField] private Transform _model;

	[Inject] public EnemyIdentifier EnemyId { private get; set; }

	[Inject] public TeamPositions TeamPositions { private get; set; }
	[Inject] public IEnemyEvents EnemyEvents { private get; set; }
	[Inject] public IEnemyCommands EnemyCommands { private get; set; }
	[Inject] public ITeamCommands TeamCommands { private get; set; }

	private const float MovePerTurnDistance = 7f;

	private Path _currentShortestPath;

	private readonly Queue<Vector3> _playerPositionsToCheck = new();

	private readonly Queue<Vector3> _pathToFollow = new();
	private EnemyVisibilityDetector _visibilityDetector;
	private Vector3 _lastNode;
	private float _accumulatedDistance;
	private readonly Collider[] _colliders = new Collider[10];
	private bool _activated;

	void Awake()
	{
		_visibilityDetector = GetComponent<EnemyVisibilityDetector>();
	}

	void Start()
	{
		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyTurnStarted>()
			.Where(started => started.EnemyId == EnemyId)
			.Subscribe(_ =>
			{
				if (_activated)
					StartPathCalculation();
				else
					EndTurn();
			})
			.AddTo(this);

		EnemyEvents.OfType<EnemyEvent, EnemyEvent.EnemyActivated>()
			.Where(activated => activated.EnemyId == EnemyId)
			.Do(Debug.Log)
			.Subscribe(_ => _activated = true)
			.AddTo(this);
	}

	// NOTE - must be done in FixedUpdate() rather than Update() because raycasting (EnemyVisibilityDetector.CanBeSeenByTeamCamera())
	// depends on fixed update positions.
	// https://forum.unity.com/threads/help-understanding-raycasting-issues.1239106/  (last post)
	void FixedUpdate()
	{
		if (!_pathToFollow.TryDequeue(out var nextNode))
			return;

		var lookPos = _pathToFollow.Any() ? _pathToFollow.Last() : nextNode;
		lookPos.y = transform.position.y;
		var lookDir = (lookPos - transform.position).normalized;

		_model.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

		nextNode.y = transform.position.y;
		_accumulatedDistance += (nextNode - transform.position).magnitude;

		var projectedCollisions = Physics.OverlapSphereNonAlloc(transform.position, 1, _colliders);

		if (projectedCollisions > 0)
		{
			var teamCollisions = _colliders.Take(projectedCollisions).Select(collider => collider.GetComponent<ITeamMember>())
				.Where(teamMember => teamMember != null);

			var teamMemberId = teamCollisions.FirstOrDefault()?.TeamMemberId;

			if (teamMemberId != null)
			{
				Debug.Log($"Projected collision with team member {teamMemberId}");
				EndTurn();

				TeamCommands.KillTeamMember(teamMemberId);

				return;
			}
			
		}

		transform.position = nextNode;

		var isSeen = _visibilityDetector.CanBeSeenByTeamCamera();
		var moveExhausted = _accumulatedDistance >= MovePerTurnDistance;

		if (isSeen || moveExhausted)
		{
			if (isSeen)
			{
				Debug.Log($"{EnemyId} -> Seen!");
				transform.position = _lastNode;
			}
			if (moveExhausted) Debug.Log($"{EnemyId} -> Move exhausted");

			EndTurn();
		}

		_lastNode = nextNode;
	}

	private void EndTurn()
	{
		_pathToFollow.Clear();
		_accumulatedDistance = 0;

		EnemyCommands.EndEnemyTurn(EnemyId);
	}
	
	private void StartPathCalculation()
    {
	    if (_visibilityDetector.CanBeSeenByTeamCamera())
	    {
			Debug.Log("Already Seen!");
			EnemyCommands.EndEnemyTurn(EnemyId);

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
