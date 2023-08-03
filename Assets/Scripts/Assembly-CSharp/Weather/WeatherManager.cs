using System.Collections;
using System.Collections.Generic;
using ApplicationManagers;
using CustomSkins;
using GameManagers;
using Settings;
using UnityEngine;
using Utility;

namespace Weather
{
	internal class WeatherManager : MonoBehaviour
	{
		private static WeatherManager _instance;

		private const float LerpDelay = 0.05f;

		private const float SyncDelay = 5f;

		private HashSet<WeatherEffect> LowEffects = new HashSet<WeatherEffect>
		{
			WeatherEffect.Daylight,
			WeatherEffect.AmbientLight,
			WeatherEffect.Flashlight,
			WeatherEffect.Skybox
		};

		private static Dictionary<string, Material> SkyboxMaterials = new Dictionary<string, Material>();

		private static Dictionary<string, Dictionary<string, Material>> SkyboxBlendedMaterials = new Dictionary<string, Dictionary<string, Material>>();

		private static Shader _blendedShader;

		private List<WeatherScheduleRunner> _scheduleRunners = new List<WeatherScheduleRunner>();

		private Dictionary<WeatherEffect, BaseWeatherEffect> _effects = new Dictionary<WeatherEffect, BaseWeatherEffect>();

		public WeatherSet _currentWeather = new WeatherSet();

		public WeatherSet _targetWeather = new WeatherSet();

		public WeatherSet _startWeather = new WeatherSet();

		public Dictionary<int, float> _targetWeatherStartTimes = new Dictionary<int, float>();

		public Dictionary<int, float> _targetWeatherEndTimes = new Dictionary<int, float>();

		private List<WeatherEffect> _needApply = new List<WeatherEffect>();

		public float _currentTime;

		public bool _needSync;

		public Dictionary<WeatherScheduleRunner, float> _currentScheduleWait = new Dictionary<WeatherScheduleRunner, float>();

		private float _currentLerpWait;

		private float _currentSyncWait;

		private bool _finishedLoading;

		private Light _mainLight;

		private Skybox _skybox;

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
		}

		public static void FinishLoadAssets()
		{
			LoadSkyboxes();
			ThunderWeatherEffect.FinishLoadAssets();
			_instance.StartCoroutine(_instance.RestartWeather());
		}

