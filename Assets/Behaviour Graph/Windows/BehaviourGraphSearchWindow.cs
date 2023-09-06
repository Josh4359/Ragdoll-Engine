#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourGraph
{
    public class BehaviourGraphSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        BehaviourGraphView behaviourGraphView;

        BehaviourGraphEditorWindow behaviourGraphEditorWindow;

        public void Initialize(BehaviourGraphView thisBehaviourGraphView, BehaviourGraphEditorWindow thisBehaviourGraphEditorWindow)
        {
            behaviourGraphView = thisBehaviourGraphView;

            behaviourGraphEditorWindow = thisBehaviourGraphEditorWindow;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext searchWindowContext)
        {
            return new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Add Behaviour")),
                new SearchTreeEntry(new GUIContent("Check"))
                {
                    level = 1,
                    userData = "CheckBehaviourNode"
                },
                new SearchTreeEntry(new GUIContent("Action"))
                {
                    level = 1,
                    userData = "ActionBehaviourNode"
                }
            };
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext searchWindowContext)
        {
            switch(searchTreeEntry.userData)
            {
                case "CheckBehaviourNode":
                    behaviourGraphView.CreateNode(new CheckBehaviourNode(), searchWindowContext.screenMousePosition - behaviourGraphEditorWindow.position.position, true);

                    return true;
                case "ActionBehaviourNode":
                    behaviourGraphView.CreateNode(new ActionBehaviourNode(), searchWindowContext.screenMousePosition - behaviourGraphEditorWindow.position.position, true);

                    return true;
                default:
                    return false;
            }
        }
    }
}

#endif
