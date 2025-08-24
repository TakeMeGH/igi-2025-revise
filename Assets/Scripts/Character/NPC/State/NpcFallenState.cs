
using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcFallenState : NpcBaseState
    {

        private float _timeToRelease;
        private bool _isReleased;
        public NpcFallenState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = false;

            NpcController.Agent.enabled = false;

            _timeToRelease = 15f;
            _isReleased = false;
            
            NpcController.Animator.Play("Fallen");
        }

        public override void Update()
        {
            if (_isReleased) return;
            _timeToRelease -= Time.deltaTime;

            if (!(_timeToRelease <= 0f)) return;
            
            _isReleased = true;
            NpcController.SelfReleased();
        }

        public override void Exit()
        {
        }
    }
}