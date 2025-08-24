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

        [Header("NPC States")] [SerializeField] private NpcType npcType;
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

        #region IntimidatingPolice Event

        [Header("Intimidating Police Attributes")] [SerializeField] private float intimidatingPoliceRange;
        [SerializeField] private float intimidatingPoliceAngle;
        [SerializeField] private float nearToIntimidatingPoliceDistance;

        #endregion

        #region Civilians Punch Event

        [Header("Civilians Punch Attributes")] [SerializeField] private float punchRange;
        [SerializeField] private float punchAngle;
        [SerializeField] private float nearToPunchDistance;
        public float NearToPunchDistance => nearToPunchDistance;
        public bool isPunchingOnOneSidedBlows;

        #endregion

        #region Police Kidnap Event

        [Header("Police Kidnap Attributes")] [SerializeField] private float kidnapRange;
        [SerializeField] private float kidnapAngle;
        [SerializeField] private float nearToKidnapDistance;
        public float NearToKidnapDistance => nearToKidnapDistance;
        public bool isKidnapper;

        #endregion

        #region Hardcore Robbery Event

        [Header("Hardcore Robbery Attributes")] [SerializeField] private float robberyRange;
        [SerializeField] private float robberyAngle;
        [SerializeField] private float nearToRobDistance;
        public float NearToRobDistance => nearToRobDistance;
        public bool isRobber;

        #endregion

        #region Brutal Police Event

        [Header("Brutal Police Attributes")] [SerializeField] private float beatdownRange;
        [SerializeField] private float beatdownAngle;
        [SerializeField] private float nearToBeatdownDistance;
        public float NearToBeatdownDistance => nearToBeatdownDistance;
        public bool isPoliceBeater;

        #endregion


        #region State

        public NpcIdlingState NpcIdlingState { get; private set; }
        public NpcWalkingState NpcWalkingState { get; private set; }
        public NpcMeetInTheMiddleState NpcMeetInTheMiddleState { get; private set; }
        public NpcPickPocketState NpcPickPocketState { get; private set; }
        public NpcYellState NpcYellState { get; private set; }
        public NpcTrashTalkState NpcTrashTalkState { get; private set; }
        public NpcTradingBlowsState NpcTradingBlowsState { get; private set; }
        public NpcFleeState NpcFleeState { get; private set; }
        public NpcChaseState NpcChaseState { get; private set; }
        public NpcBeggingState NpcBeggingState { get; private set; }
        public NpcComeOverState NpcComeOverState { get; private set; }
        public NpcOneSidedBlowsState NpcOneSidedBlowsState { get; private set; }
        public NpcKidnappingState NpcKidnappingState { get; private set; }
        public NpcFallenState NpcFallenState { get; private set; }
        
        public NpcOneSidedKickDownState NpcOneSidedKickDownState { get; private set; }

        #endregion

        private void Initialize()
        {
            Animator = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();

            // Init states
            NpcIdlingState = new NpcIdlingState(this);
            NpcWalkingState = new NpcWalkingState(this);
            NpcMeetInTheMiddleState = new NpcMeetInTheMiddleState(this);
            NpcPickPocketState = new NpcPickPocketState(this);
            NpcYellState = new NpcYellState(this);
            NpcTrashTalkState = new NpcTrashTalkState(this);
            NpcTradingBlowsState = new NpcTradingBlowsState(this);
            NpcFleeState = new NpcFleeState(this);
            NpcChaseState = new NpcChaseState(this);
            NpcBeggingState = new NpcBeggingState(this);
            NpcComeOverState = new NpcComeOverState(this);
            NpcOneSidedBlowsState = new NpcOneSidedBlowsState(this);
            NpcKidnappingState = new NpcKidnappingState(this);
            NpcFallenState = new NpcFallenState(this);
            NpcOneSidedKickDownState = new NpcOneSidedKickDownState(this);

            identifierCollider.enabled = false;
            
            Agent.avoidancePriority = UnityEngine.Random.Range(30, 70);

            var surfaces = GameObject.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
            if (surfaces.Length > 0)
                currentSurface = surfaces[0];
            else
                Debug.LogError("❌ No NavMeshSurface found in the scene!");

            ExitPoints = GameObject.FindWithTag("ExitPoints");

            // Setup event detection based on type
            switch (NpcType)
            {
                case NpcType.Civilian:
                    InvokeRepeating(nameof(DetectConversationEvent), 10.0f, 2.5f);
                    break;
                case NpcType.Thief:
                    InvokeRepeating(nameof(DetectStealEvent), 10.0f, 2.5f);
                    break;
                case NpcType.Brawler:
                case NpcType.Collector:
                    InvokeRepeating(nameof(DetectFightEvent), 10.0f, 2.5f);
                    break;
                case NpcType.IntimidatingPolice:
                    InvokeRepeating(nameof(DetectIntimidatingPoliceEvent), 10.0f, 2.5f);
                    break;
                case NpcType.CivilianBeater:
                    InvokeRepeating(nameof(DetectOneSidedBlowsByCivilian), 10.0f, 2.5f);
                    break;
                case NpcType.PoliceKidnapper:
                    InvokeRepeating(nameof(DetectKidnapEvent), 10.0f, 2.5f);
                    break;
                case NpcType.HardcoreThief:
                    InvokeRepeating(nameof(DetectRobberyEvent), 10.0f, 2.5f);
                    break;
                case NpcType.BrutalPolice:
                    InvokeRepeating(nameof(DetectBrutalPoliceEvent), 10.0f, 2.5f);
                    break;
            }
        }

        private void StartFirstState()
        {
            SwitchState(npcType switch
            {
                NpcType.Beggar => NpcBeggingState,
                NpcType.Merchant => NpcIdlingState,
                _ => NpcWalkingState
            });
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

        public void ResetEvent() => currentEvent = NpcEvent.None;
        public void ResetOtherNpc() => otherNpc = null;
        public void SetEventDetector(bool isEnable) => identifierCollider.enabled = isEnable;

        #region Event Detection (Refactored)

        private void DetectNpcEvent(
            float range,
            float angleLimit,
            Predicate<NpcController> filter,
            Action<NpcController> onValidTarget,
            float minDistance = 0f) // 👈 new optional argument
        {
            if (CurrentEvent != NpcEvent.None) return;

            var count = Physics.OverlapSphereNonAlloc(transform.position, range, hitsAnotherNpc);
            if (count <= 0) return;

            // Shuffle results for randomness
            for (int i = 0; i < count; i++)
            {
                int randIndex = UnityEngine.Random.Range(i, count);
                (hitsAnotherNpc[i], hitsAnotherNpc[randIndex]) = (hitsAnotherNpc[randIndex], hitsAnotherNpc[i]);
            }

            for (var i = 0; i < count; i++)
            {
                var other = hitsAnotherNpc[i].GetComponent<NpcController>();
                if (!other || other == this) continue;
                if (!filter(other)) continue;

                var dirToOther = (other.transform.position - transform.position).normalized;
                var angle = Vector3.Angle(transform.forward, dirToOther);
                if (!(angle <= angleLimit * 0.5f)) continue;

                var distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance < minDistance) continue;

                onValidTarget(other);
                break;
            }
        }


        private void DetectConversationEvent()
        {
            DetectNpcEvent(conversationRange, talkAngle,
                other => other.NpcType == NpcType.Civilian,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.Conversation, this)) return;
                    SetEvent(NpcEvent.Conversation, other);
                },
                nearToTalkDistance);
        }

        private void DetectStealEvent()
        {
            DetectNpcEvent(stealRange, stealAngle,
                other => other.NpcType == NpcType.Civilian && !other.Stolen,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.PickPocket, this)) return;
                    SetEvent(NpcEvent.PickPocket, other);
                    other.Stolen = true;
                },
                nearToStealDistance);
        }

        private void DetectFightEvent()
        {
            DetectNpcEvent(brawlerRange, brawlerAngle,
                other => other.NpcType == brawlTarget,
                other =>
                {
                    if (!other.SetEvent(brawlEvent, this)) return;
                    SetEvent(brawlEvent, other);
                    MainFighter = true;
                },
                nearToBrawlDistance);
        }

        private void DetectIntimidatingPoliceEvent()
        {
            DetectNpcEvent(intimidatingPoliceRange, intimidatingPoliceAngle,
                other => other.NpcType == NpcType.Civilian,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.IntimidationPolice, this)) return;
                    SetEvent(NpcEvent.IntimidationPolice, other);
                },
                nearToIntimidatingPoliceDistance);
        }

        private void DetectOneSidedBlowsByCivilian()
        {
            DetectNpcEvent(punchRange, punchAngle,
                other => other.NpcType == NpcType.PoliceWalkAround,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.OneSidedBlowsByCivilians, this)) return;
                    SetEvent(NpcEvent.OneSidedBlowsByCivilians, other);
                    isPunchingOnOneSidedBlows = true;
                    other.isPunchingOnOneSidedBlows = false;
                },
                nearToPunchDistance);
        }

        private void DetectKidnapEvent()
        {
            DetectNpcEvent(kidnapRange, kidnapAngle,
                other => other.NpcType == NpcType.Civilian,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.PoliceKidnapCivilian, this)) return;
                    SetEvent(NpcEvent.PoliceKidnapCivilian, other);

                    isKidnapper = true;
                    other.isKidnapper = false;
                },
                nearToKidnapDistance);
        }

        private void DetectRobberyEvent()
        {
            DetectNpcEvent(robberyRange, robberyAngle,
                other => other.NpcType == NpcType.Civilian,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.HardcoreRobbery, this)) return;
                    SetEvent(NpcEvent.HardcoreRobbery, other);

                    isRobber = true;
                    other.isRobber = false;
                },
                nearToRobDistance);
        }

        private void DetectBrutalPoliceEvent()
        {
            DetectNpcEvent(beatdownRange, beatdownAngle,
                other => other.NpcType == NpcType.Civilian,
                other =>
                {
                    if (!other.SetEvent(NpcEvent.CivilianBeatdown, this)) return;
                    SetEvent(NpcEvent.CivilianBeatdown, other);

                    isPoliceBeater = true;
                    other.isPoliceBeater = false;
                },
                nearToBeatdownDistance);
        }

        #endregion

        #region Animation Events

        public void OnAnimationEnterEvent() => CurrentState.OnAnimationEnterEvent();
        public void OnAnimationExitEvent() => CurrentState.OnAnimationExitEvent();
        public void OnAnimationTransitionEvent() => CurrentState.OnAnimationTransitionEvent();

        #endregion

        public void SelfReleased()
        {
            OnNpcDestroyed?.Invoke();
            OnNpcDestroyed = null;
            CancelInvoke();
        }
        public void SelfDestroy()
        {
            SelfReleased();
            Destroy(gameObject);
        }

        public void SetRunSpeed(float speed)
        {
            runSpeed = speed;
        }
    }
}