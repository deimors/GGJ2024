using System;
using System.Collections.Generic;
using Functional;
using UnityEngine;
using Unit = Functional.Unit;

public interface IEnemyCommands
{
	Result<Unit, EnemyError> Initialize(EnemiesConfig config);

	Result<Unit, EnemyError> StartTurn();
	Result<Unit, EnemyError> EndEnemyTurn(EnemyIdentifier enemyId);
}

public interface IEnemyEvents : IObservable<EnemyEvent> {}

public abstract record EnemyEvent
{
	public record EnemyCreated(EnemyIdentifier EnemyId, Vector3 Position) : EnemyEvent;

	public record EnemyTurnStarted(EnemyIdentifier EnemyId) : EnemyEvent;
}

public record EnemiesConfig(IReadOnlyDictionary<EnemyIdentifier, Vector3> Positions);

public record EnemyIdentifier(int Value)
{
	private static int _nextValue;

	public static EnemyIdentifier Create() => new(_nextValue++);
}

public record EnemyError;