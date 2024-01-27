using UnityEngine;
using Zenject;

public class ExitPresenter : MonoBehaviour
{
	private MeshRenderer _meshRenderer;

	[Inject] public ITeamCommands TeamCommands { private get; set; }

	void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	void Start()
	{
		if (_meshRenderer != null)
			_meshRenderer.enabled = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		var teamMemberId = collider.GetComponent<ITeamMember>()?.TeamMemberId;
		
		if (teamMemberId != null)
		{
			Debug.Log($"Exit triggered by {teamMemberId}");
			TeamCommands.ExitTeamMember(teamMemberId);
		}
	}
}