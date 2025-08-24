using Unity.VisualScripting;
using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcOneSidedKickDownState : NpcBaseState
    {
        private int _kickLimit;

        public NpcOneSidedKickDownState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = true;
            _kickLimit = 6;

            NpcController.transform.Rotate(0f, -45f, 0f);


            StartKickDown();
        }

        public override void Exit()
        {
        }

        private void StartKickDown()
        {
            if (NpcController.isPoliceBeater && _kickLimit == 0)
            {
                NpcController.SwitchState(NpcController.NpcFleeState);
                return;
            }

            NpcController.Animator.Play("KickDown", 0, 0.01f);
            _kickLimit--;
        }

        public override void OnAnimationExitEvent()
        {
            StartKickDown();
        }
    }
}