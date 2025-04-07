using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.room_mixing.pipeball
{
    public class PipeBallController : MonoBehaviour
    {
        [HideInInspector] public GameObject[] wayPoints;

        public        int  _colorIndex;
        public        bool active;
        private       int  _step;
        private const int  Speed = 5;

        private void FixedUpdate()
        {
            if (!active)
                return;

            var pos       = transform.position;
            var targetPos = wayPoints[_step].transform.position;
            var deltaTime = Speed * Time.fixedDeltaTime;

            if (Vector3.Distance(pos, targetPos) > 0.1f)
            {
                var newX = pos.x;
                var newY = pos.y;
                var newZ = pos.z;

                if (Mathf.Abs(newX - targetPos.x) > 0.1f)
                {
                    newX = pos.x < targetPos.x ? pos.x + deltaTime : pos.x - deltaTime;
                }

                if (Mathf.Abs(newY - targetPos.y) > 0.1f)
                {
                    newY = pos.y < targetPos.y ? pos.y + deltaTime : pos.y - deltaTime;
                }

                if (Mathf.Abs(newZ - targetPos.z) > 0.1f)
                {
                    newZ = pos.z < targetPos.z ? pos.z + deltaTime : pos.z - deltaTime;
                }

                transform.position = new Vector3(newX, newY, newZ);
            }
            else
            {
                _step++;
                if (_step >= wayPoints.Length)
                {
                    Reset();
                    GameManager.Instance.mixerController.ProcessLogic(gameObject);
                }
            }
        }

        public void Activate()
        {
            active = true;
        }

        public void Reset()
        {
            active             = false;
            transform.position = wayPoints[0].transform.position;
            _step              = 1;
        }
    }
}