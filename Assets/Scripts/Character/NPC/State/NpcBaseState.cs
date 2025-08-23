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

        public void OnUpdateEvent(NpcEvent npcEvent, NpcController other)
        {
            switch (npcEvent)
            {
                case NpcEvent.None:
                    break;
                case NpcEvent.Conversation:
                    NpcController.SwitchState(NpcController.NpcConversationState);
                    break;
                case NpcEvent.PickPocket:
                    if (NpcController.NpcType == NpcType.Thief)
                        NpcController.SwitchState(NpcController.NpcPickPocketState);
                    break;
                case NpcEvent.Fight:
                    NpcController.SwitchState(NpcController.NpcTrashTalkState);
                    break;
                case NpcEvent.Intimidation:
                    if (NpcController.NpcType == NpcType.Collector)
                        NpcController.SwitchState(NpcController.NpcTrashTalkState);
                    break;
                case NpcEvent.Begging:
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