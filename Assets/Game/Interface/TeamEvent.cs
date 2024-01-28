using Assets.Game.Implementation.Domain;
using UnityEngine;

public abstract record TeamEvent
{
	public record TeamMemberMoved(TeamMemberIdentifier TeamMemberId, Vector3 TargetVelocity) : TeamEvent;

	public record MoveRemainingReduced(TeamMemberIdentifier TeamMemberId, float RemainingMovePercent) : TeamEvent;

	public record TeamMemberMoveEnded(TeamMemberIdentifier TeamMemberId) : TeamEvent;

	public record TeamMemberSelected(TeamMemberIdentifier TeamMemberId, float RemainingMovePercent, bool isDead) : TeamEvent;

	public record TeamMemberCreated(TeamMemberIdentifier TeamMemberId, Vector3 Position) : TeamEvent;

	public record TeamMovementDepleted : TeamEvent;

	public record TeamTurnStarted : TeamEvent;

	public record TeamTurnEnded : TeamEvent;

	public record TeamExited : TeamEvent;

	public record TeamMemberPositionDeclared(TeamMemberIdentifier TeamMemberId, Vector3 Position) : TeamEvent;

	public record TeamMemberKilled(TeamMemberIdentifier TeamMemberId) : TeamEvent;
}