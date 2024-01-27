using System;
using Functional;
using UniRx;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class TeamAggregate : ITeamCommands, ITeamEvents
	{
		private readonly Subject<TeamEvent> _events = new();

		public IDisposable Subscribe(IObserver<TeamEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, TeamError> MoveTeamMember(double amount)
		{
			throw new NotImplementedException();
		}
	}
}