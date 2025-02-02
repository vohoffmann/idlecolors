using IdleColors.Globals;

namespace IdleColors.room_order.constructor.states
{
    public class Idle : State
    {
        public Idle(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.target = Owner.idlePosition.transform.position;
        }

        public override void Update()
        {
            ReachLocation();

            if (Owner.targets.Count != 0)
            {
                for (var i = 0; i < Owner.targets.Count; i++)
                {
                    var TargetInfo = Owner.targets[i];

                    // check if the color is in puffer 
                    if (GameManager.Instance.FinalColorCounts[TargetInfo.colorIndex + 1] == 0)
                        continue;

                    Owner.targetIndex = TargetInfo.colorIndex + 1;
                    Owner.target = TargetInfo.pufferPosition;
                    Owner.cubeTarget = TargetInfo.cubePosition;
                    Owner.targets.Remove(TargetInfo);

                    Owner.ChangeState(new MoveToPuffer(Owner));
                    break;
                }
            }
        }
    }
}