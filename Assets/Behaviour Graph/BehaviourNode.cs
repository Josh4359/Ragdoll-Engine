#if UNITY_EDITOR

using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourGraph
{
    public class BehaviourNode : Node
    {
        public string GUID;

        public class BehaviourPort
        {
            public Port port;
            
            public string GUID;
        }

        public virtual void Initialize()
        {
            GUID = Guid.NewGuid().ToString();

            Create();
        }

        public virtual void Create() { }

        public virtual NodeGraphData CreateNodeGraphData(BehaviourTree behaviourTree)
        {
            return new NodeGraphData()
            {
                position = GetPosition().position,

                GUID = GUID
            };
        }

        public virtual NodeTreeData CreateNodeTreeData(BehaviourTree behaviourTree)
        {
            return new NodeTreeData() { };
        }

        public virtual BehaviourPort GetContainer(Port port) { return null; }

        public virtual BehaviourPort GetContainer(string GUID) { return null; }

        public BehaviourPort AddContainer(string name, Orientation orientation, Direction direction, Port.Capacity capacity, string portGUID)
        {
            BehaviourPort behaviourPort = new BehaviourPort()
            {
                port = InstantiatePort(orientation, direction, capacity, typeof(float)),

                GUID = portGUID
            };
            
            behaviourPort.port.portName = name;

            switch(direction)
            {
                case Direction.Input:
                    inputContainer.Add(behaviourPort.port);

                    break;
                case Direction.Output:
                    outputContainer.Add(behaviourPort.port);

                    break;
            }

            return behaviourPort;
        }
    }
}

#endif
