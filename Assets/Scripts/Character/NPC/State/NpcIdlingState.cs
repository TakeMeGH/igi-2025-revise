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

            if (NpcController.NpcType == NpcType.Thief || NpcController.NpcType == NpcType.ThugsWalkAround)
            {
                NpcController.Animator.Play("Smoke");
            }
            else NpcController.Animator.Play("Idle/Walk");

            NpcController.Animator.SetFloat(Speed, 0.0f);
        }

        public override void Update()
        {
            base.Update();
            if (NpcController.NpcType == NpcType.Merchant)
            {
                if (NpcController.OtherNpc)
                    NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);
                return;
            }

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