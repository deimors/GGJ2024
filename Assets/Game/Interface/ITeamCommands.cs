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