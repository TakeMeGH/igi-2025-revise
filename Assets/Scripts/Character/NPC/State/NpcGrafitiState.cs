using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcGrafitiState : NpcBaseState
    {
        public NpcGrafitiState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Animator.Play("Graffiti");

            NpcController.SetEvent(NpcEvent.Grafiti, NpcController);
            NpcController.SetEventDetector(true);
        }
    }
}