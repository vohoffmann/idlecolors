using System;
using IdleColors.Globals;
using IdleColors.room_mixing.pipeball;
using UnityEngine;

namespace IdleColors.room_storage
{
    public class KisteController : MonoBehaviour
    {
        private int _amount;
        private int _coins;
        public int _colorIndex;
        private bool _loadingPositionReached;
        private bool _readyToStore;
        private float _targetZPos;

        private void OnEnable()
        {
            EventManager.SetBoxPosition += SetBoxPosition;
        }

        private void OnDisable()
        {
            EventManager.SetBoxPosition -= SetBoxPosition;
        }

        private void SetBoxPosition()
        {
            transform.position = new Vector3(11.18f, -29.75f, -41.76f);
        }

        private void OnTriggerEnter(Collider other)
        {
            var script = other.gameObject.GetComponent<PipeBallController>();
            script.Reset();

            // erstes triggerevent
            if (_amount == 0)
            {
                _colorIndex = script._colorIndex;
                _targetZPos = 6 + (1 - _colorIndex) * 3;

                var otherColor = GameManager.Instance.GetColorForIndex(_colorIndex);
                gameObject.GetComponent<Renderer>().material.color = otherColor;

                if (otherColor.r > 0)
                    _coins += 5;
                if (otherColor.g > 0)
                    _coins += 10;
                if (otherColor.b > 0)
                    _coins += 20;
            }

            _amount++;
            if (_amount == 5)
            {
                GameManager.Instance.AddCoins(_coins);

                _amount = 0;
                _coins = 0;

                print("#####  5 mal trigger ... bereit los zu fahren");
                print("#####  neue box instantieren ...");

                InstantiatNewBox();
                _readyToStore = true;
            }
        }

        private void Update()
        {
            if (!_loadingPositionReached)
            {
                if (transform.localPosition.z < 9.8f)
                {
                    _loadingPositionReached = true;
                }

                transform.Translate(0, 0, -3 * Time.deltaTime);
            }

            if (_readyToStore)
            {
                if (transform.localPosition.z > _targetZPos)
                {
                    transform.Translate(0, 0, -3 * Time.deltaTime);
                    return;
                }

                if (transform.localPosition.x > 7f)
                {
                    transform.Translate(-3 * Time.deltaTime, 0, 0);
                    return;
                }

                // TODO: kiste steht am endgültigem platz ... kann also abgeholt werden
                EventManager.BoxStored?.Invoke(this.gameObject);
            }
        }

        private void InstantiatNewBox()
        {
            var storageRoom = GameObject.Find("storage_room");

            var kiste = Instantiate(
                GameManager.Instance.kisteBp,
                storageRoom.transform);
            kiste.transform.position = new Vector3(11.18f, -29.75f, -32.50f);

            // kiste.transform.position = new Vector3(11.18f, 1.15f, 19f);

            print("kiste.transform.position : " + kiste.transform.position);
        }
    }
}