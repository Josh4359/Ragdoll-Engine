using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Volume : MonoBehaviour
{
    public Trigger[] triggers
    {
        get
        {
            Trigger[] value = new Trigger[0];

            foreach (Trigger thisTrigger in GetComponentsInChildren<Trigger>())
                value = value.Append(thisTrigger).ToArray();

            return value;
        }
    }
}
