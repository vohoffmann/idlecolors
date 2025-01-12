namespace IdleColors.room_storage.drone.states
{
    public class UnloadBox : State
    {
        public UnloadBox(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);
            Owner.SetAnimation(DroneController.STATE_UNLOADING);
            Owner.targetPos = Owner.transform.position;
        }
    }
}