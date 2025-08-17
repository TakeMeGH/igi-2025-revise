using Perspective.Utils;
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

            NpcController.Agent.isStopped = false;

            NpcController.Animator.Play("Yell");

            _yellDuration = 4.0f;

            NpcController.Agent.isStopped = true;
            
            NpcController.ResetEvent();
            NpcController.SetEvent(NpcEvent.DisableEvent, NpcController);
        }

        public override void Update()
        {
            if (_yellDuration > 0)
            {
                _yellDuration -= Time.deltaTime;
                return;
            }

            NavMeshUtils.SetDestinationNearest(NpcController.Agent,
                NpcController.ExitPoints.transform.GetChild(1).transform.position);

            NpcController.Agent.isStopped = false;
            NpcController.Agent.speed = NpcController.RunSpeed;
            NpcController.Animator.Play("Idle/Walk");

            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();
            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;
            NpcController.SelfDestroy();
        }
        
        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
        }
    }
}