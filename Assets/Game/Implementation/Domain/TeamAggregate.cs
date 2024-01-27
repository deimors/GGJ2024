using Functional;
using System;
using UniRx;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class TeamAggregate : ITeamCommands, ITeamEvents
	{
		private readonly Subject<TeamEvent> _events = new();

		private float _totalMove = 5.0f;
		private float _moveRemaining = 5.0f;

		private int _currentTeamMember = 0;

		public IDisposable Subscribe(IObserver<TeamEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, TeamError> Initialize()
		{
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

		public Result<Unit, TeamError> SelectTeamMember(int teamMemberId)
		{
			_currentTeamMember = teamMemberId;

			_events.OnNext(new TeamEvent.TeamMemberSelected(_currentTeamMember));

			return Unit.Value;
		}
	}
}