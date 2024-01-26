using System;
using UniRx;
using Zenject;
using Unit=Functional.Unit;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public class EventFactoryBinding<TObservableEvent, TEvent, TParams> : IDisposable
		where TEvent : TObservableEvent
	{
		private readonly IDisposable _disposable;

		public EventFactoryBinding(IObservable<TObservableEvent> events, IFactory<TParams, Unit> factory, Func<TEvent, TParams> map)
			=> _disposable = events
				.OfType<TObservableEvent, TEvent>()
				.Select(map)
				.Subscribe(parameters => factory.Create(parameters));

		public void Dispose()
			=> _disposable.Dispose();
	}
}