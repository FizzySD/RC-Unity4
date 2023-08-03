using UnityEngine;

namespace Weather
{
	internal class RainWeatherEffect : BaseWeatherEffect
	{
		protected override Vector3 _positionOffset
		{
			get
			{
				return Vector3.up * 30f;
			}
		}

		public override void Randomize()
		{
			float num = Random.Range(0f, 20f);
			num = Random.Range(0f - num, num);
			foreach (ParticleEmitter particleEmitter in _particleEmitters)
			{
				particleEmitter.transform.localPosition = _positionOffset;
				particleEmitter.transform.localRotation = Quaternion.identity;
				particleEmitter.transform.RotateAround(_transform.position, Vector3.forward, num);
				particleEmitter.transform.RotateAround(_transform.position, Vector3.up, Random.Range(0f, 360f));
			}
		}

		public override void SetLevel(float level)
		{
			base.SetLevel(level);
			if (!(level <= 0f))
			{
				if (level < 0.5f)
				{
					float num = level / 0.5f;
					SetActiveEmitter(0);
					ParticleEmitter obj = _particleEmitters[0];
					float minEmission = (_particleEmitters[0].maxEmission = ClampParticles(50f + 150f * num));
					obj.minEmission = minEmission;
					ParticleEmitter obj2 = _particleEmitters[0];
					minEmission = (_particleEmitters[0].maxSize = 30f + 30f * num);
					obj2.minSize = minEmission;
					SetActiveAudio(0, 0.25f + 0.25f * num);
				}
				else
				{
					float num4 = (level - 0.5f) / 0.5f;
					SetActiveEmitter(1);
					ParticleEmitter obj3 = _particleEmitters[1];
					float minEmission = (_particleEmitters[1].maxEmission = ClampParticles(100f + 150f * num4));
					obj3.minEmission = minEmission;
					ParticleEmitter obj4 = _particleEmitters[1];
					minEmission = (_particleEmitters[1].maxSize = 50f + num4 * 10f);
					obj4.minSize = minEmission;
					SetActiveAudio(1, 0.25f + 0.25f * num4);
				}
			}
		}

		public override void Setup(Transform parent)
		{
			base.Setup(parent);
			_particleEmitters[0].localVelocity = Vector3.down * 100f;
			_particleEmitters[1].localVelocity = Vector3.down * 100f;
			_particleEmitters[0].rndVelocity = new Vector3(10f, 0f, 10f);
			_particleEmitters[1].rndVelocity = new Vector3(10f, 0f, 10f);
		}
	}
}
