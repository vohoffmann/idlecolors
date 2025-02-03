using UnityEngine;

namespace IdleColors.room_order.constructor.states
{
    public abstract class State
    {
        protected readonly ConstructorController Owner;

        protected State(ConstructorController owner)
        {
            Owner = owner;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }
        
        

        protected bool ReachLocation()
        {
            var deltaX = Owner.target.x - Owner.transform.position.x;
            var deltaZ = Owner.target.z - Owner.transform.position.z;


            if (Mathf.Abs(deltaX) > 0.1 || Mathf.Abs(deltaZ) > 0.1)
            {
                var deltaSpeed = Owner.Speed * Time.deltaTime;
                var newPosition = Owner.transform.position;

                if (Mathf.Abs(deltaX) > 0.1) newPosition.x += deltaX < 0 ? -deltaSpeed : deltaSpeed;

                if (Mathf.Abs(deltaZ) > 0.1) newPosition.z += deltaZ < 0 ? -deltaSpeed : deltaSpeed;

                Owner.transform.position = newPosition;
                return false;
            }

            return true;
        }
    }
}