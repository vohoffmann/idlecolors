using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.room_order.constructor.states
{
    public class Unloading : State
    {
        private bool down;

        public Unloading(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            down = true;
        }

        public override void Update()
        {
            var kopfPosition = Owner._kopf.transform.position;
            if (down && Owner._kopf.transform.localPosition.y > -11.0f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y - Time.deltaTime * Owner.Speed, kopfPosition.z);
                return;
            }
            
            down = false;

            if (Owner._kopf.transform.localPosition.y < -1.5f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y + Time.deltaTime * Owner.Speed, kopfPosition.z);
                return;
            }

            
            Owner.ChangeState(new Idle(Owner));
        }

        public override void Exit()
        {
            if(Owner.targets.Count == 0)
            {
                GameManager.Instance.ImageOrderInProcess = false;
                Owner.holdConstructor = true;
            }
        }
    }
}