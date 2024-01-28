using UniRx;
using UnityEngine;
using Zenject;
using Unit = Functional.Unit;

public class TeamCamerasPipPresenter : MonoBehaviour
{
	[SerializeField] private GameObject PipPrefab;
	[SerializeField] private GameObject PipCameraPrefab;

	[Inject] public TeamCameras TeamCameras { private get; set; }
	[Inject] public IFactory<PipImageParams, Unit> PipImageFactory { private get; set; }

	void Start()
	{
		Observable.NextFrame()
			.Subscribe(_ => CreatePictureInPictures())
			.AddTo(this);
	}

	private void CreatePictureInPictures()
	{
		foreach (var (teamMemberId, teamCamera) in TeamCameras)
		{
			var renderTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);

			var pip = Instantiate(PipCameraPrefab);
			pip.transform.parent = teamCamera.transform;
			pip.transform.localPosition = Vector3.zero;
			pip.transform.localRotation = Quaternion.identity;
			
			var pipCamera = pip.GetComponent<Camera>();

			pipCamera.targetTexture = renderTexture;

			PipImageFactory.Create(new PipImageParams(teamMemberId, renderTexture));
		}
	}
}