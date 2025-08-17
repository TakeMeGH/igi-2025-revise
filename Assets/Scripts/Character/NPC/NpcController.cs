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
        public GameObject ExitPoints { get; private set; }

        #endregion

        #region NPC States

        [Header("NPC States")] [SerializeField] NpcType npcType;
        public NpcType NpcType => npcType;
        [SerializeField] private NpcEvent currentEvent;
        public NpcEvent CurrentEvent => currentEvent;

        [SerializeField] private NpcController otherNpc;
        public NpcController OtherNpc => otherNpc;
        public Action<NpcEvent, NpcController> OnUpdateEvent;

        #endregion

        #region NPC Attributes

        [Header("NPC Attributes")] [SerializeField] private float walkSpeed = 1f;
        public float WalkSpeed => walkSpeed;
        [SerializeField] private float runSpeed = 4f;
        public float RunSpeed => runSpeed;
        private readonly Collider[] hitsAnotherNpc = new Collider[105];

        #endregion

        #region Civilian Attributes

        [Header("Civilian Attributes")] [SerializeField] private float conversationRange;
        [SerializeField] private float talkAngle;
        [SerializeField] private float nearToTalkDistance;
        public float NearToTalkDistance => nearToTalkDistance;
        private bool Stolen { get; set; }

        #endregion

        #region Thief Attributes

        [Header("Thief Attributes")] [SerializeField] private float stealRange;
        [SerializeField] private float stealAngle;
        [SerializeField] private float nearToStealDistance;
        public float NearToStealDistance => nearToStealDistance;

        #endregion


        #region State

        public NpcIdlingState NpcIdlingState { get; private set; }
        public NpcWalkingState NpcWalkingState { get; private set; }
        public NpcConversationState NpcConversationState { get; private set; }
        public NpcPickPocketState NpcPickPocketState { get; private set; }
        public NpcYellState NpcYellState { get; private set; }
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
            NpcPickPocketState = new NpcPickPocketState(this);
            NpcYellState = new NpcYellState(this);

            ExitPoints = GameObject.FindWithTag("ExitPoints");
            switch (NpcType)
            {
                case NpcType.None:
                    break;
                case NpcType.Civilian:
                    InvokeRepeating(nameof(DetectConversationEvent), 5.0f, 10.0f);
                    break;
                case NpcType.Thief:
                    InvokeRepeating(nameof(DetectStealEvent), 10.0f, 2.5f);
                    break;
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

        public bool SetEvent(NpcEvent npcEvent, NpcController other)
        {
            if (currentEvent != NpcEvent.None) return false;

            currentEvent = npcEvent;
            otherNpc = other;
            OnUpdateEvent.Invoke(currentEvent, other);
            return true;
        }

        public void ResetEvent()
        {
            currentEvent = NpcEvent.None;
        }


        #region Civilian

        private void DetectConversationEvent()
        {
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
                if (!(angle <= talkAngle * 0.5f)) continue;

                if (other.SetEvent(NpcEvent.Conversation, this))
                {
                    SetEvent(NpcEvent.Conversation, other);
                }
            }
        }

        #endregion

        #region Thief

        private void DetectStealEvent()
        {
            if (CurrentEvent != NpcEvent.None)
            {
                return;
            }

            var count = Physics.OverlapSphereNonAlloc(transform.position, stealRange,
                hitsAnotherNpc);
            for (var i = 0; i < count; i++)
            {
                var other = hitsAnotherNpc[i].GetComponent<NpcController>();
                if (!other || other == this) continue;
                if (other.NpcType != NpcType.Civilian) continue;

                var dirToOther = (other.transform.position - transform.position).normalized;
                var angle = Vector3.Angle(transform.forward, dirToOther);
                if (!(angle <= stealAngle * 0.5f)) continue;

                if (other.Stolen) continue;

                other.Stolen = true;
                SetEvent(NpcEvent.PickPocket, other);
            }
        }

        #endregion

        public void OnAnimationEnterEvent()
        {
            CurrentState.OnAnimationEnterEvent();
        }

        public void OnAnimationExitEvent()
        {
            CurrentState.OnAnimationExitEvent();
        }

        public void OnAnimationTransitionEvent()
        {
            CurrentState.OnAnimationTransitionEvent();
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}