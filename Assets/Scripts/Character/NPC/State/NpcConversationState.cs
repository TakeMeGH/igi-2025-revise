using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcConversationState : NpcBaseState
    {
        private float _defaultStopingDistance;
        private bool _conversationStarted;
        private float _conversationDuration;
        public NpcConversationState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            NpcController.Agent.isStopped = false;
            _conversationStarted = false;

            _conversationDuration = 4f;
            
            NpcController.Animator.Play("Idle/Walk");

            _defaultStopingDistance = NpcController.Agent.stoppingDistance;
            NpcController.Agent.stoppingDistance = NpcController.NearToTalkDistance;
        }

        public override void Update()
        {
            if (_conversationStarted)
            {
                _conversationDuration -= Time.deltaTime;
                if (_conversationDuration > 0) return;
                NpcController.SwitchState(NpcController.NpcWalkingState);
            }
            else
            {
                WalkToConversation();
            }
        }

        private void WalkToConversation()
        {
            NpcController.Agent.SetDestination(NpcController.OtherNpc.transform.position);
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();

            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;
            
            NpcController.Agent.isStopped = true;
            NpcController.Animator.Play("Talk");
            _conversationStarted = true;
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
            NpcController.ResetEvent();
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }
    }
}