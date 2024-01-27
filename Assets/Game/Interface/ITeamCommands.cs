using System;
using Functional;
using Unit = Functional.Unit;

public interface ITeamCommands
{
	Result<Unit, TeamError> Initialize();
	Result<Unit, TeamError> MoveTeamMember(float amount);
	Result<Unit, TeamError> SelectTeamMember(int teamMemberId);
}

public interface ITeamEvents : IObservable<TeamEvent> {}

public abstract record TeamEvent
{
	public record TeamMemberMoved(float RemainingMovePercent) : TeamEvent;

	public record TeamMemberMoveEnded(int TeamMemberId) : TeamEvent;

	public record TeamMemberSelected(int TeamMemberId) : TeamEvent;
}

public record TeamError
{
}