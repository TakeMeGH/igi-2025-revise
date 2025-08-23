using Perspective.Utils;

namespace Perspective.Character.NPC.State
{
    public class NpcPickPocketState : NpcBaseState
    {
        private float _defaultStopingDistance;

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
            WalkToConversation();
        }

        private void WalkToConversation()
        {
            NpcController.Agent.SetDestination(NpcController.OtherNpc.transform.position);
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();

            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;

            NpcController.OtherNpc.ForcedSetEvent(NpcEvent.PickPocket, NpcController);
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcIdlingState);
            
            NpcController.SetEventDetector(true);
            NpcController.OtherNpc.SetEventDetector(true);

            NpcController.Animator.Play("Steal");
            NpcController.Agent.isStopped = true;
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
            NpcController.ResetEvent();
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }

        public override void OnAnimationExitEvent()
        {
            NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcYellState);
            NpcController.SwitchState(NpcController.NpcFleeState);
            
            // NpcController.SetEventDetector(false);
            // NpcController.OtherNpc.SetEventDetector(false);

        }
    }
}