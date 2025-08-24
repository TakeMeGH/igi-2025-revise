using UnityEngine;

namespace Perspective.Character.NPC.State
{
    public class NpcTradingBlowsState : NpcBaseState
    {
        private int _punchLimit;
        private bool _isChase;
        private float _toChaseDuration;

        public NpcTradingBlowsState(NpcController npcController) : base(npcController)
        {
        }

        public override void Enter()
        {
            base.Enter();

            NpcController.Agent.isStopped = true;
            NpcController.Agent.speed = NpcController.RunSpeed;

            _isChase = false;
            _toChaseDuration = -1.0f;

            NpcController.Animator.Play("Intimidate");

            NpcController.isPunching = NpcController.MainFighter;
            if (NpcController.MainFighter)
            {
                _punchLimit = 3;
                NpcController.SetEventDetector(true);
                NpcController.OtherNpc.SetEventDetector(true);
            }
            else _punchLimit = -1;

            if (NpcController.isPunching) StartPunching();
        }

        public override void Update()
        {
            switch (_isChase)
            {
                case true when _toChaseDuration > 0:
                    if (NpcController.OtherNpc)
                        NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);
                    _toChaseDuration -= Time.deltaTime;
                    break;
                case true:
                    NpcController.SwitchState(NpcController.NpcChaseState);
                    break;
            }
        }

        public override void Exit()
        {
        }

        private void StartPunching()
        {
            if (NpcController.MainFighter && _punchLimit == 0)
            {
                NpcController.Animator.Play("Intimidate");
                _isChase = true;
                _toChaseDuration = 1.5f;
                NpcController.transform.LookAt(NpcController.OtherNpc.transform.position);

                NpcController.OtherNpc.SwitchState(NpcController.OtherNpc.NpcFleeState);
                NpcController.SetEventDetector(false);
                NpcController.OtherNpc.SetEventDetector(false);
                return;
            }

            NpcController.Animator.Play("Punching");
            NpcController.isPunching = true;
            _punchLimit--;
        }

        public override void OnAnimationExitEvent()
        {
            if (NpcController.isPunching)
            {
                NpcController.OtherNpc.Animator.Play("Punched");
                NpcController.isPunching = false;
            }
            else
            {
                StartPunching();
            }
        }
    }
}