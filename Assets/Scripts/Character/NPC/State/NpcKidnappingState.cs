using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcKidnappingState : NpcBaseState
    {
        public NpcKidnappingState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            NpcController.Agent.isStopped = true;
            if (NpcController.isKidnapper)
                NpcController.Animator.Play("PickUp");

            NpcController.SetEventDetector(true);
            NpcController.OtherNpc.SetEventDetector(true);
        }

        public override void OnAnimationExitEvent()
        {
            if (!NpcController.OtherNpc) return;
            NpcController.OtherNpc.Agent.enabled = false;

            NpcController.OtherNpc.transform.SetParent(NpcController.transform);

            NpcController.OtherNpc.transform.localPosition = Vector3.zero;

            NpcController.OtherNpc.transform.localRotation = Quaternion.identity;

            NpcController.OtherNpc.Animator.Play("Dibopong", 0, 0f);
            
            NpcController.SwitchState(NpcController.NpcFleeState);
        }
    }
}