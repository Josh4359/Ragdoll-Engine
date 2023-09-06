#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace BehaviourGraph
{
    public class InputBehaviourNode : BehaviourNode
    {
        public BehaviourPort outputPort;

        public override void Create()
        {
            outputPort = AddContainer("Output", Orientation.Horizontal, Direction.Output, Port.Capacity.Single, Guid.NewGuid().ToString());

            capabilities -= Capabilities.Deletable;

            RefreshExpandedState();

            RefreshPorts();
        }

        public override NodeGraphData CreateNodeGraphData(BehaviourTree behaviourTree)
        {
            return new InputNodeGraphData()
            {
                outputPortGUID = outputPort.GUID,

                position = GetPosition().position,

                title = title,

                GUID = GUID
            };
        }

        public override NodeTreeData CreateNodeTreeData(BehaviourTree behaviourTree)
        {
            return new InputNodeTreeData()
            {
                pass = outputPort.port.connected ? ((BehaviourNode)outputPort.port.connections.First().input.node).CreateNodeTreeData(behaviourTree) : null
            };
        }

        public override BehaviourPort GetContainer(Port port)
        {
            if (outputPort.port == port)
                return outputPort;

            return null;
        }

        public override BehaviourPort GetContainer(string GUID)
        {
            if (outputPort.GUID == GUID)
                return outputPort;

            return null;
        }
    }

}

#endif
