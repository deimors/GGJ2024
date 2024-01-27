using Assets.Game.Implementation.Domain;
using Functional;
using UnityEngine;
using Unit = Functional.Unit;

public interface ITeamCommands
{
	Result<Unit, TeamError> Initialize(TeamConfig config);
	Result<Unit, TeamError> MoveTeamMember(Vector3 targetVelocity, float amount);
	Result<Unit, TeamError> DeclareTeamMemberPosition(TeamMemberIdentifier teamMemberId, Vector3 position);
	Result<Unit, TeamError> SelectTeamMember(TeamMemberIdentifier teamMemberId);
	Result<Unit, TeamError> EndTurn();
	Result<Unit, TeamError> ExitTeamMember(TeamMemberIdentifier teamMemberId);
}