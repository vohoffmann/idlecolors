using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IdleColors.room_mixing.haxler
{
    public class MoveHaxlerHammer : MonoBehaviour
    {
        private float _targetx_1;
        private float _targetx_2;
        private float targetPosX;
        private float startTime;
        private float duration = 5.0f;
        private bool  direction;

        private void Start()
        {
            var pos = transform.localPosition;
            _targetx_1 = pos.x;
            _targetx_2 = pos.x - .35f;
            targetPosX = _targetx_2;

            transform.localPosition = new Vector3(pos.x - Random.Range(.1f, .3f), pos.y, 0);

            enabled = false;
        }

        private void OnEnable()
        {
            startTime = Time.time;
        }

        void Update()
        {
            var   pos = transform.localPosition;
            float x   = (float)Math.Round(pos.x, 2);

            if ((x <= _targetx_2 && !direction) ||
                (x >= _targetx_1 - 0.01 && direction))
            {
                // if (!direction) parent.PlayHammerSound();
                direction  = !direction;
                startTime  = Time.time;
                duration   = direction ? 4f + Random.Range(1f, 2f) : .5f;
                targetPosX = direction ? _targetx_1 : _targetx_2;
            }

            x = Mathf.Lerp(pos.x, targetPosX, ((Time.time - startTime) / duration) * 130 * Time.deltaTime);

            transform.localPosition = new Vector3(x, pos.y, pos.z);
        }
    }
}