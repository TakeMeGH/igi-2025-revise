using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcYellState : NpcBaseState
    {
        private float _yellDuration;

        public NpcYellState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Animator.Play("Yell");

            _yellDuration = 1.5f;

            NpcController.Agent.isStopped = true;

            NpcController.ResetEvent();
            NpcController.SetEvent(NpcEvent.DisableEvent, NpcController);
            NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);
        }

        public override void Update()
        {
            if (_yellDuration > 0)
            {
                _yellDuration -= Time.deltaTime;
                if (NpcController.OtherNpc)
                    NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);
                return;
            }

            NpcController.SwitchState(NpcController.NpcChaseState);
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
        }
    }
}