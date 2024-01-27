using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{
	// TODO - we're not using layer masks currently, is this necessary?
    [SerializeField]
	private LayerMask collisionMask;

    [SerializeField]
	private List<GameObject> enemies;

	private Camera cam;

	// TODO - debug/test - randomly-generated colors for each enemy, to show ray traces
	private List<Color> rayColorsByEnemy;

	// For each enemy inside the camera frustum, test the centre point + 8 points towards the extremities (scaled by 1/2 for now)
	private const float testExtremityScale = 0.5f;
	private static readonly List<Vector3> enemyTestPointOffsetsFromCentre = new()
	{
		new Vector3(0f, 0f, 0f), // centre
		new Vector3(1f, 1f, 1f) * testExtremityScale,
		new Vector3(1f, 1f, -1f) * testExtremityScale,
		new Vector3(1f, -1f, 1f) * testExtremityScale,
		new Vector3(1f, -1f, -1f) * testExtremityScale,
		new Vector3(-1f, 1f, -1f) * testExtremityScale,
		new Vector3(-1f, -1f, 1f) * testExtremityScale,
		new Vector3(-1f, -1f, -1f) * testExtremityScale,
		new Vector3(-1f, 1f, 1f) * testExtremityScale,
	};

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

		rayColorsByEnemy = new List<Color>();
		for (int i = 0; i < enemies.Count; i++)
		{
			rayColorsByEnemy.Add(Random.ColorHSV());
		}
	}

    // Update is called once per frame
    void Update()
    {
		var planes = GeometryUtility.CalculateFrustumPlanes(cam);

		int enemyIndex = 0;
		// Check each enemy's visibility relative to the camera.
		// An enemy is visible if BOTH:
		//    1. Any portion of the enemy's collider is within the camera's visible frustum
		//    2. At least one (?) of the enemy's test points (offsets defined in enemyTestPointOffsetsFromCentre) has a
		//       ray-cast hit from the camera's origin
		foreach (var enemy in enemies)
		{
			// TODO: debug, testing
			var drawColor = rayColorsByEnemy[enemyIndex];
			enemyIndex++;

			var enemyCollider = enemy.GetComponent<Collider>();
			// Ignore enemies that have no portion inside the camera's visible frustum.
			// TODO: We want to check if ANY portion of the bounding box is inside the frustum. Does GeometryUtility.TestPlanesAABB() test
			//       for ANY or ALL?
			//       From https://docs.unity3d.com/ScriptReference/GeometryUtility.TestPlanesAABB.html:
			//           "Will return true if the bounding box is inside the planes or intersects any of the planes."
			//         "intersects" seems to indicate that it does indeed test for ANY portion?
			var isInFrustum = GeometryUtility.TestPlanesAABB(planes, enemyCollider.bounds);

			Debug.LogWarning($"Object {enemy.name} is{(isInFrustum ? "" : " NOT")} in the frustum bounds");

			if (!isInFrustum)
				continue;

			var enemyMesh = enemy.GetComponent<MeshFilter>().sharedMesh;
			var enemyBounds = enemyMesh.bounds;

			var pointsOnEnemy = enemyTestPointOffsetsFromCentre.Select(o =>
				enemy.transform.TransformPoint(enemyBounds.center + Vector3.Scale(enemyBounds.extents, o))
			);

			var numTestPointsVisible = 0;
			foreach (var testPoint in pointsOnEnemy)
			{
				var directionFromCamToEnemyTestPoint = (testPoint - cam.transform.position).normalized;

				// Test point is visible if a raycast from the camera origin to the point hits the enemy
				// TODO - is it correct/optimal to simply check if the enemy's collider is reference-equal to the hitInfo collider?
				var raycastHit = Physics.Raycast(new Ray(cam.transform.position, directionFromCamToEnemyTestPoint), out var hitInfo)
								&& hitInfo.collider == enemyCollider;

				if (raycastHit)
					numTestPointsVisible++;

				// TODO - test/debug - uncomment to show ray trace from camera to test point
				//Debug.DrawRay(cam.transform.position, (testPoint - cam.transform.position), drawColor);
			}

			Debug.LogWarning($"{enemy.name}: {numTestPointsVisible} test points visible.");
		}
	}
}
