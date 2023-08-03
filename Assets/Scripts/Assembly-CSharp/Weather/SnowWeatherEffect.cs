using UnityEngine;

namespace Weather
{
	internal class SnowWeatherEffect : BaseWeatherEffect
	{
		protected override Vector3 _positionOffset
		{
			get
			{
				return Vector3.up * 0f;
			}
		}

		public override void Randomize()
		{
			_particleEmitters[0].rndVelocity = new Vector3(20f, 20f, 20f);
			ParticleEmitter obj = _particleEmitters[0];
			float minEnergy = (_particleEmitters[0].maxEnergy = 1.2f);
			obj.minEnergy = minEnergy;
			_particleEmitters[1].rndVelocity = new Vector3(5f, 5f, 5f);
			_particleEmitters[1].localVelocity = new Vector3(20f * Util.GetRandomSign(), 0f, 0f);
			ParticleEmitter obj2 = _particleEmitters[1];
			minEnergy = (_particleEmitters[0].maxEnergy = 1.2f);
			obj2.minEnergy = minEnergy;
		}

		public override void SetLevel(float level)
		{
			base.SetLevel(level);
			if (!(level <= 0f))
			{
				if (level <= 0.5f)
				{
					float num = level / 0.5f;
					SetActiveEmitter(0);
					SetActiveAudio(0, 0.25f + 0.25f * num);
					ParticleEmitter obj = _particleEmitters[0];
					float minEmission = (_particleEmitters[0].maxEmission = ClampParticles(100f + num * 300f));
					obj.minEmission = minEmission;
					ParticleEmitter obj2 = _particleEmitters[0];
					minEmission = (_particleEmitters[0].maxSize = 25f);
					obj2.minSize = minEmission;
				}
				else
				{
					float num4 = (level - 0.5f) / 0.5f;
					SetActiveEmitter(1);
					SetAudioVolume(1, 0.25f + 0.25f * num4);
					ParticleEmitter obj3 = _particleEmitters[1];
					float minEmission = (_particleEmitters[1].maxEmission = ClampParticles(200f + num4 * 200f));
					obj3.minEmission = minEmission;
					ParticleEmitter obj4 = _particleEmitters[1];
					minEmission = (_particleEmitters[1].maxSize = 12f);
					obj4.minSize = minEmission;
				}
			}
		}

		public override void Setup(Transform parent)
		{
			base.Setup(parent);
		}
	}
}
