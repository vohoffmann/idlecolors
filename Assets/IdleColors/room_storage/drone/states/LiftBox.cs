namespace IdleColors.room_storage.drone.states
{
    public class LiftBox : State

    {
        public LiftBox(DroneController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // GameManager.Log("Entering State : " + this);
            Owner.SetAnimation(DroneController.STATE_LIFTBOX);
            Owner.targetPos = Owner.transform.position;
            
            // wenn die kiste aufgeladen wurde, wird per animationEvent im dronenController auf den nächten state gewechselt ( moveToDestination)
        }
    }
}