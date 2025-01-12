namespace IdleColors.room_storage.drone.states
{
    public class MoveToDestination : State
    {
        public MoveToDestination(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);
            Owner.SetAnimation(DroneController.STATE_MOVETODESTINATION);
            Owner.targetPos = Owner.destinationPufferPos;
        }

        public override void Update()
        {
            Owner.ChangeState(new UnloadBox(Owner));
        }
    }
}