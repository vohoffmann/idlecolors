namespace IdleColors.room_storage.drone.states
{
    public abstract class State
    {
        protected readonly DroneController Owner;

        protected State(DroneController owner)
        {
            Owner = owner;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }
    }
}