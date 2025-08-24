using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcComeOverState : NpcBaseState
    {
        private float _defaultStopingDistance;

        public NpcComeOverState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;

            NpcController.Animator.Play("Idle/Walk");

            _defaultStopingDistance = NpcController.Agent.stoppingDistance;
            switch (NpcController.NpcType)
            {
                case NpcType.PoliceKidnapper:
                    NpcController.Agent.stoppingDistance = NpcController.NearToKidnapDistance;
                    break;
                case NpcType.HardcoreThief:
                    NpcController.Agent.stoppingDistance = NpcController.NearToRobDistance;
                    break;
                case NpcType.BrutalPolice:
                    NpcController.Agent.stoppingDistance = NpcController.NearToBeatdownDistance;
                    break;
                default:
                    NpcController.Agent.stoppingDistance = NpcController.NearToPunchDistance;
                    break;
            }
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcIdlingState);
        }

        public override void Update()
        {
            WalkToConversation();
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
            NpcController.SetEventDetector(true);
            NpcController.SwitchState(NpcController.NpcOneSidedBlowsState);
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcOneSidedBlowsState);

            NpcController.OtherNpc.transform.LookAt(NpcController.OtherNpc.transform.position);
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }
    }
}