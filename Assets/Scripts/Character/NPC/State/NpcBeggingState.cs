using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcBeggingState : NpcBaseState
    {
        public NpcBeggingState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            NpcController.Animator.Play("Begging");
            NpcController.Animator.SetFloat(Speed, 0.0f);

            NpcController.SetEvent(NpcEvent.Begging, NpcController);
            NpcController.SetEventDetector(true);
        }
    }
}