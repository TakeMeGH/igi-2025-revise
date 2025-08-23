using Perspective.Utils;
using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcWalkingState : NpcBaseState
    {
        private float _walkDuration;
        public NpcWalkingState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            _walkDuration = Random.Range(10f, 15f);
            
            PickNewDestination();

            NpcController.Agent.isStopped = false;
            
            NpcController.Animator.Play("Idle/Walk");
        }

        public override void Update()
        {
            if (_walkDuration > 0)
            {
                _walkDuration -= Time.deltaTime;
            }
            
            if (_walkDuration <= 0f)
            {
                NpcController.SwitchState(NpcController.NpcIdlingState);
                return;
            }
            
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            if (!NpcController.Agent.pathPending && NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)
            {
                PickNewDestination();
            }
            UpdateRotation();
        }
        
        private void PickNewDestination()
        {
            var newPos = NavMeshUtils.Instance.GetRandomPointOnSurface(NpcController.CurrentSurface);
            NpcController.Agent.SetDestination(newPos);
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
        }

    }
}
