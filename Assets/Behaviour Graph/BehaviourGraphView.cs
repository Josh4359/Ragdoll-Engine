#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph
{
    public class BehaviourGraphView : GraphView
    {
        public InputBehaviourNode updateBehaviourNode;

        public InputBehaviourNode lateUpdateBehaviourNode;

        public InputBehaviourNode fixedUpdateBehaviourNode;

        public InputBehaviourNode lateFixedUpdateBehaviourNode;

        BehaviourGraphEditorWindow behaviourGraphEditorWindow;

        BehaviourGraphSearchWindow behaviourGraphSearchWindow;

        public BehaviourGraphView(BehaviourGraphEditorWindow thisBehaviourGraphEditorWindow)
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(CreateNodeContextualMenu<CheckBehaviourNode>("Add Check"));

            this.AddManipulator(CreateNodeContextualMenu<ActionBehaviourNode>("Add Action"));

            this.AddManipulator(new ContentDragger());

            this.AddManipulator(new SelectionDragger());

            this.AddManipulator(new RectangleSelector());

            if (!behaviourGraphEditorWindow)
                behaviourGraphEditorWindow = thisBehaviourGraphEditorWindow;

            if (!behaviourGraphSearchWindow)
            {
                behaviourGraphSearchWindow = ScriptableObject.CreateInstance<BehaviourGraphSearchWindow>();

                behaviourGraphSearchWindow.Initialize(this, behaviourGraphEditorWindow);
            }

            nodeCreationRequest = nodeCreationContext => SearchWindow.Open(new SearchWindowContext(nodeCreationContext.screenMousePosition), behaviourGraphSearchWindow);

            GridBackground gridBackground = new GridBackground();

            Insert(0, gridBackground);

            gridBackground.StretchToParentSize();

            CreateInputNodes();
        }

        IManipulator CreateNodeContextualMenu<T>(string title) where T : BehaviourNode, new()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(contextualMenuPopulateEvent => contextualMenuPopulateEvent.menu.AppendAction(title, dropdownMenuAction => CreateNode(new T(), dropdownMenuAction.eventInfo.localMousePosition, true)));

            return contextualMenuManipulator;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateInputNodes()
        {
            if (!nodes.Contains(updateBehaviourNode))
            {
                foreach(Node thisNode in nodes)
                {
                    if (thisNode is InputBehaviourNode && thisNode.title == "Update")
                    {
                        updateBehaviourNode = thisNode as InputBehaviourNode;

                        goto LateUpdate;
                    }
                }

                if (nodes.Contains(updateBehaviourNode)) return;

                updateBehaviourNode = new InputBehaviourNode();

                updateBehaviourNode.title = "Update";

                CreateNode(updateBehaviourNode, new Vector2(100, 100));
            }

            LateUpdate:

            if (!nodes.Contains(lateUpdateBehaviourNode))
            {
                foreach (Node thisNode in nodes)
                {
                    if (thisNode is InputBehaviourNode && thisNode.title == "Late Update")
                    {
                        lateUpdateBehaviourNode = thisNode as InputBehaviourNode;

                        goto FixedUpdate;
                    }
                }

                if (nodes.Contains(lateUpdateBehaviourNode)) return;

                lateUpdateBehaviourNode = new InputBehaviourNode();

                lateUpdateBehaviourNode.title = "Late Update";

                CreateNode(lateUpdateBehaviourNode, new Vector2(100, 500));
            }

        FixedUpdate:

            if (!nodes.Contains(fixedUpdateBehaviourNode))
            {
                foreach (Node thisNode in nodes)
                {
                    if (thisNode is InputBehaviourNode && thisNode.title == "Fixed Update")
                    {
                        fixedUpdateBehaviourNode = thisNode as InputBehaviourNode;

                        goto LateFixedUpdate;
                    }
                }

                fixedUpdateBehaviourNode = new InputBehaviourNode();

                fixedUpdateBehaviourNode.title = "Fixed Update";

                CreateNode(fixedUpdateBehaviourNode, new Vector2(100, 900));
            }

            LateFixedUpdate:

            if (!nodes.Contains(lateFixedUpdateBehaviourNode))
            {
                foreach (Node thisNode in nodes)
                {
                    if (thisNode is InputBehaviourNode && thisNode.title == "Late Fixed Update")
                    {
                        lateFixedUpdateBehaviourNode = thisNode as InputBehaviourNode;

                        return;
                    }
                }

                lateFixedUpdateBehaviourNode = new InputBehaviourNode();

                lateFixedUpdateBehaviourNode.title = "Late Fixed Update";

                CreateNode(lateFixedUpdateBehaviourNode, new Vector2(100, 1300));
            }
        }

        public void CreateNode<T>(T behaviourNode, Vector2 position, bool localSpace = false) where T : BehaviourNode
        {
            behaviourNode.Initialize();

            behaviourNode.SetPosition(new Rect(localSpace ? contentViewContainer.WorldToLocal(position) : position, Vector2.zero));
            
            AddElement(behaviourNode);
        }
    }
}

#endif
