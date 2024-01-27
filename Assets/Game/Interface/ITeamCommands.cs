using System;
using Assets.Game.Implementation.Domain;
using Functional;
using UnityEngine;
using Unit = Functional.Unit;

public interface ITeamCommands
{
	Result<Unit, TeamError> Initialize(TeamConfig config);
	Result<Unit, TeamError> MoveTeamMember(Vector3 targetVelocity, float amount);
	Result<Unit, TeamError> SelectTeamMember(TeamMemberIdentifier teamMemberId);
}

public interface ITeamEvents : IObservable<TeamEvent> {}

public abstract record TeamEvent
{
	public record TeamMemberMoved(TeamMemberIdentifier TeamMemberId, Vector3 TargetVelocity, float RemainingMovePercent) : TeamEvent;

	public record TeamMemberMoveEnded(TeamMemberIdentifier TeamMemberId) : TeamEvent;

	public record TeamMemberSelected(TeamMemberIdentifier TeamMemberId) : TeamEvent;

	public record TeamMemberCreated(TeamMemberIdentifier TeamMemberId, Vector3 Position) : TeamEvent;
}

public record TeamError
{
}