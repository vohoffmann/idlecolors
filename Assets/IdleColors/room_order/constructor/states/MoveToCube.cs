namespace IdleColors.room_order.constructor.states
{
    public class MoveToCube : State
    {
        public MoveToCube(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.target = Owner.cubeTarget;
            Owner.audioSource.Play();
        }

        public override void Update()
        {
            if (ReachLocation())
            {
                Owner.ChangeState(new Unloading(Owner));
            }
        }
    }
}