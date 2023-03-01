using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QQ.StageBase
{
    public class StageData : StageDataBase
    {
        public int GetStageId()
        {
            if (int.TryParse(stageID, out int id))
                return id;
            else
                throw new System.Exception($"stageId {stageID} not correct format");
        }
    }
}

