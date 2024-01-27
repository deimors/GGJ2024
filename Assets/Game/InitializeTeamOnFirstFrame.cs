using System;
using UniRx;

public class InitializeTeamOnFirstFrame : IDisposable
{
	private readonly IDisposable _disposable;

	public InitializeTeamOnFirstFrame(ITeamCommands teamCommands) =>
		_disposable = Observable
			.NextFrame()
			.Subscribe(_ => teamCommands.Initialize());

	public void Dispose() 
		=> _disposable.Dispose();
}