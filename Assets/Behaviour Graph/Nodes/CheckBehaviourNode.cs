#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace BehaviourGraph
{
    public class CheckBehaviourNode : BehaviourNode
    {
        public BehaviourPort inputPort;

        public BehaviourPort passPort;

        public BehaviourPort failPort;

        public ObjectField checkObjectField;

        public override void Create()
        {
            title = "Check";

            inputPort = AddContainer("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Guid.NewGuid().ToString());

            passPort = AddContainer("Pass", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, Guid.NewGuid().ToString());

            failPort = AddContainer("Fail", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, Guid.NewGuid().ToString());

            checkObjectField = new ObjectField();

            checkObjectField.objectType = typeof(Behaviour);

            extensionContainer.Add(checkObjectField);

            RefreshExpandedState();

            RefreshPorts();
        }

        public override NodeGraphData CreateNodeGraphData(BehaviourTree behaviourTree)
        {
            return new CheckNodeGraphData()
            {
                checkIndex = behaviourTree.GetBehaviourIndex(checkObjectField.value as Behaviour),

                inputPortGUID = inputPort.GUID,

                passPortGUID = passPort.GUID,

                failPortGUID = failPort.GUID,

                position = GetPosition().position,

                GUID = GUID
            };
        }

        public override NodeTreeData CreateNodeTreeData(BehaviourTree behaviourTree)
        {
            return new CheckNodeTreeData()
            {
                pass = passPort.port.connected ? ((BehaviourNode)passPort.port.connections.First().input.node).CreateNodeTreeData(behaviourTree) : null,

                fail = failPort.port.connected ? ((BehaviourNode)failPort.port.connections.First().input.node).CreateNodeTreeData(behaviourTree) : null,

                checkIndex = behaviourTree.GetBehaviourIndex(checkObjectField.value as Behaviour)
            };
        }

        public override BehaviourPort GetContainer(Port port)
        {
            if (inputPort.port == port)
                return inputPort;

            if (passPort.port == port)
                return passPort;

            if (failPort.port == port)
                return failPort;

            return null;
        }

        public override BehaviourPort GetContainer(string GUID)
        {
            if (inputPort.GUID == GUID)
                return inputPort;

            if (passPort.GUID == GUID)
                return passPort;

            if (failPort.GUID == GUID)
                return failPort;

            return null;
        }
    }
}

#endif
