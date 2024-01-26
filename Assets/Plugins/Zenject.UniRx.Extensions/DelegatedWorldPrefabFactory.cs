using System;
using UnityEngine;
using Zenject;

namespace Assets.Plugins.Zenject.UniRx.Extensions
{
	public class DelegatedWorldPrefabFactory<TParams> : AbstractWorldPrefabFactory<TParams>
	{
		private readonly Action<DiContainer, TParams> _bindParameters;
		private readonly Func<TParams, Vector3> _getPosition;

		public DelegatedWorldPrefabFactory(
			DiContainer container,
			GameObject prefab,
			Transform parent,
			Action<DiContainer, TParams> bindParameters,
			Func<TParams, Vector3> getPosition
		) : base(container, prefab, parent)
		{
			_bindParameters = bindParameters;
			_getPosition = getPosition;
		}

		protected override void BindParameters(DiContainer subContainer, TParams parameters)
			=> _bindParameters(subContainer, parameters);

		protected override Vector3 GetPosition(TParams parameters)
			=> _getPosition(parameters);
	}
}