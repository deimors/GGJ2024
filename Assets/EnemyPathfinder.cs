using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using Zenject;

public class EnemyPathfinder : MonoBehaviour
{
    // https://arongranberg.com/astar/docs/custom_movement_script.html

	[Inject] public TeamCameras TeamCameras { private get; set; }

	private int currentPlayerIndex;
	private Path currentShortestPath;
	private OnPathDelegate shortestPathCallback;

    public void StartPathCalculation(OnPathDelegate callback = null)
    {
		if (AstarPath.active == null)
		{
            Debug.LogError("AstarPath has not been initialized!");
            return;
		}

		currentPlayerIndex = 0;
		currentShortestPath = null;
		shortestPathCallback = callback;

        // TODO - launch multiple paths in parallel, one for each player camera, and take the result which is closest to our origin.
        // https://arongranberg.com/astar/documentation/dev_4_1_5_3cb9f189/calling-pathfinding.php - " Calling AstarPath directly"
        // (Note - free version is apparently limited to 1 thread anyway)
		StartFindNextPlayerPath();
		//Debug.LogWarning($"Enemy position: {transform.position}; Player position: {player.transform.position}");
	}

	private void StartFindNextPlayerPath()
	{
		var player = TeamCameras.Skip(currentPlayerIndex++).FirstOrDefault();
		if (player == null)
		{
			var curShortestPathCallback = shortestPathCallback;
			shortestPathCallback = null;
			curShortestPathCallback?.Invoke(currentShortestPath);
			return;
		}

		var pathToPlayer = ABPath.Construct(transform.position, player.transform.position, OnSinglePathComplete);
		AstarPath.StartPath(pathToPlayer);
	}

	private void OnSinglePathComplete(Path p)
	{
		Debug.LogWarning($"currentShortestPath={currentShortestPath?.GetTotalLength()}; p={p?.GetTotalLength()}");

		if (currentShortestPath == null)
			currentShortestPath = p;
		else if (p != null && p.GetTotalLength() < currentShortestPath.GetTotalLength())
			currentShortestPath = p;

		StartFindNextPlayerPath();
	}
}
