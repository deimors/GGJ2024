using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TeamCamerasPipPresenter : MonoBehaviour
{
	[SerializeField] private GameObject PipPrefab;
	[SerializeField] private GameObject PipCameraPrefab;

	[Inject] public TeamCameras TeamCameras { private get; set; }

	void Start()
	{
		Observable.NextFrame()
			.Subscribe(_ => CreatePictureInPictures())
			.AddTo(this);
	}

	private void CreatePictureInPictures()
	{
		foreach (var teamCamera in TeamCameras)
		{
			var renderTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);

			var pip = Instantiate(PipCameraPrefab);
			pip.transform.parent = teamCamera.transform;
			pip.transform.localPosition = Vector3.zero;
			pip.transform.localRotation = Quaternion.identity;
			
			var pipCamera = pip.GetComponent<Camera>();

			pipCamera.targetTexture = renderTexture;

			var pipPanel = Instantiate(PipPrefab);

			pipPanel.transform.SetParent(transform);

			pipPanel.GetComponent<RawImage>().texture = renderTexture;
		}
	}
}