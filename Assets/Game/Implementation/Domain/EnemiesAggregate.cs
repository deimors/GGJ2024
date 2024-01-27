using Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class EnemiesAggregate : IEnemyCommands, IEnemyEvents
	{
		private readonly Subject<EnemyEvent> _events = new();
		private IReadOnlyList<EnemyIdentifier> _enemies;

		public IDisposable Subscribe(IObserver<EnemyEvent> observer) 
			=> _events.Subscribe(observer);

		public Result<Unit, EnemyError> Initialize(EnemiesConfig config)
		{
			_enemies = config.Positions.Keys.ToArray();

			foreach (var (enemyId, position) in config.Positions)
			{
				_events.OnNext(new EnemyEvent.EnemyCreated(enemyId, position));
			}

			return Unit.Value;
		}

		public Result<Unit, EnemyError> StartTurn()
		{
			var firstEnemyId = _enemies[0];

			_events.OnNext(new EnemyEvent.EnemyTurnStarted(firstEnemyId));

			return Unit.Value;
		}
	}
}