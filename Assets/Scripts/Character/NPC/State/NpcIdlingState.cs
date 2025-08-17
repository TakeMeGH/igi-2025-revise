using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcIdlingState : NpcBaseState
    {
        private float _idleDuration;
        public NpcIdlingState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _idleDuration = Random.Range(2f, 6f);
            
            NpcController.Animator.Play("Idle/Walk");
            NpcController.Animator.SetFloat(Speed, 0.0f);
        }
        
        public override void Update()
        {
            base.Update();
            
            if (_idleDuration > 0)
            {
                _idleDuration -= Time.deltaTime;
            }
            
            if (_idleDuration <= 0f)
            {
                NpcController.SwitchState(NpcController.NpcWalkingState);
            }
        }


    }
}