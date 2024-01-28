using System.Collections;
using System.Collections.Generic;
using Assets.Game.Implementation.Domain;
using UnityEngine;

public class TeamCameras : IReadOnlyDictionary<TeamMemberIdentifier, Camera>
{
	private readonly Dictionary<TeamMemberIdentifier, Camera> _cameras = new();

	public IEnumerator<KeyValuePair<TeamMemberIdentifier, Camera>> GetEnumerator()
	{
		return _cameras.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_cameras).GetEnumerator();
	}

	public int Count => _cameras.Count;

	public bool ContainsKey(TeamMemberIdentifier key)
	{
		return _cameras.ContainsKey(key);
	}

	public bool TryGetValue(TeamMemberIdentifier key, out Camera value)
	{
		return _cameras.TryGetValue(key, out value);
	}

	public Camera this[TeamMemberIdentifier key] => _cameras[key];

	public IEnumerable<TeamMemberIdentifier> Keys => _cameras.Keys;

	public IEnumerable<Camera> Values => _cameras.Values;

	public void Add(TeamMemberIdentifier teamMemberId, Camera camera) 
		=> _cameras[teamMemberId] = camera;

	public void Remove(TeamMemberIdentifier teamMemberId)
		=> _cameras.Remove(teamMemberId);
}