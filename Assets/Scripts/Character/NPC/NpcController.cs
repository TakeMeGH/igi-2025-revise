using System;
using Perspective.Character.NPC.State;
using Perspective.StateMachine;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Perspective.Character.NPC
{
    public class NpcController : FiniteStateMachine
    {
        #region Component

        [Header("Component")] [SerializeField] private NavMeshSurface currentSurface;
        public NavMeshSurface CurrentSurface => currentSurface;
        public Animator Animator { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        #endregion

        #region NPC States

        [Header("NPC Attributes")] [SerializeField] NpcType npcType;
        public NpcType NpcType => npcType;
        [SerializeField] private NpcEvent currentEvent;
        public NpcEvent CurrentEvent => currentEvent;

        [SerializeField] private NpcController otherNpc;
        public NpcController OtherNpc => otherNpc;
        public Action<NpcEvent, NpcController> OnUpdateEvent;

        #endregion

        #region Civilian Attributes

        [Header("Civilian Attributes")] [SerializeField] private float conversationRange;
        [SerializeField] private float talkAngle;
        [SerializeField] private float nearToTalkDistance;
        public float NearToTalkDistance => nearToTalkDistance;
        private readonly Collider[] hitsAnotherNpc = new Collider[105];

        #endregion

        #region State

        public NpcIdlingState NpcIdlingState { get; private set; }
        public NpcWalkingState NpcWalkingState { get; private set; }
        public NpcConversationState NpcConversationState { get; private set; }

        #endregion

        private void OnDisable()
        {
            OnUpdateEvent = null;
        }

        private void Initialize()
        {
            Animator = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();

            NpcIdlingState = new NpcIdlingState(this);
            NpcWalkingState = new NpcWalkingState(this);
            NpcConversationState = new NpcConversationState(this);

            switch (NpcType)
            {
                case NpcType.None:
                    break;
                case NpcType.Civilian:
                    InvokeRepeating(nameof(DetectConversationEvent), 5.0f, 10.0f);
                    break;
                case NpcType.Thief:
                case NpcType.Guard:
                case NpcType.Beggar:
                case NpcType.Aggressor:
                default:
                    break;
            }
        }

        private void Start()
        {
            Initialize();
            SwitchState(NpcIdlingState);
        }

        private bool SetEvent(NpcEvent npcEvent, NpcController other)
        {
            if (currentEvent != NpcEvent.None) return false;

            currentEvent = npcEvent;
            otherNpc = other;
            OnUpdateEvent.Invoke(currentEvent, other);
            return true;
        }
        
        public void ResetEvent()
        {
            currentEvent =  NpcEvent.None;
        }


        #region Civilian

        private void DetectConversationEvent()
        {
            Debug.Log("DetectConversationEvent");
            if (CurrentEvent != NpcEvent.None)
            {
                return;
            }

            var count = Physics.OverlapSphereNonAlloc(transform.position, conversationRange,
                hitsAnotherNpc);
            for (var i = 0; i < count; i++)
            {
                var other = hitsAnotherNpc[i].GetComponent<NpcController>();
                if (!other || other == this) continue;
                if (other.NpcType != NpcType.Civilian) continue;

                var dirToOther = (other.transform.position - transform.position).normalized;
                var angle = Vector3.Angle(transform.forward, dirToOther);
                Debug.Log(angle + " " + talkAngle + " ANGLE");
                if (!(angle <= talkAngle * 0.5f)) continue;

                if (other.SetEvent(NpcEvent.Conversation, this))
                {
                    SetEvent(NpcEvent.Conversation, other);
                }
            }
        }

        #endregion
    }
}