using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcIntimidationState : NpcBaseState
    {
        private float _defaultStopingDistance;
        private bool _intimidationStarted;
        private float _intimidationDuration;
    
        public NpcIntimidationState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;
            NpcController.Agent.speed = NpcController.RunSpeed;
            
            NpcController.Animator.Play("Idle/Walk");

            _defaultStopingDistance = NpcController.Agent.stoppingDistance;
            NpcController.Agent.stoppingDistance = NpcController.NearToBrawlDistance;

            _intimidationDuration = 4.0f;
        }

        public override void Update()
        {
            if (_intimidationStarted)
            {
                _intimidationDuration -= Time.deltaTime;
                if (_intimidationDuration > 0) return;
                NpcController.SwitchState(NpcController.NpcFightState);
            }
            else
            {
                RunToFight();
            }
        }
        
        private void RunToFight()
        {
            NpcController.Agent.SetDestination(NpcController.OtherNpc.transform.position);
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();

            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;

            NpcController.Agent.velocity = Vector3.zero;
            _intimidationStarted = true;
            NpcController.Animator.Play("Intimidate");
            NpcController.Agent.isStopped = true;
        }
        
        public override void Exit()
        {
            NpcController.Agent.stoppingDistance = _defaultStopingDistance;
        }
    }
}