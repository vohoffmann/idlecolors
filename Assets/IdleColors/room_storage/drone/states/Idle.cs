namespace IdleColors.room_storage.drone.states
{
    public class Idle : State
    {
        public Idle(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);
            Owner.SetAnimation(DroneController.STATE_IDLE);
            Owner.SetDroneColor(0);
            Owner.idlePosition = Owner.transform.position;
            Owner.targetPos = Owner.transform.position;
        }

        public override void Update()
        {
            if (Owner.boxesToLift.Count > 0)
            {
                Owner.ChangeState(new MoveToBox(Owner));
            }
        }
    }
}