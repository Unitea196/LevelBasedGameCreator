using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unitea.Common
{
    public class StageSetup : MonoBehaviour
    {
        public delegate StageData CreateEmptyStageDelegate(string stageID);

        public static StageSetup Instance;
        public static Action<StageData> onSave;
        public static CreateEmptyStageDelegate onCreateEmptyStage;
        [SerializeField] bool destroyExistObject = true;
        [SerializeField] bool loadStage = true;

        StageData data;
        public string stageID;

        /// <summary>
        /// all prefab that object spawn via script
        /// </summary>
        Dictionary<string, StageObject> allPrefabs = new Dictionary<string, StageObject>();
        public Dictionary<string, StageObject> AllPrefabs { get => allPrefabs; set => allPrefabs = value; }
        public StageData Data => data;

        public Dictionary<string, List<StageObject>> allObjectInScene = new Dictionary<string, List<StageObject>>();

        private void Awake()
        {
            Instance = this;
        }
        public async UniTask LoadStageAsync(string stageID)
        {
            if (!loadStage)
                return;

#if UNITY_EDITOR
            if (destroyExistObject)
                DestroyStageObjectBeforeLoad();
#endif

            StageData origin = GameDatabase.GetStageData(stageID);
            if (origin == null)
            {
                if (onCreateEmptyStage != null)
                {
                    origin = onCreateEmptyStage.Invoke(stageID);
                }
                else
                {
                    Debug.LogError("Delegate is not registered. Load fail");
                    return;
                }
            }

            data = ScriptableObject.CreateInstance<StageData>();
            data.CopyFrom(origin);

            this.stageID = data.StageID;

            List<UniTask<StageObject>> loadStageObjectAsync = new List<UniTask<StageObject>>();
            foreach (var i in data.stageObjects)
            {
                if (!allPrefabs.ContainsKey(i.keyName))
                {
                    allPrefabs.Add(i.keyName, null);
                    loadStageObjectAsync.Add(GameDatabase.LoadResourceAsyn<StageObject>(StageObjectManager.GetPath(i.keyName)));
                }
            }

            StageObject[] prefabs = await UniTask.WhenAll<StageObject>(loadStageObjectAsync);

            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i] == null)
                {
                    Debug.LogException(new NullReferenceException($"Object {i} is null, key: {data.stageObjects[i].keyName}. Object load may have failed. Check if its resources still exist."));
                    continue;
                }
                allPrefabs[prefabs[i].keyName] = prefabs[i];
            }

            foreach (var i in data.stageObjects)
            {
                if (allPrefabs[i.keyName] == null) { Debug.LogException(new System.NullReferenceException($"Prefab {i.keyName} is null")); }
                StageObject obj = Instantiate<StageObject>(allPrefabs[i.keyName]);
                obj.Setup(i);

                if (allObjectInScene.ContainsKey(obj.keyName))
                    allObjectInScene[obj.keyName].Add(obj);
                else
                    allObjectInScene.Add(obj.keyName, new List<StageObject>() { obj });
            }
        }

        public async UniTask LoadStageAsync(int stageID)
        {
            await LoadStageAsync(stageID.ToString());
        }

        public void Save()
        {
            if (data == null)
                data = ScriptableObject.CreateInstance<StageData>();
            else
                data.ResetData();

            data.StageID = stageID;

            GameObject[] allObj = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var obj in allObj)
            {
                if (obj.TryGetComponent<StageObject>(out StageObject stageObj))
                {
                    data.stageObjects.Add(ConvertStageObjectToObjectData(stageObj));
                }
            }

            onSave?.Invoke(data);
        }

        StageObjectData ConvertStageObjectToObjectData(StageObject obj)
        {
            StageObjectData data = new StageObjectData();

            data.keyName = obj.keyName;
            data.instanceID = obj.gameObject.GetInstanceID();
            data.localPosition = obj.transform.localPosition;
            data.localEulerAngle = obj.transform.localEulerAngles;
            data.localScale = obj.transform.localScale;

            if (obj.TryGetComponent<IStageSetupObject>(out IStageSetupObject setupObject))
            {
                data.data = setupObject.ExportData();
            }

            return data;
        }

        void DestroyStageObjectBeforeLoad()
        {
            StageObject[] allObj = GameObject.FindObjectsOfType<StageObject>();
            foreach (var i in allObj)
            {
                Destroy(i.gameObject);
            }
        }

        public void EditStage()
        {
            //Manager.Load(StageMakerController.STAGEMAKER_SCENE_NAME, stageID);
        }

        private void OnDisable()
        {
            Instance = null;
        }
    }
}
