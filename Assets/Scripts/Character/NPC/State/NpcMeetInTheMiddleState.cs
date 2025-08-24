using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcMeetInTheMiddleState : NpcBaseState
    {
        private float _defaultStopingDistance;
        private bool _meetInTheMiddleStarted;
        private float _meetInTheMiddleDuration;

        public NpcMeetInTheMiddleState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;
            _meetInTheMiddleStarted = false;

            _meetInTheMiddleDuration = 8f;

            NpcController.Animator.Play("Idle/Walk");

            _defaultStopingDistance = NpcController.Agent.stoppingDistance;
            NpcController.Agent.stoppingDistance = NpcController.NearToTalkDistance;
        }

        public override void Update()
        {
            if (_meetInTheMiddleStarted)
            {
                _meetInTheMiddleDuration -= Time.deltaTime;
                if (_meetInTheMiddleDuration > 0) return;
                NpcController.SwitchState(NpcController.NpcWalkingState);
                NpcController.SetEventDetector(false);
            }
            else
            {
                WalkToConversation();
            }
        }

        private void WalkToConversation()
        {
            if (!NpcController.OtherNpc)
            {
                NpcController.SwitchState(NpcController.NpcIdlingState);
                return;
            }

            NpcController.Agent.SetDestination(NpcController.OtherNpc.transform.position);
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();

            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;

            NpcController.Agent.isStopped = true;
            _meetInTheMiddleStarted = true;
            NpcController.SetEventDetector(true);
            
            NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);

            switch (NpcController.CurrentEvent)
            {
                case NpcEvent.Conversation:
                    NpcController.Animator.Play("Talk");
                    break;
                case NpcEvent.IntimidationPoliceAndCivilianScared when NpcController.NpcType == NpcType.Civilian:
                    NpcController.Animator.Play("PeaceTalk");
                    break;
                case NpcEvent.IntimidationPoliceAndCivilianScared:
                    NpcController.Animator.Play("Yell", 0, Random.Range(0, 2f));
                    break;
                default:
                    NpcController.Animator.Play("Yell", 0, Random.Range(0, 2f));
                    break;
            }
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
            NpcController.ResetEvent();
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }
    }
}