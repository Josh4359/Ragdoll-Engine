using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BehaviourGraph
{

#if UNITY_EDITOR

    [CustomEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Edit Behaviour Tree"))
                BehaviourTreeLoader.LoadGraph(target as BehaviourTree).Load();
        }
    }

    [RequireComponent(typeof(BehaviourTreeLoader))]

#endif

    public class BehaviourTree : MonoBehaviour
    {

#if UNITY_EDITOR

        [HideInInspector, SerializeReference] public List<NodeGraphData> nodeGraphData;

        [HideInInspector] public List<EdgeGraphData> edgeGraphData;

#endif

        [HideInInspector, SerializeReference] public InputNodeTreeData update;

        [HideInInspector, SerializeReference] public InputNodeTreeData lateUpdate;

        [HideInInspector, SerializeReference] public InputNodeTreeData fixedUpdate;

        [HideInInspector, SerializeReference] public InputNodeTreeData lateFixedUpdate;

        [HideInInspector] public Behaviour[] behaviours;

        public event EventHandler Set;

        public event EventHandler Reset;

#if UNITY_EDITOR

        public BehaviourTreeLoader behaviourTreeLoader
        {
            get
            {
                return GetComponent<BehaviourTreeLoader>();
            }
        }

#endif

        public virtual void Update()
        {
            update.Run(this);
        }

        public virtual void LateUpdate()
        {
            lateUpdate.Run(this);
        }

        public virtual void FixedUpdate()
        {
            Reset?.Invoke(this, EventArgs.Empty);

            fixedUpdate.Run(this);

            StartCoroutine(ScheduleLateFixedUpdate());
        }

        public virtual void LateFixedUpdate()
        {
            lateFixedUpdate.Run(this);

            Set?.Invoke(this, EventArgs.Empty);
        }

        IEnumerator ScheduleLateFixedUpdate()
        {
            yield return new WaitForFixedUpdate();

            LateFixedUpdate();
        }

        public int GetBehaviourIndex(Behaviour behaviour)
        {
            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i] == behaviour) return i;

            behaviours = behaviours.Append(behaviour).ToArray();

            return behaviours.Length - 1;
        }
    }

    public class Behaviour : MonoBehaviour
    {
        [HideInInspector] public BehaviourTree behaviourTree;

        [HideInInspector] public bool active;

        [HideInInspector] public bool wasActive;

        public virtual void OnEnable()
        {
            behaviourTree = GetComponentInParent<BehaviourTree>();

            behaviourTree.Set += OnSet;

            behaviourTree.Reset += OnReset;
        }

        public virtual void OnDisable()
        {
            behaviourTree.Set -= OnSet;

            behaviourTree.Reset -= OnReset;
        }

        public virtual bool Evaluate()
        {
            return false;
        }

        public virtual void Execute() { }

        public virtual void OnSet(object sender, EventArgs e)
        {
            wasActive = active;
        }

        public virtual void OnReset(object sender, EventArgs e)
        {
            active = false;
        }
    }

#if UNITY_EDITOR

    #region Graph Data

    [Serializable]
    public class EdgeGraphData
    {
        public string input;

        public string output;
    }

    [Serializable]
    public class NodeGraphData
    {
        public Vector2 position;

        public string GUID;

        public virtual BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            BehaviourNode behaviourNode = new BehaviourNode()
            {
                GUID = GUID
            };

            behaviourNode.Create();

            behaviourNode.SetPosition(new Rect(position, Vector3.zero));

            return behaviourNode;
        }
    }

    [Serializable]
    public class InputNodeGraphData : NodeGraphData
    {
        public string title;

        public string outputPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            InputBehaviourNode inputBehaviourNode = new InputBehaviourNode()
            {
                title = title,

                GUID = GUID
            };

            inputBehaviourNode.Create();

            inputBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            inputBehaviourNode.outputPort.GUID = outputPortGUID;

            return inputBehaviourNode;
        }
    }

    [Serializable]
    public class CheckNodeGraphData : NodeGraphData
    {
        public int checkIndex;

        public string inputPortGUID;

        public string passPortGUID;

        public string failPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            CheckBehaviourNode checkBehaviourNode = new CheckBehaviourNode()
            {
                GUID = GUID
            };

            checkBehaviourNode.Create();

            checkBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            checkBehaviourNode.checkObjectField.value = behaviourTree.behaviours[checkIndex];

            checkBehaviourNode.inputPort.GUID = inputPortGUID;

            checkBehaviourNode.passPort.GUID = passPortGUID;

            checkBehaviourNode.failPort.GUID = failPortGUID;

            return checkBehaviourNode;
        }
    }

    [Serializable]
    public class ActionNodeGraphData : NodeGraphData
    {
        public int actionIndex;

        public string inputPortGUID;

        public string outputPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            ActionBehaviourNode actionBehaviourNode = new ActionBehaviourNode()
            {
                GUID = GUID
            };

            actionBehaviourNode.Create();

            actionBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            actionBehaviourNode.actionObjectField.value = behaviourTree.behaviours[actionIndex];

            actionBehaviourNode.inputPort.GUID = inputPortGUID;

            actionBehaviourNode.outputPort.GUID = outputPortGUID;

            return actionBehaviourNode;
        }
    }

    #endregion

#endif

    #region Tree Data

    [Serializable]
    public class NodeTreeData
    {
        public virtual void Reset(BehaviourTree behaviourTree) { }

        public virtual void Run(BehaviourTree behaviourTree) { }

        public virtual void Set(BehaviourTree behaviourTree) { }
    }

    [Serializable]
    public class InputNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        public override void Reset(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Run(behaviourTree);
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Set(behaviourTree);
        }
    }

    [Serializable]
    public class CheckNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        [SerializeReference] public NodeTreeData fail;

        public int checkIndex;

        public override void Reset(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
                thisCheck.active = false;

            if (pass != null)
                pass.Reset(behaviourTree);

            if (fail != null)
                fail.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
            {
                thisCheck.active = thisCheck.Evaluate();

                if (thisCheck.active)
                {
                    if (pass != null)
                        pass.Run(behaviourTree);
                }
                else if (fail != null)
                    fail.Run(behaviourTree);
            }
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
                thisCheck.wasActive = thisCheck.active;

            if (pass != null)
                pass.Set(behaviourTree);

            if (fail != null)
                fail.Set(behaviourTree);
        }
    }

    [Serializable]
    public class ActionNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        public int actionIndex;

        public override void Reset(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            Behaviour thisAction = behaviourTree.behaviours[actionIndex];

            if (thisAction != null)
                thisAction.Execute();

            if (pass != null)
                pass.Run(behaviourTree);
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Set(behaviourTree);
        }
    }

    #endregion
}
