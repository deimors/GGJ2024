using System;
using Functional;
using UnityEngine;
using Zenject;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public abstract class AbstractWorldPrefabFactory<TParams> : IFactory<TParams, Unit>
	{
		private readonly DiContainer _container;
		private readonly GameObject _prefab;
		private readonly Transform _parent;

		protected AbstractWorldPrefabFactory(DiContainer container, GameObject prefab, Transform parent)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
			_prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
			_parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		public Unit Create(TParams parameters)
		{
			var subContainer = _container.CreateSubContainer();

			BindParameters(subContainer, parameters);

			var position = GetPosition(parameters);
			var rotation = GetRotation(parameters);

			subContainer.InstantiatePrefab(_prefab, position, rotation, _parent);

			return Unit.Value;
		}

		protected abstract void BindParameters(DiContainer subContainer, TParams parameters);
		protected abstract Vector3 GetPosition(TParams parameters);
		protected virtual Quaternion GetRotation(TParams parameters) => Quaternion.identity;
	}
}