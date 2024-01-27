using Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class TeamAggregate : ITeamCommands, ITeamEvents
	{
		private readonly Subject<TeamEvent> _events = new();

		private float _totalMove = 5.0f;
		private float _moveRemaining = 5.0f;

		private TeamMemberIdentifier _currentTeamMember;

		private Dictionary<TeamMemberIdentifier, TeamMemberState> _states;

		public IDisposable Subscribe(IObserver<TeamEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, TeamError> Initialize(TeamConfig config)
		{
			_states = config.Positions.ToDictionary(pair => pair.Key, _ => new TeamMemberState(_totalMove));
			_currentTeamMember = config.Positions.Keys.First();

			foreach (var (teamMemberId, position) in config.Positions)
			{
				_events.OnNext(new TeamEvent.TeamMemberCreated(teamMemberId, position));
			}

			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember));

			return Unit.Value;
		}

		public Result<Unit, TeamError> MoveTeamMember(float amount)
		{
			_moveRemaining = Math.Max(_moveRemaining - amount, 0);

			var remainingMovePercent = _moveRemaining / _totalMove;

			_events.OnNext(new TeamEvent.TeamMemberMoved(remainingMovePercent));

			if (_moveRemaining <= 0)
				_events.OnNext(new TeamEvent.TeamMemberMoveEnded(_currentTeamMember));

			return Unit.Value;
		}

		public Result<Unit, TeamError> SelectTeamMember(TeamMemberIdentifier teamMemberId)
		{
			_currentTeamMember = teamMemberId;

			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember));

			return Unit.Value;
		}
	}

	internal record TeamMemberState(float MoveRemaining);

	public record TeamConfig(
		IReadOnlyDictionary<TeamMemberIdentifier, Vector3> Positions
	);

	public record TeamMemberIdentifier(int Value)
	{
		private static int _nextValue;

		public static TeamMemberIdentifier Create() => new(_nextValue++);
	}
}