		private static void LoadSkyboxes()
		{
			_blendedShader = AssetBundleManager.InstantiateAsset<Shader>("SkyboxBlendShader");
			string[] array = RCextensions.EnumToStringArray<WeatherSkybox>();
			string[] parts = RCextensions.EnumToStringArray<SkyboxCustomSkinPartId>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				SkyboxMaterials.Add(text, AssetBundleManager.InstantiateAsset<Material>(text.ToString() + "Skybox"));
			}
			string[] array3 = array;
			foreach (string text2 in array3)
			{
				SkyboxBlendedMaterials.Add(text2, new Dictionary<string, Material>());
				string[] array4 = array;
				foreach (string text3 in array4)
				{
					Material value = CreateBlendedSkybox(_blendedShader, parts, text2, text3);
					SkyboxBlendedMaterials[text2].Add(text3, value);
				}
			}
		}

		public static void TakeFlashlight(Transform parent)
		{
			if (_instance._effects.ContainsKey(WeatherEffect.Flashlight) && _instance._effects[WeatherEffect.Flashlight] != null)
			{
				_instance._effects[WeatherEffect.Flashlight].SetParent(parent);
			}
		}

		private static Material CreateBlendedSkybox(Shader shader, string[] parts, string skybox1, string skybox2)
		{
			Material material = new Material(shader);
			foreach (string text in parts)
			{
				string text2 = "_" + text + "Tex";
				material.SetTexture(text2, SkyboxMaterials[skybox1].GetTexture(text2));
				material.SetTexture(text2 + "2", SkyboxMaterials[skybox2].GetTexture(text2));
			}
			SetSkyboxBlend(material, 0f);
			return material;
		}

		private static void SetSkyboxBlend(Material skybox, float blend)
		{
			skybox.SetFloat("_Blend", blend);
		}

		private void Cache()
		{
			_mainLight = GameObject.Find("mainLight").GetComponent<Light>();
			_skybox = Camera.main.GetComponent<Skybox>();
		}

		private void ResetSkyboxColors()
		{
			foreach (string key in SkyboxBlendedMaterials.Keys)
			{
				foreach (string key2 in SkyboxBlendedMaterials[key].Keys)
				{
					SkyboxBlendedMaterials[key][key2].SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f));
				}
			}
		}

		private IEnumerator RestartWeather()
		{
			while (Camera.main == null)
			{
				yield return null;
			}
			Cache();
			ResetSkyboxColors();
			_scheduleRunners.Clear();
			_effects.Clear();
			_currentWeather.SetDefault();
			_startWeather.SetDefault();
			_targetWeather.SetDefault();
			_targetWeatherStartTimes.Clear();
			_targetWeatherEndTimes.Clear();
			_needApply.Clear();
			_currentTime = 0f;
			_currentScheduleWait.Clear();
			CreateEffects();
			if (Application.loadedLevel == 0 && SettingsManager.GraphicsSettings.AnimatedIntro.Value)
			{
				SetMainMenuWeather();
			}
			ApplyCurrentWeather(true, true);
			bool flag = SettingsManager.GraphicsSettings.WeatherEffects.Value == 0;
			if (Application.loadedLevel != 0 && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || PhotonNetwork.isMasterClient))
			{
				if (!flag)
				{
					_currentWeather.Copy(SettingsManager.WeatherSettings.WeatherSets.GetSelectedSet());
					CreateScheduleRunners(_currentWeather.Schedule.Value);
					_currentWeather.Schedule.SetDefault();
				}
				if (_currentWeather.UseSchedule.Value)
				{
					foreach (WeatherScheduleRunner scheduleRunner in _scheduleRunners)
					{
						scheduleRunner.ProcessSchedule();
						scheduleRunner.ConsumeSchedule();
					}
				}
				SyncWeather();
				_currentSyncWait = 5f;
				_needSync = false;
			}
			_currentLerpWait = 0.05f;
			_finishedLoading = true;
		}

		private void SetMainMenuWeather()
		{
			_currentWeather.Rain.Value = 0.45f;
			_currentWeather.Thunder.Value = 0.1f;
			_currentWeather.Skybox.Value = "Storm";
			_currentWeather.FogDensity.Value = 0.01f;
			_currentWeather.Daylight.Value = new Color(0.1f, 0.1f, 0.1f);
			_currentWeather.AmbientLight.Value = new Color(0.1f, 0.1f, 0.1f);
		}

		private void CreateScheduleRunners(string schedule)
		{
			WeatherScheduleRunner weatherScheduleRunner = new WeatherScheduleRunner(this);
			WeatherSchedule weatherSchedule = new WeatherSchedule(schedule);
			foreach (WeatherEvent @event in weatherSchedule.Events)
			{
				if (@event.Action == WeatherAction.BeginSchedule)
				{
					weatherScheduleRunner = new WeatherScheduleRunner(this);
					_scheduleRunners.Add(weatherScheduleRunner);
					_currentScheduleWait.Add(weatherScheduleRunner, 0f);
				}
				weatherScheduleRunner.Schedule.Events.Add(@event);
			}
		}

		private void CreateEffects()
		{
			_effects.Add(WeatherEffect.Rain, AssetBundleManager.InstantiateAsset<GameObject>("RainEffect").AddComponent<RainWeatherEffect>());
			_effects.Add(WeatherEffect.Snow, AssetBundleManager.InstantiateAsset<GameObject>("SnowEffect").AddComponent<SnowWeatherEffect>());
			_effects.Add(WeatherEffect.Wind, AssetBundleManager.InstantiateAsset<GameObject>("WindEffect").AddComponent<WindWeatherEffect>());
			_effects.Add(WeatherEffect.Thunder, AssetBundleManager.InstantiateAsset<GameObject>("ThunderEffect").AddComponent<ThunderWeatherEffect>());
			Transform parent = Camera.main.transform;
			foreach (BaseWeatherEffect value in _effects.Values)
			{
				value.Setup(parent);
				value.Randomize();
				value.Disable();
			}
			CreateFlashlight();
		}

		private void CreateFlashlight()
		{
			_effects.Add(WeatherEffect.Flashlight, AssetBundleManager.InstantiateAsset<GameObject>("FlashlightEffect").AddComponent<FlashlightWeatherEffect>());
			_effects[WeatherEffect.Flashlight].Setup(null);
			_effects[WeatherEffect.Flashlight].Disable();
			if (IN_GAME_MAIN_CAMERA.Instance != null)
			{
				TakeFlashlight(IN_GAME_MAIN_CAMERA.Instance.transform);
			}
		}

		private void FixedUpdate()
		{
			if (!_finishedLoading)
			{
				return;
			}
			_currentTime += Time.fixedDeltaTime;
			if (_targetWeatherStartTimes.Count > 0)
			{
				_currentLerpWait -= Time.fixedDeltaTime;
				if (_currentLerpWait <= 0f)
				{
					LerpCurrentWeatherToTarget();
					ApplyCurrentWeather(false, false);
					_currentLerpWait = 0.05f;
				}
			}
			if ((IN_GAME_MAIN_CAMERA.gametype != 0 && !PhotonNetwork.isMasterClient) || !_currentWeather.UseSchedule.Value)
			{
				return;
			}
			foreach (WeatherScheduleRunner item in new List<WeatherScheduleRunner>(_currentScheduleWait.Keys))
			{
				_currentScheduleWait[item] -= Time.fixedDeltaTime;
				if (_currentScheduleWait[item] <= 0f)
				{
					item.ConsumeSchedule();
				}
			}
			_currentSyncWait -= Time.fixedDeltaTime;
			if (_currentSyncWait <= 0f && _needSync)
			{
				LerpCurrentWeatherToTarget();
				SyncWeather();
				_needSync = false;
				_currentSyncWait = 5f;
			}
		}

		private void SyncWeather()
		{
			ApplyCurrentWeather(false, true);
			if (PhotonNetwork.isMasterClient && IN_GAME_MAIN_CAMERA.gametype != 0)
			{
				CustomRPCManager.PhotonView.RPC("SetWeatherRPC", PhotonTargets.Others, _currentWeather.SerializeToJsonString(), _startWeather.SerializeToJsonString(), _targetWeather.SerializeToJsonString(), _targetWeatherStartTimes, _targetWeatherEndTimes, _currentTime);
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			WindWeatherEffect.WindEnabled = false;
			foreach (List<LightningParticle> item in ThunderWeatherEffect.LightningPool)
			{
				foreach (LightningParticle item2 in item)
				{
					item2.Disable();
				}
			}
			if (Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot")
			{
				_finishedLoading = false;
				StartCoroutine(RestartWeather());
			}
		}

		private void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			if (PhotonNetwork.isMasterClient)
			{
				CustomRPCManager.PhotonView.RPC("SetWeatherRPC", player, _currentWeather.SerializeToJsonString(), _startWeather.SerializeToJsonString(), _targetWeather.SerializeToJsonString(), _targetWeatherStartTimes, _targetWeatherEndTimes, _currentTime);
			}
		}

		private void LerpCurrentWeatherToTarget()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, float> targetWeatherEndTime in _targetWeatherEndTimes)
			{
				float num;
				if (targetWeatherEndTime.Value <= _currentTime)
				{
					list.Add(targetWeatherEndTime.Key);
					num = 1f;
				}
				else
				{
					float num2 = _targetWeatherStartTimes[targetWeatherEndTime.Key];
					float value = targetWeatherEndTime.Value;
					num = (_currentTime - num2) / Mathf.Max(value - num2, 1f);
					num = Mathf.Clamp(num, 0f, 1f);
				}
				string key = ((WeatherEffect)targetWeatherEndTime.Key).ToString();
				BaseSetting baseSetting = (BaseSetting)_startWeather.Settings[key];
				BaseSetting baseSetting2 = (BaseSetting)_currentWeather.Settings[key];
				BaseSetting baseSetting3 = (BaseSetting)_targetWeather.Settings[key];
				switch ((WeatherEffect)targetWeatherEndTime.Key)
				{
				case WeatherEffect.Daylight:
				case WeatherEffect.AmbientLight:
				case WeatherEffect.SkyboxColor:
				case WeatherEffect.Flashlight:
				case WeatherEffect.FogColor:
					((ColorSetting)baseSetting2).Value = Color.Lerp(((ColorSetting)baseSetting).Value, ((ColorSetting)baseSetting3).Value, num);
					break;
				case WeatherEffect.FogDensity:
				case WeatherEffect.Rain:
				case WeatherEffect.Thunder:
				case WeatherEffect.Snow:
				case WeatherEffect.Wind:
					((FloatSetting)baseSetting2).Value = Mathf.Lerp(((FloatSetting)baseSetting).Value, ((FloatSetting)baseSetting3).Value, num);
					break;
				case WeatherEffect.Skybox:
				{
					Material blendedSkybox = GetBlendedSkybox(_currentWeather.Skybox.Value, _targetWeather.Skybox.Value);
					if (blendedSkybox != null)
					{
						if (num >= 1f)
						{
							((StringSetting)baseSetting2).Value = ((StringSetting)baseSetting3).Value;
						}
						SetSkyboxBlend(blendedSkybox, num);
					}
					break;
				}
				}
				_needApply.Add((WeatherEffect)targetWeatherEndTime.Key);
			}
			foreach (int item in list)
			{
				_targetWeatherStartTimes.Remove(item);
				_targetWeatherEndTimes.Remove(item);
			}
		}

		private void ApplyCurrentWeather(bool firstStart, bool applyAll)
		{
			if (applyAll)
			{
				_needApply = RCextensions.EnumToList<WeatherEffect>();
			}
			WeatherEffectLevel value = (WeatherEffectLevel)SettingsManager.GraphicsSettings.WeatherEffects.Value;
			foreach (WeatherEffect item in _needApply)
			{
				if (!firstStart && value == WeatherEffectLevel.Low && !LowEffects.Contains(item))
				{
					continue;
				}
				switch (item)
				{
				case WeatherEffect.Daylight:
					_mainLight.color = _currentWeather.Daylight.Value;
					break;
				case WeatherEffect.AmbientLight:
					RenderSettings.ambientLight = _currentWeather.AmbientLight.Value;
					break;
				case WeatherEffect.FogColor:
					RenderSettings.fogColor = _currentWeather.FogColor.Value;
					break;
				case WeatherEffect.FogDensity:
					if (_currentWeather.FogDensity.Value > 0f)
					{
						RenderSettings.fog = true;
						RenderSettings.fogMode = FogMode.Exponential;
						RenderSettings.fogDensity = _currentWeather.FogDensity.Value * 0.05f;
					}
					else
					{
						RenderSettings.fog = false;
					}
					break;
				case WeatherEffect.Flashlight:
					((FlashlightWeatherEffect)_effects[WeatherEffect.Flashlight]).SetColor(_currentWeather.Flashlight.Value);
					if (_currentWeather.Flashlight.Value.a > 0f && _currentWeather.Flashlight.Value != Color.black)
					{
						if (!_effects[WeatherEffect.Flashlight].gameObject.activeSelf)
						{
							_effects[WeatherEffect.Flashlight].Enable();
						}
					}
					else
					{
						_effects[WeatherEffect.Flashlight].Disable();
					}
					break;
				case WeatherEffect.Skybox:
					StartCoroutine(WaitAndApplySkybox());
					break;
				case WeatherEffect.SkyboxColor:
				{
					Material blendedSkybox = GetBlendedSkybox(_currentWeather.Skybox.Value, _targetWeather.Skybox.Value);
					if (blendedSkybox != null)
					{
						blendedSkybox.SetColor("_Tint", _currentWeather.SkyboxColor.Value);
					}
					break;
				}
				case WeatherEffect.Rain:
				case WeatherEffect.Thunder:
				case WeatherEffect.Snow:
				case WeatherEffect.Wind:
				{
					float value2 = ((FloatSetting)_currentWeather.Settings[item.ToString()]).Value;
					_effects[item].SetLevel(value2);
					if (value2 > 0f)
					{
						if (!_effects[item].gameObject.activeSelf)
						{
							_effects[item].Randomize();
							_effects[item].Enable();
						}
					}
					else
					{
						_effects[item].Disable(true);
					}
					break;
				}
				}
			}
			_needApply.Clear();
		}

		private IEnumerator WaitAndApplySkybox()
		{
			yield return new WaitForEndOfFrame();
			Material blendedSkybox = GetBlendedSkybox(_currentWeather.Skybox.Value, _targetWeather.Skybox.Value);
			if (blendedSkybox != null && _skybox.material != blendedSkybox && SkyboxCustomSkinLoader.SkyboxMaterial == null)
			{
				blendedSkybox.SetColor("_Tint", _currentWeather.SkyboxColor.Value);
				_skybox.material = blendedSkybox;
				if (IN_GAME_MAIN_CAMERA.Instance != null)
				{
					IN_GAME_MAIN_CAMERA.Instance.UpdateSnapshotSkybox();
				}
			}
		}

		private Material GetBlendedSkybox(string skybox1, string skybox2)
		{
			if (SkyboxBlendedMaterials.ContainsKey(skybox1) && SkyboxBlendedMaterials[skybox1].ContainsKey(skybox2))
			{
				return SkyboxBlendedMaterials[skybox1][skybox2];
			}
			return null;
		}

		public static void OnSetWeatherRPC(string currentWeatherJson, string startWeatherJson, string targetWeatherJson, Dictionary<int, float> targetWeatherStartTimes, Dictionary<int, float> targetWeatherEndTimes, float currentTime, PhotonMessageInfo info)
		{
			if ((info == null || info.sender == PhotonNetwork.masterClient) && SettingsManager.GraphicsSettings.WeatherEffects.Value != 0)
			{
				_instance.StartCoroutine(_instance.WaitAndFinishOnSetWeather(currentWeatherJson, startWeatherJson, targetWeatherJson, targetWeatherStartTimes, targetWeatherEndTimes, currentTime));
			}
		}

		private IEnumerator WaitAndFinishOnSetWeather(string currentWeatherJson, string startWeatherJson, string targetWeatherJson, Dictionary<int, float> targetWeatherStartTimes, Dictionary<int, float> targetWeatherEndTimes, float currentTime)
		{
			while (!_finishedLoading)
			{
				yield return null;
			}
			_currentWeather.DeserializeFromJsonString(currentWeatherJson);
			_startWeather.DeserializeFromJsonString(startWeatherJson);
			_targetWeather.DeserializeFromJsonString(targetWeatherJson);
			_targetWeatherStartTimes = targetWeatherStartTimes;
			_targetWeatherEndTimes = targetWeatherEndTimes;
			_currentTime = currentTime;
			LerpCurrentWeatherToTarget();
			ApplyCurrentWeather(false, true);
		}
	}
}
