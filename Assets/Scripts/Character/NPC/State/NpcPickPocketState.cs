using Perspective.Utils;

namespace Perspective.Character.NPC.State
{
    public class NpcPickPocketState : NpcBaseState
    {
        private float _defaultStopingDistance;
        private bool _stealCompleted;

        public NpcPickPocketState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;

            NpcController.Animator.Play("Idle/Walk");

            _defaultStopingDistance = NpcController.Agent.stoppingDistance;
            NpcController.Agent.stoppingDistance = NpcController.NearToStealDistance;
        }

        public override void Update()
        {
            if (_stealCompleted)
            {
                NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
                UpdateRotation();
                if (NpcController.Agent.pathPending ||
                    !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;
                NpcController.SelfDestroy();
            }
            else
            {
                WalkToConversation();
            }
        }

        private void WalkToConversation()
        {
            NpcController.Agent.SetDestination(NpcController.OtherNpc.transform.position);
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();

            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;

            NpcController.Animator.Play("Steal");
            NpcController.Agent.isStopped = true;
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcIdlingState);
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
            NpcController.ResetEvent();
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }

        public override void OnAnimationExitEvent()
        {
            _stealCompleted = true;
            
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcYellState);

            NavMeshUtils.SetDestinationNearest(NpcController.Agent,
                NpcController.ExitPoints.transform.GetChild(0).transform.position);

            NpcController.Agent.isStopped = false;
            NpcController.Agent.speed = NpcController.RunSpeed;
            NpcController.Animator.Play("Idle/Walk");
        }
    }
}