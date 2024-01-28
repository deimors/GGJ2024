using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Game.Implementation.Domain;
using UniRx;
using UnityEngine;

public class TeamPositions : IReadOnlyDictionary<TeamMemberIdentifier, Vector3>, IDisposable
{
	private readonly Dictionary<TeamMemberIdentifier, Vector3> _positions = new();
	private readonly CompositeDisposable _disposables;

	public TeamPositions(ITeamEvents teamEvents)
	{
		_disposables = new CompositeDisposable();

		teamEvents.OfType<TeamEvent, TeamEvent.TeamMemberPositionDeclared>()
			.Do(Debug.Log)
			.Subscribe(declared => _positions[declared.TeamMemberId] = declared.Position)
			.AddTo(_disposables);

		teamEvents.OfType<TeamEvent, TeamEvent.TeamMemberKilled>()
			.Subscribe(killed => _positions.Remove(killed.TeamMemberId))
			.AddTo(_disposables);
	}

	public IEnumerator<KeyValuePair<TeamMemberIdentifier, Vector3>> GetEnumerator() => _positions.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_positions).GetEnumerator();

	public int Count => _positions.Count;

	public bool ContainsKey(TeamMemberIdentifier key) => _positions.ContainsKey(key);

	public bool TryGetValue(TeamMemberIdentifier key, out Vector3 value) => _positions.TryGetValue(key, out value);

	public Vector3 this[TeamMemberIdentifier key] => _positions[key];

	public IEnumerable<TeamMemberIdentifier> Keys => _positions.Keys;

	public IEnumerable<Vector3> Values => _positions.Values;

	public void Dispose() => _disposables.Dispose();
}