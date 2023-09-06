#if UNITY_EDITOR

using BehaviourGraph;
using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BehaviourTreeLoader : MonoBehaviour
{
    [HideInInspector, NonSerialized] public bool graphOpen;

    BehaviourTree behaviourTree
    {
        get
        {
            return GetComponent<BehaviourTree>();
        }
    }

    void OnEnable()
    {
        if (graphOpen)
            LoadGraph(behaviourTree);
    }

    public static BehaviourGraphEditorWindow LoadGraph(BehaviourTree behaviourTree)
    {
        BehaviourGraphEditorWindow behaviourGraphEditorWindow = EditorWindow.GetWindow(typeof(BehaviourGraphEditorWindow)) as BehaviourGraphEditorWindow;

        behaviourGraphEditorWindow.behaviourTree = behaviourTree;

        return behaviourGraphEditorWindow;
    }
}

#endif
