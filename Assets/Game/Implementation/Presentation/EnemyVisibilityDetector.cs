using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyVisibilityDetector : MonoBehaviour
{
	[Inject] public TeamCameras TeamCameras { private get; set; }

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
		var nearPlaneMidLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, cam.nearClipPlane));
		var nearPlaneMidRight = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, cam.nearClipPlane));
		var nearPlaneMidCentre = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));

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
		// TODO - necessary???
		var isInFrustum = GeometryUtility.TestPlanesAABB(camFrustrum, _enemyCollider.bounds);

		if (!isInFrustum)
			return false;

		var enemyMesh = _meshFilter.sharedMesh;
		enemyMesh.RecalculateBounds(); // TODO - necessary???

		// Algorithm based on: https://forum.unity.com/threads/check-if-a-collider-is-visible-from-a-position.424760/
		foreach (var vertex in enemyMesh.vertices)
		{
			var vertexInWorldCoords = transform.TransformPoint(vertex);
			var viewportCoords = cam.WorldToViewportPoint(vertexInWorldCoords);
			if (!(viewportCoords.z > 0f
				&& viewportCoords.x >= 0f && viewportCoords.x <= 1f
				&& viewportCoords.y >= 0f && viewportCoords.y <= 1f))
			{
				continue;
			}

			// Test point is visible if a raycast from the camera's near plane far-left, far-right or centre (mid-height)
			// to the test point hits the enemy
			var ray = new Ray(nearPlaneMidLeft, (vertexInWorldCoords - nearPlaneMidLeft).normalized);
			var raycastHit = Physics.Raycast(ray, out var hitInfo) && hitInfo.transform.gameObject == gameObject;
			if (raycastHit)
				return true;

			ray = new Ray(nearPlaneMidRight, (vertexInWorldCoords - nearPlaneMidRight).normalized);
			raycastHit = Physics.Raycast(ray, out hitInfo) && hitInfo.transform.gameObject == gameObject;
			if (raycastHit)
				return true;

			ray = new Ray(nearPlaneMidCentre, (vertexInWorldCoords - nearPlaneMidCentre).normalized);
			raycastHit = Physics.Raycast(ray, out hitInfo) && hitInfo.transform.gameObject == gameObject;
			if (raycastHit)
				return true;
		}

		return false;
	}
}