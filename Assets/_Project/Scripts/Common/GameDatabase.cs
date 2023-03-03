using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.U2D;

namespace Unitea.Common
{
    public static class GameDatabase
    {
        public static StageData GetStageData(string id)
        {
            return Resources.Load<StageData>($"{Const.STAGE_DATA_PATH}/stage_{id}");
        }

        public static async UniTask<T> LoadResourceAsyn<T>(string path) where T : UnityEngine.Object
        {
            var asset = await Resources.LoadAsync<T>(path);
            if (asset == null)
            {
                Debug.LogException(new System.NullReferenceException($"Resources not found at path: {path}"));
            }
            return asset as T;
        }
    }
}
