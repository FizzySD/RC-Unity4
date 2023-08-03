using System.Collections.Generic;
using ApplicationManagers;
using Settings;
using UnityEngine;

namespace Weather
{
	internal class ThunderWeatherEffect : BaseWeatherEffect
	{
		public static List<List<LightningParticle>> LightningPool = new List<List<LightningParticle>>();

		protected float _lightningWaitTime = Random.Range(10f, 20f);

		private const int MaxLightningParticles = 4;

		protected override Vector3 _positionOffset
		{
			get
			{
				return Vector3.up * 0f;
			}
		}

		public static void FinishLoadAssets()
		{
			for (int i = 0; i < 10; i++)
			{
				Vector3 vector = Vector3.up * 1500f + Vector3.right * Random.Range(-1000f, 1000f);
				Vector3 vector2 = Vector3.down * 300f + Vector3.right * Random.Range(-1000f, 1000f);
				float num = Vector3.Distance(vector, vector2);
				int generation = 9;
				if (SettingsManager.GraphicsSettings.WeatherEffects.Value == 2)
				{
					generation = 8;
				}
				List<LightningParticle> list = new List<LightningParticle>();
				for (int j = 0; j < 4; j++)
				{
					LightningParticle lightningParticle = AssetBundleManager.InstantiateAsset<GameObject>("LightningParticle").AddComponent<LightningParticle>();
					Object.DontDestroyOnLoad(lightningParticle.gameObject);
					lightningParticle.Setup(vector, vector2, generation);
					list.Add(lightningParticle);
					lightningParticle.Disable();
				}
				LightningPool.Add(list);
			}
		}

		public override void Randomize()
		{
		}

		public override void Setup(Transform parent)
		{
			base.Setup(parent);
		}

		public override void SetLevel(float level)
		{
			base.SetLevel(level);
			if (!(level <= 0f))
			{
				if (level < 0.5f)
				{
					SetActiveAudio(0, 0f);
				}
				else
				{
					SetActiveAudio(0, 0f);
				}
			}
		}

		protected void FixedUpdate()
		{
			_lightningWaitTime -= Time.fixedDeltaTime;
			if (_lightningWaitTime <= 0f)
			{
				_lightningWaitTime = 20f - _level * 15f;
				_lightningWaitTime = Random.Range(_lightningWaitTime * 0.5f, _lightningWaitTime * 1.5f);
				CreateLightning();
			}
		}

		protected void CreateLightning()
		{
			List<LightningParticle> list = LightningPool[Random.Range(0, LightningPool.Count)];
			int num = Random.Range(1, 4);
			float fieldOfView = Camera.main.fieldOfView;
			Vector3 normalized = new Vector3(_parent.forward.x, 0f, _parent.forward.z).normalized;
			Vector3 vector = Quaternion.AngleAxis(Random.Range((0f - fieldOfView) * 0.5f, fieldOfView * 0.5f), Vector3.up) * normalized;
			float num2 = Random.Range(900f, 1400f);
			Vector3 position = base.transform.position + normalized * num2;
			for (int i = 0; i < num; i++)
			{
				list[i].transform.position = position;
				list[i].transform.LookAt(_parent);
				list[i].Enable();
				list[i].Strike(i == 0);
			}
		}
	}
}
