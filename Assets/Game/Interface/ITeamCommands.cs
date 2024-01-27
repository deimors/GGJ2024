using System;
using Functional;
using Unit = Functional.Unit;

public interface ITeamCommands
{
	Result<Unit, TeamError> MoveTeamMember(double amount);
}

public interface ITeamEvents : IObservable<TeamEvent> {}

public abstract record TeamEvent
{
}

public record TeamError
{
}