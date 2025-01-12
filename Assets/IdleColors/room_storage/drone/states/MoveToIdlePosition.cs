namespace IdleColors.room_storage.drone.states
{
    public class MoveToIdlePosition : State
    {
        public MoveToIdlePosition(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);
            Owner.targetPos = Owner.idlePosition;
            Owner.SetAnimation(DroneController.STATE_MOVETOIDLEPOSITION);
        }

        public override void Update()
        {
            Owner.ChangeState(new Idle(Owner));
        }
    }
}