using Assets.Game.Implementation.Domain;
using UnityEngine;

public abstract record TeamEvent
{
	public record TeamMemberMoved(TeamMemberIdentifier TeamMemberId, Vector3 TargetVelocity, float RemainingMovePercent) : TeamEvent;

	public record TeamMemberMoveEnded(TeamMemberIdentifier TeamMemberId) : TeamEvent;

	public record TeamMemberSelected(TeamMemberIdentifier TeamMemberId, float RemainingMovePercent) : TeamEvent;

	public record TeamMemberCreated(TeamMemberIdentifier TeamMemberId, Vector3 Position) : TeamEvent;

	public record TeamTurnEnded : TeamEvent;
}