using IdleColors.Globals;
using TMPro;
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

            if (Owner.targets.Count != 0)
            {
                for (var i = 0; i < Owner.targets.Count; i++)
                {
                    var TargetInfo = Owner.targets[i];

                    // check if the color is in puffer 
                    var colorIdx = TargetInfo.colorIndex + 1;
                    if (GameManager.Instance.FinalColorCounts[colorIdx] == 0)
                    {
                        missingColor = GameManager.Instance.GetColorForIndex(colorIdx);
                        continue;
                    }

                    Owner.targetIndex = colorIdx;
                    Owner.target = TargetInfo.pufferPosition;
                    Owner.cubeTarget = TargetInfo.cubePosition;
                    Owner.targets.Remove(TargetInfo);
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