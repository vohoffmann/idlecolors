using UnityEngine;

namespace IdleColors.room_storage.drone.states
{
    public class MoveToCup : State
    {
        private GameObject _cup;

        public MoveToCup(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            _cup = Owner.cupsToLift.Dequeue();
            if(!_cup)
            {
                Debug.Log("MoveToCup State ... cup was deleted already ... ");
                Owner.ChangeState(new MoveToIdlePosition(Owner));
                return;
            }
            _cup.GetComponent<Rigidbody>().mass = 1000f;
            Owner.cup = _cup;
            
            Owner.targetPos = _cup.transform.position;
            Owner.targetPos.y += 3.85f;

            var colorIndex = _cup.GetComponent<CupController>()._colorIndex;
            Owner.DetermineTargetPosition(colorIndex);
            Owner.SetKisteColor(colorIndex);
            Owner.SetAnimation(DroneController.STATE_MOVETOCUP);
        }

        public override void Update()
        {
            Owner.ChangeState(new LiftBox(Owner));
        }
    }
}