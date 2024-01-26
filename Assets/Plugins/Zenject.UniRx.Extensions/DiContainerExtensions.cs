using System;
using UnityEngine;
using Zenject;
using Unit=Functional.Unit;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public static class DiContainerExtensions
	{
		public static IfNotBoundBinder BindIntegration<TIntegration>(this DiContainer container) 
			where TIntegration : IDisposable
			=> container.Bind<IDisposable>().To<TIntegration>().AsSingle().NonLazy();

		public static ConcreteIdArgConditionCopyNonLazyBinder BindModel<TModel>(this DiContainer container)
			=> container.BindInterfacesTo<TModel>().AsSingle();

		public static IfNotBoundBinder BindReadModel<TReadModel>(this DiContainer container)
			where TReadModel : IDisposable
			=> container.BindInterfacesTo<TReadModel>().AsSingle().NonLazy();

		public static void BindPrefabFactory<TParams, TFactory>(this DiContainer container, GameObject prefab,
			Transform parent)
			where TFactory : IFactory<TParams, Unit>
			=> container.BindPrefabFactory<TParams, TFactory, GameObject>(prefab, parent);

		public static void BindPrefabFactory<TParams, TFactory, TPrefab>(this DiContainer container, TPrefab prefab, Transform parent)
			where TFactory : IFactory<TParams, Unit>
		{
			container.BindInstance(prefab).WhenInjectedInto<TFactory>();
			container.BindInstance(parent).WhenInjectedInto<TFactory>();
			container.BindIFactory<TParams, Unit>().FromFactory<TFactory>();
		}

		public static void BindPrefabFactory<TParams>(
			this DiContainer container,
			GameObject prefab,
			Transform parent,
			Action<DiContainer, TParams> bindParameters,
			Func<TParams, Vector3> getPosition
		) => container.BindPrefabFactory<TParams, GameObject>(prefab, parent, bindParameters, getPosition);

		public static void BindPrefabFactory<TParams, TPrefab>(
			this DiContainer container, 
			TPrefab prefab, 
			Transform parent,
			Action<DiContainer, TParams> bindParameters,
			Func<TParams, Vector3> getPosition)
		{
			container.BindInstance(prefab).WhenInjectedInto<DelegatedWorldPrefabFactory<TParams>>();
			container.BindInstance(parent).WhenInjectedInto<DelegatedWorldPrefabFactory<TParams>>();
			container.BindInstance(bindParameters).WhenInjectedInto<DelegatedWorldPrefabFactory<TParams>>();
			container.BindInstance(getPosition).WhenInjectedInto<DelegatedWorldPrefabFactory<TParams>>();

			container.BindIFactory<TParams, Unit>().FromFactory<DelegatedWorldPrefabFactory<TParams>>();
		}

		public static EventBinding<TObservableEvent, TEvent> BindEvent<TObservableEvent, TEvent>(this DiContainer container) 
			where TEvent : TObservableEvent 
			=> new(container);
	}
}