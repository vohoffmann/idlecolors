using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.room_order.constructor.states
{
    public class Loading : State
    {
        private bool down;

        public Loading(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            down = true;
            Owner.audioSource.Stop();
        }

        public override void Update()
        {
            var kopfPosition = Owner._kopf.transform.position;
            if (down && Owner._kopf.transform.localPosition.y > -5.5f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y - Time.fixedDeltaTime * Owner.Speed, kopfPosition.z);
                return;
            }

            down = false;
            
            if (Owner._kopf.transform.localPosition.y < -1.5f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y + Time.fixedDeltaTime * Owner.Speed, kopfPosition.z);
                return;
            }

            GameManager.Instance.FinalColorCounts[Owner.targetIndex]--;
            Owner.ChangeState(new MoveToCube(Owner));
        }
    }
}