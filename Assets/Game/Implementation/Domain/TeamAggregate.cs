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

		private const float TotalMove = 10.0f;

		private TeamMemberIdentifier _currentTeamMember;

		private Dictionary<TeamMemberIdentifier, TeamMemberState> _states;
		private HashSet<TeamMemberIdentifier> _killed = new();

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

			Observable.EveryUpdate().Skip(2).Take(1).Subscribe(_ => _events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember, 1, false)));

			return Unit.Value;
		}

		public Result<Unit, TeamError> MoveTeamMember(Vector3 targetVelocity)
		{
			if (_killed.Contains(_currentTeamMember))
				return Unit.Value;

			var currentState = _states[_currentTeamMember];

			if (currentState.MoveRemaining == 0)
				return Unit.Value;

			
			_events.OnNext(new TeamEvent.TeamMemberMoved(_currentTeamMember, targetVelocity));

			return Unit.Value;
		}

		public Result<Unit, TeamError> ReduceRemainingMove(TeamMemberIdentifier teamMemberId, float amount)
		{
			var currentState = _states[_currentTeamMember];

			if (currentState.MoveRemaining == 0)
				return Unit.Value;

			var newMoveRemaining = Math.Max(currentState.MoveRemaining - amount, 0);

			var remainingMovePercent = newMoveRemaining / TotalMove;

			_states[_currentTeamMember] = currentState with { MoveRemaining = newMoveRemaining };

			_events.OnNext(new TeamEvent.MoveRemainingReduced(teamMemberId, remainingMovePercent));

			if (newMoveRemaining == 0)
				_events.OnNext(new TeamEvent.TeamMemberMoveEnded(teamMemberId));

			if (NoMovementRemaining)
				_events.OnNext(new TeamEvent.TeamMovementDepleted());

			return Unit.Value;
		}

		public Result<Unit, TeamError> DeclareTeamMemberPosition(TeamMemberIdentifier teamMemberId, Vector3 position)
		{
			_events.OnNext(new TeamEvent.TeamMemberPositionDeclared(teamMemberId, position));

			return Unit.Value;
		}

		public Result<Unit, TeamError> SelectTeamMember(TeamMemberIdentifier teamMemberId)
		{
			_currentTeamMember = teamMemberId;
			var remainingMovePercent = _states[_currentTeamMember].MoveRemaining / TotalMove;

			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember, remainingMovePercent, _killed.Contains(teamMemberId)));

			return Unit.Value;
		}

		public Result<Unit, TeamError> StartTurn()
		{
			_states = _states
				.ToDictionary(
					pair => pair.Key, 
					pair => _killed.Contains(pair.Key) ? pair.Value : pair.Value with { MoveRemaining = TotalMove }
				);

			_events.OnNext(new TeamEvent.TeamTurnStarted());
			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember, 1, _killed.Contains(_currentTeamMember)));

			return Unit.Value;
		}

		public Result<Unit, TeamError> EndTurn()
		{
			if (NoMovementRemaining)
				_events.OnNext(new TeamEvent.TeamTurnEnded());

			return Unit.Value;
		}

		public Result<Unit, TeamError> ExitTeamMember(TeamMemberIdentifier teamMemberId)
		{
			_events.OnNext(new TeamEvent.TeamExited());

			return Unit.Value;
		}

		public Result<Unit, TeamError> KillTeamMember(TeamMemberIdentifier teamMemberId)
		{
			_killed.Add(teamMemberId);

			_events.OnNext(new TeamEvent.TeamMemberKilled(teamMemberId));

			return Unit.Value;
		}


		private bool NoMovementRemaining => _states.All(pair => pair.Value.MoveRemaining == 0);
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