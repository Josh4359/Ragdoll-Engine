#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace BehaviourGraph
{
    public class ActionBehaviourNode : BehaviourNode
    {
        public BehaviourPort inputPort;

        public BehaviourPort outputPort;

        public ObjectField actionObjectField;

        public override void Create()
        {
            title = "Action";

            inputPort = AddContainer("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Guid.NewGuid().ToString());

            outputPort = AddContainer("Output", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, Guid.NewGuid().ToString());

            actionObjectField = new ObjectField();

            actionObjectField.objectType = typeof(Behaviour);

            extensionContainer.Add(actionObjectField);

            RefreshExpandedState();

            RefreshPorts();
        }

        public override NodeGraphData CreateNodeGraphData(BehaviourTree behaviourTree)
        {
            return new ActionNodeGraphData()
            {
                actionIndex = behaviourTree.GetBehaviourIndex(actionObjectField.value as Behaviour),

                inputPortGUID = inputPort.GUID,

                outputPortGUID = outputPort.GUID,

                position = GetPosition().position,

                GUID = GUID
            };
        }

        public override NodeTreeData CreateNodeTreeData(BehaviourTree behaviourTree)
        {
            return new ActionNodeTreeData()
            {
                pass = outputPort.port.connected ? ((BehaviourNode)outputPort.port.connections.First().input.node).CreateNodeTreeData(behaviourTree) : null,

                actionIndex = behaviourTree.GetBehaviourIndex(actionObjectField.value as Behaviour)
            };
        }

        public override BehaviourPort GetContainer(Port port)
        {
            if (inputPort.port == port)
                return inputPort;

            if (outputPort.port == port)
                return outputPort;

            return null;
        }

        public override BehaviourPort GetContainer(string GUID)
        {
            if (inputPort.GUID == GUID)
                return inputPort;

            if (outputPort.GUID == GUID)
                return outputPort;

            return null;
        }
    }
}

#endif
