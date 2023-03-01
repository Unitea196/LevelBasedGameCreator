using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QQ.StoryBase
{
    [CreateAssetMenu(fileName = "NewStageData", menuName = "Stage Data")]
    public class StageData : StageDataBase
    {
        public string GetStoryId()
        {
            return stageID.Split(Const.storyIdAndStageIdSplitChar, System.StringSplitOptions.None)[0];
        }

        public string GetOrderInStory()
        {
            return stageID.Split(Const.storyIdAndStageIdSplitChar, System.StringSplitOptions.None)[1];
        }

        public static string GetStageId(int storyId, int orderInStory)
        {
            return $"{storyId}_{orderInStory}";
        }
    }
}
