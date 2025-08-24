using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcOneSidedBlowsState : NpcBaseState
    {
        private int _punchLimit;
        public NpcOneSidedBlowsState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = true;
            NpcController.Agent.speed = NpcController.RunSpeed;
            _punchLimit = 1;

            if (NpcController.isKidnapper)
            {
                StartPunching();
            }
            else if (NpcController.isPunchingOnOneSidedBlows || NpcController.isRobber || NpcController.isPoliceBeater)
            {
                NpcController.SetEventDetector(true);
                NpcController.OtherNpc.SetEventDetector(true);
                StartPunching();
            }
            else
            {
                NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);
            }
        }

        public override void Exit()
        {
        }

        private void StartPunching()
        {
            if ((NpcController.isPunchingOnOneSidedBlows || NpcController.isKidnapper || NpcController.isRobber 
                 || NpcController.isPoliceBeater) && _punchLimit == 0)
            {
                if (NpcController.CurrentEvent == NpcEvent.PoliceKidnapCivilian)
                {
                    NpcController.SwitchState(NpcController.NpcKidnappingState);
                    NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcKidnappingState);
                }
                else if (NpcController.CurrentEvent == NpcEvent.HardcoreRobbery)
                {
                    NpcController.SwitchState(NpcController.NpcFleeState);
                    NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcFallenState);
                }
                else if (NpcController.CurrentEvent == NpcEvent.CivilianBeatdown)
                {
                    NpcController.SwitchState(NpcController.NpcOneSidedKickDownState);
                    NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcFallenState);
                }
                else
                {
                    NpcController.SwitchState(NpcController.NpcFleeState);
                    NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcChaseState);
                    NpcController.SetEventDetector(false);
                    NpcController.OtherNpc.SetEventDetector(false);
                }

                return;
            }

            NpcController.Animator.Play("Punching", 0, 0.01f);
            NpcController.isPunching = true;
            _punchLimit--;
        }

        public override void OnAnimationExitEvent()
        {
            if (NpcController.CurrentEvent == NpcEvent.PoliceKidnapCivilian)
            {
                if (!NpcController.isKidnapper)
                {
                    ((NpcOneSidedBlowsState)NpcController.OtherNpc.GetCurrentState()).StartPunching();
                }
                else
                {
                    NpcController.OtherNpc.Animator.Play("PunchedFall", 0, 0f);
                }
            }
            else if (NpcController.CurrentEvent == NpcEvent.HardcoreRobbery)
            {
                if (!NpcController.isRobber)
                {
                    ((NpcOneSidedBlowsState)NpcController.OtherNpc.GetCurrentState()).StartPunching();
                }
                else
                {
                    NpcController.OtherNpc.Animator.Play("PunchedFall", 0, 0f);
                }
            }
            else if (NpcController.CurrentEvent == NpcEvent.CivilianBeatdown)
            {
                if (!NpcController.isPoliceBeater)
                {
                    ((NpcOneSidedBlowsState)NpcController.OtherNpc.GetCurrentState()).StartPunching();
                }
                else
                {
                    NpcController.OtherNpc.Animator.Play("PunchedFall", 0, 0f);
                }
            }
            else
            {
                if (!NpcController.isPunchingOnOneSidedBlows)
                {
                    ((NpcOneSidedBlowsState)NpcController.OtherNpc.GetCurrentState()).StartPunching();
                }
                else
                {
                    NpcController.OtherNpc.Animator.Play("Punched", 0, 0f);
                }
            }
        }
    }
}