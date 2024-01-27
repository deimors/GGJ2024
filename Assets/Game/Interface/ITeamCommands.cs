using System;
using Functional;
using Unit = Functional.Unit;

public interface ITeamCommands
{
	Result<Unit, TeamError> MoveTeamMember(float amount);
}

public interface ITeamEvents : IObservable<TeamEvent> {}

public abstract record TeamEvent
{
	public record TeamMemberMoved(float RemainingMovePercent) : TeamEvent;

	public record TeamMemberMoveEnded : TeamEvent;
}

public record TeamError
{
}