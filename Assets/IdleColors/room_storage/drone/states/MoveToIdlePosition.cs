using UnityEngine;

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
            
            while (Owner.cupsToLift.Count > 0)
            {
                // if the cup is already destroyed than remove from queue
                try
                {
                    var cup = Owner.cupsToLift.Peek();
                    
                    if(!cup)
                    {
                        throw new MissingReferenceException("cup was deleted already ");
                    }

                    // check if cup is not pushed by another already ...
                    if (cup.transform.position.x < 6.7f)
                    {
                        throw new MissingReferenceException("cup was moved by another already");
                    }
                    
                    Owner.ChangeState(new MoveToCup(Owner));
                    break;
                }
                catch (MissingReferenceException e)
                {
                    Debug.Log("MoveToIdlePosition State : " + e.Message);
                    
                    Owner.cupsToLift.Dequeue();
                }
            }
        }

        public override void Update()
        {
            Owner.ChangeState(new Idle(Owner));
        }
    }
}