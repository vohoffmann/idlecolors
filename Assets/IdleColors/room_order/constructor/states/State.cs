using UnityEngine;

namespace IdleColors.room_order.constructor.states
{
    public abstract class State
    {
        protected readonly ConstructorController Owner;
        private float accelerationFactor = 0f;
        private Vector3 _velocity = Vector3.zero;
        private readonly float _baseSpeed = 17.6f;

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
            var distance = Vector3.Distance(Owner.target, Owner.transform.position);

            if (distance > .01f)
            {
                accelerationFactor =
                    Mathf.Clamp01(accelerationFactor +
                                  Time.fixedDeltaTime / (2 - (Owner.Speed.value * 0.1f + _baseSpeed) * .1f));

                Owner.transform.position =
                    Vector3.SmoothDamp(Owner.transform.position,
                        Owner.target,
                        ref _velocity,
                        (2 - (Owner.Speed.value * 0.1f + _baseSpeed) * .1f) / accelerationFactor);
                return false;
            }

            accelerationFactor = 0f;

            return true;
        }
    }
}