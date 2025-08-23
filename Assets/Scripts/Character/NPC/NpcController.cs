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

        [Header("Component")] private NavMeshSurface currentSurface;
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

        #endregion

        #region NPC Attributes

        [Header("NPC Attributes")] [SerializeField] private float walkSpeed = 1f;
        public float WalkSpeed => walkSpeed;
        [SerializeField] private float runSpeed = 4f;
        public float RunSpeed => runSpeed;
        public int chaseIndex;
        public Collider identifierCollider;
        private readonly Collider[] hitsAnotherNpc = new Collider[105];
        public Action OnNpcDestroyed;

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

        #region Brawler / Collector Attributes

        [Header("Brawler / Collector Attributes")] [SerializeField] private float brawlerRange;
        [SerializeField] private float brawlerAngle;
        [SerializeField] private float nearToBrawlDistance;
        [SerializeField] private NpcType brawlTarget = NpcType.Brawler;
        [SerializeField] private NpcEvent brawlEvent = NpcEvent.Fight;
        public float NearToBrawlDistance => nearToBrawlDistance;
        public bool MainFighter { get; private set; }
        public bool isPunching;

        #endregion

        #region State

        public NpcIdlingState NpcIdlingState { get; private set; }
        public NpcWalkingState NpcWalkingState { get; private set; }
        public NpcConversationState NpcConversationState { get; private set; }
        public NpcPickPocketState NpcPickPocketState { get; private set; }
        public NpcYellState NpcYellState { get; private set; }
        public NpcTrashTalkState NpcTrashTalkState { get; private set; }
        public NpcFightState NpcFightState { get; private set; }
        public NpcFleeState NpcFleeState { get; private set; }
        public NpcChaseState NpcChaseState { get; private set; }
        public NpcBeggingState NpcBeggingState { get; private set; }

        #endregion
        
        private void Initialize()
        {
            Animator = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();

            NpcIdlingState = new NpcIdlingState(this);
            NpcWalkingState = new NpcWalkingState(this);
            NpcConversationState = new NpcConversationState(this);
            NpcPickPocketState = new NpcPickPocketState(this);
            NpcYellState = new NpcYellState(this);
            NpcTrashTalkState = new NpcTrashTalkState(this);
            NpcFightState = new NpcFightState(this);
            NpcFleeState = new NpcFleeState(this);
            NpcChaseState = new NpcChaseState(this);
            NpcBeggingState = new NpcBeggingState(this);

            identifierCollider.enabled = false;

            var surfaces = GameObject.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);

            if (surfaces.Length > 0)
            {
                currentSurface = surfaces[0];
            }
            else
            {
                Debug.LogError("❌ No NavMeshSurface found in the scene!");
            }


            ExitPoints = GameObject.FindWithTag("ExitPoints");
            switch (NpcType)
            {
                case NpcType.None:
                    break;
                case NpcType.Civilian:
                    InvokeRepeating(nameof(DetectConversationEvent), 10.0f, 2.5f);
                    break;
                case NpcType.Thief:
                    InvokeRepeating(nameof(DetectStealEvent), 10.0f, 2.5f);
                    break;
                case NpcType.Police:
                case NpcType.Beggar:
                case NpcType.Merchant:
                    break;
                case NpcType.Brawler:
                    InvokeRepeating(nameof(DetectFightEvent), 10.0f, 2.5f);
                    break;
                case NpcType.Collector:
                    InvokeRepeating(nameof(DetectFightEvent), 10.0f, 2.5f);
                    break;
            }
        }

        private void StartFirstState()
        {
            switch (npcType)
            {
                case NpcType.Beggar:
                    SwitchState(NpcBeggingState);
                    break;
                case NpcType.Merchant:
                    SwitchState(NpcIdlingState);
                    break;
                default:
                    SwitchState(NpcWalkingState);
                    break;
            }
        }

        private void Start()
        {
            Initialize();
            StartFirstState();
        }

        public void ForcedSetEvent(NpcEvent npcEvent, NpcController other)
        {
            currentEvent = npcEvent;
            otherNpc = other;
            ((NpcBaseState)CurrentState).OnUpdateEvent(currentEvent, other);
        }

        public bool SetEvent(NpcEvent npcEvent, NpcController other)
        {
            if (currentEvent != NpcEvent.None) return false;

            currentEvent = npcEvent;
            otherNpc = other;
            ((NpcBaseState)CurrentState).OnUpdateEvent(currentEvent, other);
            return true;
        }

        public void ResetEvent()
        {
            currentEvent = NpcEvent.None;
        }
        
        public void ResetOtherNpc()
        {
            otherNpc = null;
        }


        public void SetEventDetector(bool isEnable)
        {
            identifierCollider.enabled = isEnable;
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

                if (!other.SetEvent(NpcEvent.Conversation, this)) continue;

                SetEvent(NpcEvent.Conversation, other);
                break;
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
                break;
            }
        }

        #endregion

        #region Brawler

        private void DetectFightEvent()
        {
            if (CurrentEvent != NpcEvent.None)
            {
                return;
            }

            var count = Physics.OverlapSphereNonAlloc(transform.position, brawlerRange,
                hitsAnotherNpc);
            for (var i = 0; i < count; i++)
            {
                var other = hitsAnotherNpc[i].GetComponent<NpcController>();
                if (!other || other == this) continue;
                if (other.NpcType != brawlTarget) continue;

                var dirToOther = (other.transform.position - transform.position).normalized;
                var angle = Vector3.Angle(transform.forward, dirToOther);
                if (!(angle <= brawlerAngle * 0.5f)) continue;

                if (!other.SetEvent(brawlEvent, this)) continue;

                SetEvent(brawlEvent, other);
                MainFighter = true;
                break;
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
            OnNpcDestroyed?.Invoke();
            OnNpcDestroyed = null;
            CancelInvoke();
            Destroy(gameObject);
        }
    }
}