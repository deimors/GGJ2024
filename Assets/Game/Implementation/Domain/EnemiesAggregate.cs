using Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Unit = Functional.Unit;

namespace Assets.Game.Implementation.Domain
{
	public class EnemiesAggregate : IEnemyCommands, IEnemyEvents
	{
		private readonly Subject<EnemyEvent> _events = new();
		private IReadOnlyList<EnemyIdentifier> _enemies;
		private int _nextEnemyIndex;
		private readonly HashSet<EnemyIdentifier> _activated = new();

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

			_nextEnemyIndex = 1;

			_events.OnNext(new EnemyEvent.EnemyTurnStarted(firstEnemyId));

			return Unit.Value;
		}

		public Result<Unit, EnemyError> EndEnemyTurn(EnemyIdentifier enemyId)
		{
			Debug.Log($"End turn for {enemyId}");

			if (_nextEnemyIndex == _enemies.Count)
			{
				Debug.Log("All enemies done!");

				_events.OnNext(new EnemyEvent.EnemiesTurnEnded());

				return Unit.Value;
			}

			var nextEnemyId = _enemies[_nextEnemyIndex];
			_nextEnemyIndex++;
			_events.OnNext(new EnemyEvent.EnemyTurnStarted(nextEnemyId));

			return Unit.Value;
		}

		public Result<Unit, EnemyError> ActivateEnemy(EnemyIdentifier enemyId)
		{
			if (_activated.Contains(enemyId))
				return Unit.Value;

			_activated.Add(enemyId);

			_events.OnNext(new EnemyEvent.EnemyActivated(enemyId));

			return Unit.Value;
		}
	}
}