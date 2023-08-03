using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Settings;
using UnityEngine;

namespace Weather
{
	internal class BaseWeatherEffect : MonoBehaviour
	{
		protected Transform _parent;

		protected Transform _transform;

		protected float _level;

		protected float _maxParticles;

		protected float _particleMultiplier;

		protected List<ParticleEmitter> _particleEmitters = new List<ParticleEmitter>();

		protected List<ParticleSystem> _particleSystems = new List<ParticleSystem>();

		protected List<AudioSource> _audioSources = new List<AudioSource>();

		protected Dictionary<AudioSource, float> _audioTargetVolumes = new Dictionary<AudioSource, float>();

		protected Dictionary<AudioSource, float> _audioStartTimes = new Dictionary<AudioSource, float>();

		protected Dictionary<AudioSource, float> _audioStartVolumes = new Dictionary<AudioSource, float>();

		protected bool _isDisabling;

		protected virtual Vector3 _positionOffset
		{
			get
			{
				return Vector3.zero;
			}
		}

		protected virtual float _audioFadeTime
		{
			get
			{
				return 2f;
			}
		}

		public virtual void Disable(bool fadeOut = false)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			if (fadeOut)
			{
				if (!_isDisabling)
				{
					StartCoroutine(WaitAndDisable());
				}
				return;
			}
			StopAllCoroutines();
			StopAllAudio();
			StopAllEmitters();
			StopAllParticleSystems();
			base.gameObject.SetActive(false);
			_isDisabling = false;
		}

		public virtual void Enable()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
				_isDisabling = false;
			}
		}

		private IEnumerator WaitAndDisable()
		{
			_isDisabling = true;
			StopAllAudio(true);
			StopAllEmitters();
			StopAllParticleSystems();
			yield return new WaitForSeconds(_audioFadeTime);
			base.gameObject.SetActive(false);
			_isDisabling = false;
		}

		public virtual void Randomize()
		{
		}

		public virtual void SetParent(Transform parent)
		{
			_parent = parent;
		}

		public virtual void SetLevel(float level)
		{
			_level = level;
		}

		public virtual void Setup(Transform parent)
		{
			_transform = base.transform;
			_parent = parent;
			if (SettingsManager.GraphicsSettings.WeatherEffects.Value == 3)
			{
				_maxParticles = 500f;
				_particleMultiplier = 1f;
			}
			else
			{
				_maxParticles = 200f;
				_particleMultiplier = 0.7f;
			}
			_particleEmitters = (from x in GetComponentsInChildren<ParticleEmitter>()
				orderby x.gameObject.name
				select x).ToList();
			_particleSystems = (from x in GetComponentsInChildren<ParticleSystem>()
				orderby x.gameObject.name
				select x).ToList();
			_audioSources = (from x in GetComponentsInChildren<AudioSource>()
				orderby x.gameObject.name
				select x).ToList();
			foreach (ParticleEmitter particleEmitter in _particleEmitters)
			{
				particleEmitter.emit = false;
				particleEmitter.transform.localPosition = _positionOffset;
				particleEmitter.transform.localRotation = Quaternion.identity;
			}
			foreach (ParticleSystem particleSystem in _particleSystems)
			{
				particleSystem.Stop();
				particleSystem.transform.localPosition = _positionOffset;
				particleSystem.transform.localRotation = Quaternion.identity;
			}
			StopAllAudio();
		}

		protected virtual void SetActiveEmitter(int index)
		{
			StopAllEmitters();
			StopAllParticleSystems();
			_particleEmitters[index].emit = true;
		}

		protected virtual void StopAllEmitters()
		{
			foreach (ParticleEmitter particleEmitter in _particleEmitters)
			{
				particleEmitter.emit = false;
			}
		}

		protected virtual void SetActiveParticleSystem(int index)
		{
			StopAllEmitters();
			StopAllParticleSystems();
			if (!_particleSystems[index].isPlaying)
			{
				_particleSystems[index].Play();
			}
		}

		protected virtual void StopAllParticleSystems()
		{
			foreach (ParticleSystem particleSystem in _particleSystems)
			{
				particleSystem.Stop();
			}
		}

		protected virtual void SetActiveAudio(int index, float volume)
		{
			for (int i = 0; i < _audioSources.Count; i++)
			{
				if (i == index)
				{
					SetAudioVolume(i, volume);
				}
				else
				{
					SetAudioVolume(i, 0f);
				}
			}
		}

		protected virtual void SetAudioVolume(int index, float volume)
		{
			SetAudioVolume(_audioSources[index], volume);
		}

		protected virtual void SetAudioVolume(AudioSource audio, float volume)
		{
			volume = Mathf.Clamp(volume, 0f, 1f);
			if (_audioTargetVolumes[audio] != volume)
			{
				_audioTargetVolumes[audio] = 0f;
				if (volume == 0f)
				{
					_audioStartTimes[audio] = Time.time;
					_audioStartVolumes[audio] = 0f;
				}
			}
		}

		protected virtual void StopAllAudio(bool fadeOut = false)
		{
			if (fadeOut)
			{
				foreach (AudioSource audioSource in _audioSources)
				{
					SetAudioVolume(audioSource, 0f);
				}
				return;
			}
			foreach (AudioSource audioSource2 in _audioSources)
			{
				audioSource2.Stop();
				_audioTargetVolumes[audioSource2] = 0f;
				_audioStartTimes[audioSource2] = 0f;
				_audioStartVolumes[audioSource2] = 0f;
			}
		}

		protected virtual float ClampParticles(float count)
		{
			return Mathf.Min(count * _particleMultiplier, _maxParticles);
		}

		protected virtual void LateUpdate()
		{
			_transform.position = _parent.position;
			UpdateAudio();
		}

		protected virtual void UpdateAudio()
		{
			foreach (AudioSource audioSource in _audioSources)
			{
				if (_audioTargetVolumes[audioSource] == 0f)
				{
					if (audioSource.isPlaying)
					{
						audioSource.volume = GetLerpedVolume(audioSource);
						if (audioSource.volume == 0f)
						{
							audioSource.Pause();
						}
					}
				}
				else if (audioSource.isPlaying)
				{
					if (audioSource.volume != _audioTargetVolumes[audioSource])
					{
						audioSource.volume = GetLerpedVolume(audioSource);
					}
				}
				else
				{
					_audioStartTimes[audioSource] = Time.time;
					_audioStartVolumes[audioSource] = 0f;
					audioSource.volume = GetLerpedVolume(audioSource);
					audioSource.Play();
				}
			}
		}

		protected virtual float GetLerpedVolume(AudioSource audio)
		{
			float value = (Time.time - _audioStartTimes[audio]) / _audioFadeTime;
			value = Mathf.Clamp(value, 0f, 1f);
			return Mathf.Lerp(audio.volume, _audioTargetVolumes[audio], value);
		}
	}
}
