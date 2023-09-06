#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph
{
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        public BehaviourTree behaviourTree
        {
            get
            {
                return _behaviourTree;
            }
            set
            {
                if (_behaviourTree)
                    _behaviourTree.behaviourTreeLoader.graphOpen = false;

                _behaviourTree = value;

                _behaviourTree.behaviourTreeLoader.graphOpen = true;
            }
        }

        BehaviourTree _behaviourTree;

        BehaviourGraphView behaviourGraphView;

        void CreateGUI()
        {
            titleContent = new GUIContent("Behaviour Graph");

            CreateGraph();

            CreateToolbar();

            Load();
        }

        void OnDestroy()
        {
            if (!behaviourTree) return;

            behaviourTree.behaviourTreeLoader.graphOpen = false;
        }

        void CreateGraph()
        {
            behaviourGraphView = new BehaviourGraphView(this)
            {
                name = "Behaviour Graph"
            };

            behaviourGraphView.StretchToParentSize();

            rootVisualElement.Add(behaviourGraphView);
        }

        void CreateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            toolbar.Add(new Button(() => Save()) { text = "Save" });

            toolbar.Add(new Button(() => Load()) { text = "Load" });

            toolbar.Add(new Button(() => Clear()) { text = "Clear" });

            rootVisualElement.Add(toolbar);
        }

        public void Save()
        {
            if (!behaviourTree) return;

            behaviourTree.behaviours = new Behaviour[0];

            // Graph Data

            List<NodeGraphData> nodeData = new List<NodeGraphData>();

            List<EdgeGraphData> edgeData = new List<EdgeGraphData>();

            behaviourGraphView.nodes.Cast<BehaviourNode>().ToList().ForEach(thisBehaviourNode =>
            {
                nodeData.Add(thisBehaviourNode.CreateNodeGraphData(behaviourTree));

                thisBehaviourNode.outputContainer.Children().ToList().ForEach(thisElement =>
                {
                    if (!(thisElement is Port)) return;

                    Port thisPort = (Port)thisElement;

                    if (!thisPort.connected || thisPort.direction == Direction.Input) return;

                    EdgeGraphData newEdgeData = new EdgeGraphData();

                    newEdgeData.input = ((BehaviourNode)thisPort.connections.First().input.node).GetContainer(thisPort.connections.First().input).GUID;

                    newEdgeData.output = thisBehaviourNode.GetContainer(thisPort).GUID;

                    edgeData.Add(newEdgeData);
                });
            });

            behaviourTree.nodeGraphData = nodeData;

            behaviourTree.edgeGraphData = edgeData;

            // Tree Data

            behaviourTree.update = new InputNodeTreeData()
            {
                pass = behaviourGraphView.updateBehaviourNode.CreateNodeTreeData(behaviourTree)
            };

            behaviourTree.lateUpdate = new InputNodeTreeData()
            {
                pass = behaviourGraphView.lateUpdateBehaviourNode.CreateNodeTreeData(behaviourTree)
            };

            behaviourTree.fixedUpdate = new InputNodeTreeData()
            {
                pass = behaviourGraphView.fixedUpdateBehaviourNode.CreateNodeTreeData(behaviourTree)
            };

            behaviourTree.lateFixedUpdate = new InputNodeTreeData()
            {
                pass = behaviourGraphView.lateFixedUpdateBehaviourNode.CreateNodeTreeData(behaviourTree)
            };

            EditorUtility.SetDirty(behaviourTree);
        }

        public void Load()
        {
            behaviourGraphView.DeleteElements(behaviourGraphView.graphElements);

            if (!behaviourTree) return;

            behaviourTree.nodeGraphData.ForEach(thisNodeGraphData => behaviourGraphView.AddElement(thisNodeGraphData.CreateNode(behaviourTree)));

            behaviourTree.edgeGraphData.ForEach(thisEdgeData =>
            {
                BehaviourNode.BehaviourPort inputBehaviourPort = null;

                behaviourGraphView.nodes.Cast<BehaviourNode>().ToList().ForEach(thisBehaviourNode =>
                {
                    BehaviourNode.BehaviourPort thisBehaviourPort = thisBehaviourNode.GetContainer(thisEdgeData.input);

                    if (thisBehaviourPort != null)
                        inputBehaviourPort = thisBehaviourPort;
                });

                if (inputBehaviourPort == null) return;

                BehaviourNode.BehaviourPort outputBehaviourPort = null;

                behaviourGraphView.nodes.Cast<BehaviourNode>().ToList().ForEach(thisBehaviourNode =>
                {
                    BehaviourNode.BehaviourPort thisBehaviourPort = thisBehaviourNode.GetContainer(thisEdgeData.output);

                    if (thisBehaviourPort != null)
                        outputBehaviourPort = thisBehaviourPort;
                });

                if (outputBehaviourPort == null) return;

                behaviourGraphView.AddElement(inputBehaviourPort.port.ConnectTo(outputBehaviourPort.port));
            });

            behaviourGraphView.CreateInputNodes();
        }

        public void Clear()
        {
            behaviourGraphView.DeleteElements(behaviourGraphView.graphElements);

            behaviourGraphView.CreateInputNodes();
        }

        void OnDisable()
        {
            rootVisualElement.Remove(behaviourGraphView);
        }
    }
}

#endif
