using Functional;
using System;
using UniRx;
using UnityEngine;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class TeamAggregate : ITeamCommands, ITeamEvents
	{
		private readonly Subject<TeamEvent> _events = new();

		private float _totalMove = 1.0f;
		private float _moveRemaining = 1.0f;

		public IDisposable Subscribe(IObserver<TeamEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, TeamError> MoveTeamMember(float amount)
		{
			_moveRemaining = Math.Max(_moveRemaining - amount, 0);

			var remainingMovePercent = _moveRemaining / _totalMove;

			Debug.Log(remainingMovePercent);

			_events.OnNext(new TeamEvent.TeamMemberMoved(remainingMovePercent));

			if (_moveRemaining <= 0)
				_events.OnNext(new TeamEvent.TeamMemberMoveEnded());

			return Unit.Value;
		}
	}
}