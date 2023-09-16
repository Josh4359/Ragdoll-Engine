using RagdollEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Rings
{
    static Dictionary<PlayerBehaviourTree, RingCounter> rings = new();

    public class RingCounter
    {
        public class RingsChangedEventArgs : EventArgs
        {
            public int oldValue;

            public int difference;
        }

        public EventHandler ringsChangedEvent;

        public int rings;
    }

    public static RingCounter GetRingCounter(PlayerBehaviourTree playerBehaviourTree)
    {
        if (!rings.ContainsKey(playerBehaviourTree))
            rings.Add(playerBehaviourTree, new());

        return rings[playerBehaviourTree];
    }

    public static bool HasRings(PlayerBehaviourTree playerBehaviourTree)
    {
        return rings.ContainsKey(playerBehaviourTree);
    }

    public static int GetRings(PlayerBehaviourTree playerBehaviourTree)
    {
        return GetRingCounter(playerBehaviourTree).rings;
    }

    public static void SetRings(PlayerBehaviourTree playerBehaviourTree, int value)
    {
        int oldValue = 0;

        value = Mathf.Clamp(value, 0, 999);

        if (rings.ContainsKey(playerBehaviourTree))
        {
            oldValue = rings[playerBehaviourTree].rings;

            rings[playerBehaviourTree].rings = value;
        }
        else
            rings.Add(playerBehaviourTree, new()
            {
                rings = value
            });

        rings[playerBehaviourTree].ringsChangedEvent?.Invoke(rings[playerBehaviourTree], new RingCounter.RingsChangedEventArgs
        {
            oldValue = oldValue,
            difference = value - oldValue
        });
    }

    public static void AddRings(PlayerBehaviourTree playerBehaviourTree, int amount)
    {
        SetRings(playerBehaviourTree, GetRings(playerBehaviourTree) + amount);
    }
}
