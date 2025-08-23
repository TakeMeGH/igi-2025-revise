using Perspective.Utils;

namespace Perspective.Character.NPC.State
{
    public class NpcFleeState : NpcBaseState
    {
        private bool _isGettingDestroyed;
        public NpcFleeState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;

            NpcController.SetEvent(NpcEvent.DisableEvent, NpcController);

            NavMeshUtils.SetDestinationNearest(NpcController.Agent,
                NpcController.ExitPoints.transform.GetChild(0).transform.position);

            NpcController.OtherNpc.chaseIndex = 0;

            NpcController.Agent.speed = NpcController.RunSpeed;

            _isGettingDestroyed = false;
            
            NpcController.Animator.Play("Idle/Walk");
        }

        public override void Update()
        {
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();
            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;
            if (_isGettingDestroyed) return;

            _isGettingDestroyed = true;
            NpcController.SelfDestroy();
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
        }
    }
}