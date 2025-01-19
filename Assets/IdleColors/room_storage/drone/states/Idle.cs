using UnityEngine;

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
            if (Owner.cupsToLift.Count > 0)
            {
                // if the cup is already destroyed than remove from queue
                try
                {
                    var cup = Owner.cupsToLift.Peek();
                    if (!cup)
                    {
                        throw new MissingReferenceException("object was deleted already");
                    }

                    // check if cup is not pushed by another already ...
                    if (cup.transform.position.x < 6.7f)
                    {
                        throw new MissingReferenceException("cup was moved by another already");
                    }

                    Owner.ChangeState(new MoveToCup(Owner));
                }
                catch (MissingReferenceException e)
                {
                    Debug.Log("Idle State : " + e.Message);

                    Owner.cupsToLift.Dequeue();
                }
            }
        }
    }
}