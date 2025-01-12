using UnityEngine;

namespace IdleColors.room_storage.drone.states
{
    public class MoveToBox : State
    {
        private GameObject box;

        public MoveToBox(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);

            box = Owner.boxesToLift.Dequeue();

            Owner.targetPos = box.transform.position;
            Owner.targetPos.y = Owner.targetPos.y + 3.85f;

            var colorIndex = box.GetComponent<KisteController>()._colorIndex;
            Owner.DetermineTargetPosition(colorIndex);
            Owner.SetKisteColor(colorIndex);
            Owner.SetAnimation(DroneController.STATE_MOVETOBOX);
        }

        public override void Update()
        {
            if (box) Object.Destroy(box);
            Owner.ChangeState(new LiftBox(Owner));
        }
    }
}