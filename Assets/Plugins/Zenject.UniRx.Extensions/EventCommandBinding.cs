using System;
using Functional;
using UniRx;
using Unit = Functional.Unit;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public class EventCommandBinding<TObservableEvent, TEvent, TCommands, TFailure> : IDisposable
	{
		private readonly IDisposable _disposable;

		public EventCommandBinding(
			IObservable<TObservableEvent> events, 
			TCommands commands, 
			Func<TEvent, TCommands, Result<Unit, TFailure>> binding,
			Action<TFailure> onFailure
		) => _disposable = events
				.OfType<TObservableEvent, TEvent>()
				.Subscribe(@event => binding(@event, commands).DoOnFailure(onFailure));

		public void Dispose()
			=> _disposable.Dispose();
	}
}