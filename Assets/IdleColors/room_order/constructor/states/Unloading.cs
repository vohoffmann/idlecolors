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
            Owner.audioSource.Stop();
        }

        public override void Update()
        {
            var kopfPosition = Owner._kopf.transform.position;
            if (down && Owner._kopf.transform.localPosition.y > -11.0f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y - Time.fixedDeltaTime * (Owner.Speed.value * 0.1f + 18), kopfPosition.z);
                return;
            }

            down = false;

            if (Owner._kopf.transform.localPosition.y < -1.5f)
            {
                Owner._kopf.transform.position = new Vector3(kopfPosition.x,
                    kopfPosition.y + Time.fixedDeltaTime * (Owner.Speed.value * 0.1f + 18), kopfPosition.z);
                return;
            }

            // Debug.Log($"Owner.targetIndex : {Owner.targetIndex}");
            Owner.imageColors[Owner.targetIndex - 1] -= 1;
            Owner.UpdateStatText();

            Owner.ChangeState(new Idle(Owner));
        }

        // public override void Exit()
        // {
        //     if (Owner.jobDone)
        //     {
        //         GameManager.Instance.ImageOrderInProcess = false;
        //         Owner.holdConstructor = true;
        //     }
        // }
    }
}