using UnityEngine;

namespace UI
{
	internal abstract class BaseScaler : MonoBehaviour
	{
		protected virtual void Awake()
		{
			ApplyScale();
		}

		public abstract void ApplyScale();
	}
}
