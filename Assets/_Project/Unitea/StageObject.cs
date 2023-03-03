using System;
using UnityEngine;
using System.Collections.Generic;

public interface IStageObject
{

}

public interface IStageSetupObject
{
    public void Setup(string data);
    public string ExportData();
}

[Serializable]
public class StageObjectData
{
    public string keyName;
    public int instanceID;
    public Vector3 localPosition;
    public Vector3 localEulerAngle;
    public Vector3 localScale;
    [TextArea(1, 4)]
    public string data;

    public StageObjectData ShallowCopy()
    {
        return this.MemberwiseClone() as StageObjectData;
    }

    public StageObjectData DeepCopy()
    {
        StageObjectData cloneObj = this.ShallowCopy();
        return cloneObj;
    }
}

[SelectionBaseFixed]
public class StageObject : MonoBehaviour, IStageObject
{
    public string keyName;
    [ReadOnly] [SerializeField] private int instanceId = -1;

    [Tooltip("Note, for commenting what this object is for")]
    [SerializeField] [TextArea] string note;

    public int InstanceId => instanceId;
    public void Setup(StageObjectData data)
    {
        transform.localPosition = data.localPosition;
        transform.localEulerAngles = data.localEulerAngle;
        transform.localScale = data.localScale;
        instanceId = data.instanceID;

        if (TryGetComponent<IStageSetupObject>(out IStageSetupObject stageSetup))
            stageSetup.Setup(data.data);
    }
}
