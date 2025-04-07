using IdleColors.Globals;
using UnityEngine;
using UnityEngine.UI;

namespace IdleColors.room_order.constructor.states
{
    public class Idle : State
    {
        public Idle(ConstructorController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            var pos = Owner.idlePosition.transform.position;
            Owner.target = new Vector3(pos.x, -29.8f, pos.z);
            Owner.audioSource.Play();
        }

        public override void Update()
        {
            if (ReachLocation())
            {
                Owner.audioSource.Stop();
            }


            if (Owner.holdConstructor)
                return;

            var missingColor = Color.black;

            var items     = Owner.targets.Count;
            var doneItems = 0;
            if (!Owner.jobDone)
            {
                for (var i = 0; i < Owner.targets.Count; i++)
                {
                    var TargetInfo = Owner.targets[i];

                    // already done ?
                    if (Owner.targets[i].done)
                    {
                        doneItems++;

                        if (items == doneItems)
                        {
                            Owner.jobDone                            = true;
                            GameManager.Instance.ImageOrderInProcess = false;
                            Owner.holdConstructor                    = true;
                            return;
                        }

                        continue;
                    }

                    // check if the color is in puffer 
                    var colorIdx = TargetInfo.colorIndex + 1;
                    if (GameManager.Instance.FinalColorCounts[colorIdx] == 0)
                    {
                        missingColor = GameManager.Instance.GetColorForIndex(colorIdx);
                        continue;
                    }

                    Owner.targetIndex = colorIdx;
                    Owner.target      = TargetInfo.pufferPosition;
                    Owner.cubeTarget  = TargetInfo.cubePosition;

                    //Owner.targets.Remove(TargetInfo);
                    Owner.targets[i].done = true;
                    Owner._missingColorImage.SetActive(false);
                    // Owner._missingColorText.SetActive(false);
                    Owner.ChangeState(new MoveToPuffer(Owner));
                    return;
                }

                // Owner._missingColorText.GetComponentInChildren<TextMeshProUGUI>().color = missingColor;
                Owner._missingColorImage.GetComponentInChildren<Image>().color = missingColor;
                Owner._missingColorImage.SetActive(true);
                // Owner._missingColorText.SetActive(true);
            }
        }
    }
}