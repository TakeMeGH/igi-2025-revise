using Perspective.StateMachine;
using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcBaseState : IState
    {
        protected readonly NpcController NpcController;
        protected static readonly int Speed = Animator.StringToHash("Speed");

        protected NpcBaseState(NpcController npcController)
        {
            NpcController = npcController;
        }

        public virtual void Enter()
        {
            NpcController.OnUpdateEvent += OnUpdateEvent;
            NpcController.Agent.updateRotation = false;
        }

        public virtual void Update()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void Exit()
        {
            NpcController.OnUpdateEvent -= OnUpdateEvent;
        }

        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        public virtual void OnAnimationTransitionEvent()
        {
        }

        private void OnUpdateEvent(NpcEvent npcEvent, NpcController other)
        {
            switch (npcEvent)
            {
                case NpcEvent.None:
                    break;
                case NpcEvent.Conversation:
                    NpcController.SwitchState(NpcController.NpcConversationState);
                    break;
                case NpcEvent.BeggingInteraction:
                case NpcEvent.CrowdGathering:
                    break;
                case NpcEvent.PickPocket:
                    NpcController.SwitchState(NpcController.NpcPickPocketState);
                    break;
                case NpcEvent.Intimidation:
                    break;
                case NpcEvent.Fight:
                    NpcController.SwitchState(NpcController.NpcIntimidationState);
                    break;
                case NpcEvent.Yell:
                case NpcEvent.Chase:
                case NpcEvent.Panic:
                case NpcEvent.ChainReaction:
                case NpcEvent.StreetPerformance:
                case NpcEvent.DisableEvent:
                default:
                    break;
            }
        }

        protected void UpdateRotation()
        {
            var velocity = NpcController.Agent.desiredVelocity;

            if (velocity.sqrMagnitude <= 0.01f) return;

            var lookDir = velocity.normalized;
            lookDir.y = 0;

            var targetRot = Quaternion.LookRotation(lookDir);

            var angleDiff = Quaternion.Angle(NpcController.transform.rotation, targetRot);

            var dynamicSpeed = Mathf.Lerp(30f, NpcController.Agent.angularSpeed, angleDiff / 180f);

            NpcController.transform.rotation = Quaternion.RotateTowards(
                NpcController.transform.rotation,
                targetRot,
                dynamicSpeed * Time.deltaTime
            );
        }
    }
}