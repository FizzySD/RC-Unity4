using UnityEngine;

namespace Weather
{
	internal class WindWeatherEffect : BaseWeatherEffect
	{
		public static bool WindEnabled = false;

		public static Vector3 WindDirection = Vector3.zero;

		protected override Vector3 _positionOffset
		{
			get
			{
				return Vector3.up * 0f;
			}
		}

		public override void Setup(Transform parent)
		{
			base.Setup(parent);
		}

		public override void Randomize()
		{
			WindDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
		}

		public override void Disable(bool fadeOut = false)
		{
			WindEnabled = false;
			base.Disable(fadeOut);
		}

		public override void SetLevel(float level)
		{
			base.SetLevel(level);
			if (!(level <= 0f))
			{
				WindEnabled = true;
				if (level < 0.5f)
				{
					SetAudioVolume(0, 1f);
				}
				else
				{
					SetAudioVolume(1, 0.5f);
				}
			}
		}
	}
}
