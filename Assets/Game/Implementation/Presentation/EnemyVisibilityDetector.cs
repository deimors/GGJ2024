using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyVisibilityDetector : MonoBehaviour
{
	[Inject] public TeamCameras TeamCameras { private get; set; }

	private const float TestExtremityScale = 0.5f;
	private static readonly List<Vector3> EnemyTestPointOffsetsFromCentre = new()
	{
		new Vector3(0f, 0f, 0f), // centre
		new Vector3(1f, 1f, 1f) * TestExtremityScale,
		new Vector3(1f, 1f, -1f) * TestExtremityScale,
		new Vector3(1f, -1f, 1f) * TestExtremityScale,
		new Vector3(1f, -1f, -1f) * TestExtremityScale,
		new Vector3(-1f, 1f, -1f) * TestExtremityScale,
		new Vector3(-1f, -1f, 1f) * TestExtremityScale,
		new Vector3(-1f, -1f, -1f) * TestExtremityScale,
		new Vector3(-1f, 1f, 1f) * TestExtremityScale,
	};

	private Collider _enemyCollider;
	private MeshFilter _meshFilter;

	void Awake()
	{
		_enemyCollider = GetComponent<Collider>();
		_meshFilter = GetComponent<MeshFilter>();
	}

	public bool CanBeSeenByTeamCamera() 
		=> TeamCameras.Values.Any(CanBeSeenByCamera);

	private bool CanBeSeenByCamera(Camera cam)
	{
		var camFrustrum = GeometryUtility.CalculateFrustumPlanes(cam);
		var camPosition = cam.transform.position;

		// Check each enemy's visibility relative to the camera.
		// An enemy is visible if BOTH:
		//    1. Any portion of the enemy's collider is within the camera's visible frustum
		//    2. At least one (?) of the enemy's test points (offsets defined in enemyTestPointOffsetsFromCentre) has a
		//       ray-cast hit from the camera's origin


		// Ignore enemies that have no portion inside the camera's visible frustum.
		// TODO: We want to check if ANY portion of the bounding box is inside the frustum. Does GeometryUtility.TestPlanesAABB() test
		//       for ANY or ALL?
		//       From https://docs.unity3d.com/ScriptReference/GeometryUtility.TestPlanesAABB.html:
		//           "Will return true if the bounding box is inside the planes or intersects any of the planes."
		//         "intersects" seems to indicate that it does indeed test for ANY portion?
		var isInFrustum = GeometryUtility.TestPlanesAABB(camFrustrum, _enemyCollider.bounds);

		if (!isInFrustum)
			return false;

		var enemyMesh = _meshFilter.sharedMesh;
		var enemyBounds = enemyMesh.bounds;

		var pointsOnEnemy = EnemyTestPointOffsetsFromCentre.Select(o =>
			transform.TransformPoint(enemyBounds.center + Vector3.Scale(enemyBounds.extents, o))
		);

		var numTestPointsVisible = 0;
		foreach (var testPoint in pointsOnEnemy)
		{
			var directionFromCamToEnemyTestPoint = (testPoint - camPosition).normalized;

			// Test point is visible if a raycast from the camera origin to the point hits the enemy
			// TODO - is it correct/optimal to simply check if the enemy's collider is reference-equal to the hitInfo collider?
			var ray = new Ray(camPosition, directionFromCamToEnemyTestPoint);
			var raycastHit = Physics.Raycast(ray, out var hitInfo) && hitInfo.collider == _enemyCollider;

			if (raycastHit)
				numTestPointsVisible++;
		}

		return numTestPointsVisible > 0;
	}
}