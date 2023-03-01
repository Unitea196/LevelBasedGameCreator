using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameEvent
{
    public string name;
    public static GameEvent e;
    public static void Trigger(string name)
    {
        e.name = name;
        MMEventManager.TriggerEvent<GameEvent>(e);
    }
}
