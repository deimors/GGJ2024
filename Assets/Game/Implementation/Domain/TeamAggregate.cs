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

		private const float TotalMove = 5.0f;

		private TeamMemberIdentifier _currentTeamMember;

		private Dictionary<TeamMemberIdentifier, TeamMemberState> _states;

		public IDisposable Subscribe(IObserver<TeamEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, TeamError> Initialize(TeamConfig config)
		{
			_states = config.Positions.ToDictionary(pair => pair.Key, _ => new TeamMemberState(TotalMove));
			_currentTeamMember = config.Positions.Keys.First();

			foreach (var (teamMemberId, position) in config.Positions)
			{
				_events.OnNext(new TeamEvent.TeamMemberCreated(teamMemberId, position));
			}

			Observable.NextFrame().Subscribe(_ => _events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember, 1.0f)));

			return Unit.Value;
		}

		public Result<Unit, TeamError> MoveTeamMember(Vector3 targetVelocity, float amount)
		{
			var currentState = _states[_currentTeamMember];

			if (currentState.MoveRemaining == 0)
				return Unit.Value;

			var newMoveRemaining = Math.Max(currentState.MoveRemaining - amount, 0);

			var remainingMovePercent = newMoveRemaining / TotalMove;

			_states[_currentTeamMember] = currentState with { MoveRemaining = newMoveRemaining };

			if (newMoveRemaining > 0)
				_events.OnNext(new TeamEvent.TeamMemberMoved(_currentTeamMember, targetVelocity, remainingMovePercent));
			else
				_events.OnNext(new TeamEvent.TeamMemberMoveEnded(_currentTeamMember));

			return Unit.Value;
		}

		public Result<Unit, TeamError> SelectTeamMember(TeamMemberIdentifier teamMemberId)
		{
			_currentTeamMember = teamMemberId;
			var remainingMovePercent = _states[_currentTeamMember].MoveRemaining / TotalMove;

			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember, remainingMovePercent));

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