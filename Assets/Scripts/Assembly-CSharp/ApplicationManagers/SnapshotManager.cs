using System;
using System.IO;
using UnityEngine;
using Utility;

namespace ApplicationManagers
{
	internal class SnapshotManager : MonoBehaviour
	{
		private static SnapshotManager _instance;

		public static readonly string SnapshotPath = Application.dataPath + "/UserData/Snapshots";

		private static readonly string SnapshotTempPath = Application.dataPath + "/UserData/Snapshots/Temp";

		private static readonly string SnapshotFilePrefix = "Snapshot";

		private static readonly int MaxSnapshots = 500;

		private static int _currentSnapshotSaveId = 0;

		private static int _maxSnapshotSaveId = 0;

		private static int[] _damages = new int[MaxSnapshots];

		public static void Init()
		{
			_instance = SingletonFactory.CreateSingleton(_instance);
			ClearTemp();
		}

		private void OnApplicationQuit()
		{
			ClearTemp();
		}

		private static void ClearTemp()
		{
			if (Directory.Exists(SnapshotTempPath))
			{
				try
				{
					Directory.Delete(SnapshotTempPath, true);
				}
				catch (Exception ex)
				{
					Debug.Log(string.Format("Error deleting snapshot temp folder: {0}", ex.Message));
				}
			}
		}

		private static string GetFileName(int snapshotId)
		{
			return SnapshotFilePrefix + snapshotId;
		}

		public static void AddSnapshot(Texture2D texture, int damage)
		{
			try
			{
				if (!Directory.Exists(SnapshotTempPath))
				{
					Directory.CreateDirectory(SnapshotTempPath);
				}
				File.WriteAllBytes(SnapshotTempPath + "/" + GetFileName(_currentSnapshotSaveId), SerializeSnapshot(texture));
				_damages[_currentSnapshotSaveId] = damage;
				_currentSnapshotSaveId++;
				_maxSnapshotSaveId++;
				_maxSnapshotSaveId = Math.Min(_maxSnapshotSaveId, MaxSnapshots);
				if (_currentSnapshotSaveId >= MaxSnapshots)
				{
					_currentSnapshotSaveId = 0;
				}
			}
			catch (Exception ex)
			{
				Debug.Log(string.Format("Exception while adding snapshot: {0}", ex.Message));
			}
		}

		private static byte[] SerializeSnapshot(Texture2D texture)
		{
			Color32[] pixels = texture.GetPixels32();
			byte[] array = new byte[pixels.Length * 3 + 8];
			int num = 0;
			byte[] bytes = BitConverter.GetBytes(texture.width);
			foreach (byte b in bytes)
			{
				array[num] = b;
				num++;
			}
			byte[] bytes2 = BitConverter.GetBytes(texture.height);
			foreach (byte b2 in bytes2)
			{
				array[num] = b2;
				num++;
			}
			Color32[] array2 = pixels;
			for (int k = 0; k < array2.Length; k++)
			{
				Color32 color = array2[k];
				array[num] = color.r;
				array[num + 1] = color.g;
				array[num + 2] = color.b;
				num += 3;
			}
			return array;
		}

		private static Texture2D DeserializeSnapshot(byte[] bytes)
		{
			int width = BitConverter.ToInt32(bytes, 0);
			int height = BitConverter.ToInt32(bytes, 4);
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
			int num = 8;
			Color32[] array = new Color32[(bytes.Length - 8) / 3];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Color32(bytes[num], bytes[num + 1], bytes[num + 2], byte.MaxValue);
				num += 3;
			}
			texture2D.SetPixels32(array);
			texture2D.Apply();
			return texture2D;
		}

		public static void SaveSnapshotFinish(Texture2D texture, string fileName)
		{
			if (!Directory.Exists(SnapshotPath))
			{
				Directory.CreateDirectory(SnapshotPath);
			}
			File.WriteAllBytes(SnapshotPath + "/" + fileName, texture.EncodeToPNG());
		}

		public static int GetDamage(int index)
		{
			if (index >= _maxSnapshotSaveId)
			{
				return 0;
			}
			return _damages[index];
		}

		public static Texture2D GetSnapshot(int index)
		{
			if (index >= _maxSnapshotSaveId)
			{
				return null;
			}
			string path = SnapshotTempPath + "/" + GetFileName(index);
			if (File.Exists(path))
			{
				Texture2D result = DeserializeSnapshot(File.ReadAllBytes(path));
				FengGameManagerMKII.instance.unloadAssets();
				return result;
			}
			return null;
		}

		public static int GetLength()
		{
			return _maxSnapshotSaveId;
		}
	}
}
