using System;
using UniRx;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public class EventActionBinding<TObservableEvent, TEvent> : IDisposable
		where TEvent : TObservableEvent
	{
		private readonly IDisposable _disposable;

		public EventActionBinding(IObservable<TObservableEvent> events, Action<TEvent> action)
			=> _disposable = events
				.OfType<TObservableEvent, TEvent>()
				.Subscribe(action);

		public void Dispose()
			=> _disposable.Dispose();
	}
}