using Perspective.Utils;

namespace Perspective.Character.NPC.State
{
    public class NpcChaseState : NpcBaseState
    {
        private bool _isGettingDestroyed;

        public NpcChaseState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;

            _isGettingDestroyed = false;

            NpcController.SetEvent(NpcEvent.DisableEvent, NpcController);

            NavMeshUtils.Instance.SetDestinationNearest(NpcController.Agent,
                NpcController.ExitPoints.transform.GetChild(NpcController.chaseIndex).transform.position);

            NpcController.Agent.speed = NpcController.RunSpeed;

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