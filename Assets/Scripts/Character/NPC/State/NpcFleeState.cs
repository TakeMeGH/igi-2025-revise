using System.Collections.Generic;
using Perspective.Utils;
using UnityEngine;

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

            PickExitPoint();

            NpcController.Agent.speed = NpcController.RunSpeed;

            _isGettingDestroyed = false;

            NpcController.Animator.Play(NpcController.CurrentEvent == NpcEvent.PoliceKidnapCivilian
                ? "RunBopong"
                : "Idle/Walk");
        }

        public override void Update()
        {
            NpcController.Animator.SetFloat(Speed, NpcController.Agent.velocity.magnitude);
            UpdateRotation();
            if (NpcController.Agent.pathPending ||
                !(NpcController.Agent.remainingDistance <= NpcController.Agent.stoppingDistance)) return;
            if (_isGettingDestroyed) return;

            _isGettingDestroyed = true;
            if (NpcController.CurrentEvent == NpcEvent.PoliceKidnapCivilian) NpcController.OtherNpc.SelfDestroy();
            NpcController.SelfDestroy();
        }

        public override void Exit()
        {
            NpcController.Agent.isStopped = true;
        }

        private void PickExitPoint()
        {
            var exitPoints = NpcController.ExitPoints.transform;
            var validExits = new List<int>();

            for (var i = 0; i < exitPoints.childCount; i++)
            {
                var exit = exitPoints.GetChild(i);

                var myDistance = Vector3.Distance(NpcController.transform.position, exit.position);
                var otherDistance = Vector3.Distance(NpcController.OtherNpc.transform.position,
                    exit.position);

                if (myDistance < otherDistance)
                {
                    validExits.Add(i);
                }
            }

            if (validExits.Count > 0)
            {
                var chosenExitIndex = validExits[Random.Range(0, validExits.Count)];
                var chosenExit = exitPoints.GetChild(chosenExitIndex);
                NpcController.OtherNpc.chaseIndex = chosenExitIndex;

                NavMeshUtils.Instance.SetDestinationNearest(NpcController.Agent, chosenExit.position);
            }
            else
            {
                Debug.LogWarning("⚠️ No valid exit found where this NPC is closer than the other!");

                var chosenExit = exitPoints.GetChild(0);
                NpcController.OtherNpc.chaseIndex = 0;

                NavMeshUtils.Instance.SetDestinationNearest(NpcController.Agent, chosenExit.position);
            }
        }
    }
}