using IdleColors.Globals;

namespace IdleColors.room_order.constructor.states
{
    public class MoveToPuffer : State
    {
        public MoveToPuffer(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.audioSource.Play();
        }

        public override void Update()
        {
            if (ReachLocation())
            {
                Owner.ChangeState(new Loading(Owner));
            }
        }
    }
}