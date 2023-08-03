using UnityEngine;
using System.Collections;
using Utility;
using System.Collections.Generic;

namespace ApplicationManagers
{
    public class AssetBundleManager : MonoBehaviour
    {
        public static AssetBundle MainAssetBundle;
        public static AssetBundle RRMAssetBundle; // Secondary Assetbundle for UI

        public static AssetBundleStatus Status = AssetBundleStatus.Loading;
        public static bool CloseFailureBox = false;
        static AssetBundleManager _instance;
        static Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        // consts
        static readonly string RootDataPath = Application.dataPath;

        static readonly string RRMLocalAssetBundlePath = "file:///" + RootDataPath + "/RRMAssets.unity3d"; // RRMBundle file path
        static readonly string RRMBackupAssetBundleURL = AutoUpdateManager.PlatformUpdateURL + "/RRMAssets.unity3d";// RRMBundle file path backup


        static readonly string LocalAssetBundlePath = "file:///" + RootDataPath + "/RCAssets.unity3d";
        static readonly string BackupAssetBundleURL = AutoUpdateManager.PlatformUpdateURL + "/RCAssets.unity3d";

        public static void Init()
        {
            _instance = SingletonFactory.CreateSingleton(_instance);
            LoadAssetBundle();
        }

        public static void LoadAssetBundle()
        {
            _instance.StartCoroutine(_instance.LoadAssetBundleCoroutine());
            _instance.StartCoroutine(_instance.LoadAssetBundleCoroutine2());
        }

        public static Object LoadAsset(string name, bool cached = false)
        {
            if (cached)
            {
                if (!_cache.ContainsKey(name))
                    _cache.Add(name, MainAssetBundle.Load(name));
                return _cache[name];
            }
            return MainAssetBundle.Load(name);
        }
        public static T InstantiateAsset<T>(string name) where T : Object
        {
            return (T)Instantiate(MainAssetBundle.Load(name));
        }


        public static T InstantiateAsset2<T>(string name) where T : Object
        {
            return (T)Instantiate(RRMAssetBundle.Load(name));
        }

        public static T InstantiateAsset2<T>(string name, Vector3 position, Quaternion rotation) where T : Object
        {
            return (T)Instantiate(RRMAssetBundle.Load(name));
        }
        public static T InstantiateAsset<T>(string name, Vector3 position, Quaternion rotation) where T : Object
        {
            return (T)Instantiate(MainAssetBundle.Load(name), position, rotation);
        }

        IEnumerator LoadAssetBundleCoroutine()
        {
            Status = AssetBundleStatus.Loading;
            while (AutoUpdateManager.Status == AutoUpdateStatus.Updating || !Caching.ready)
                yield return null;
            // try loading local asset bundle
            using (WWW wwwLocal = new WWW(LocalAssetBundlePath))
            {
                yield return wwwLocal;
                if (wwwLocal.error != null)
                {
                    // try loading server asset bundle
                    Debug.Log("Failed to load local asset bundle, trying backup URL at " + BackupAssetBundleURL + ": " + wwwLocal.error);
                    using (WWW wwwBackup = WWW.LoadFromCacheOrDownload(BackupAssetBundleURL, ApplicationConfig.AssetBundleVersion))
                    {
                        yield return wwwBackup;
                        if (wwwBackup.error != null)
                        {
                            Debug.Log("The backup asset bundle failed too: " + wwwBackup.error);
                            Status = AssetBundleStatus.Failed;
                            yield break;
                        }
                        else
                            OnAssetBundleLoaded(wwwBackup);
                    }
                }
                else
                    OnAssetBundleLoaded(wwwLocal);
            }
        }


        IEnumerator LoadAssetBundleCoroutine2()
        {
            Status = AssetBundleStatus.Loading;
            while (AutoUpdateManager.Status == AutoUpdateStatus.Updating || !Caching.ready)
                yield return null;
            // try loading local asset bundle
            using (WWW wwwLocal = new WWW(RRMLocalAssetBundlePath))
            {
                yield return wwwLocal;
                if (wwwLocal.error != null)
                {
                    logger.addLINE("Qualcosa è esploso");
                }
                else
                    OnAssetBundleLoaded2(wwwLocal);
            }
        }

        private void OnAssetBundleLoaded(WWW www)
        {
            FengGameManagerMKII.RCassets = www.assetBundle;
            FengGameManagerMKII.isAssetLoaded = true;
            MainAssetBundle = FengGameManagerMKII.RCassets;
            MainApplicationManager.FinishLoadAssets();
            Status = AssetBundleStatus.Ready;
        }

        private void OnAssetBundleLoaded2(WWW www)
        {
            FengGameManagerMKII.RRMassets = www.assetBundle;
            RRMAssetBundle = FengGameManagerMKII.RRMassets;
        }
    }
    public enum AssetBundleStatus
    {
        Loading,
        Ready,
        Failed
    }
}