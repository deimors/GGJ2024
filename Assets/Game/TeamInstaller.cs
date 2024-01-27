using Assets.Game.Implementation.Domain;
using Assets.Plugins.Zenject.UniRx.Extensions;
using Zenject;

public class TeamInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.BindModel<TeamAggregate>();

		Container.BindIntegration<InitializeTeamOnFirstFrame>();
	}
}