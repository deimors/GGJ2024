using System;
using Functional;
using UniRx;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class EnemiesAggregate : IEnemyCommands, IEnemyEvents
	{
		private readonly Subject<EnemyEvent> _events = new();

		public IDisposable Subscribe(IObserver<EnemyEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, EnemyError> Initialize(EnemiesConfig config)
		{
			foreach (var (enemyId, position) in config.Positions)
			{
				_events.OnNext(new EnemyEvent.EnemyCreated(enemyId, position));
			}

			return Unit.Value;
		}
	}
}