using System;
using IdleColors.Globals;
using IdleColors.room_mixing.pipeball;
using UnityEngine;

namespace IdleColors.room_storage
{
    public class CupController : MonoBehaviour
    {
        private int _amount;

        // private int _coins;
        public int _colorIndex;
        private bool _loadingPositionReached;
        private bool _readyToStore;
        private float _targetZPos;
        private bool used; // damit nicht die position korrigiert wird, wenn sie schon unterwegs ist 
        private bool _needUpdate = true;
        [SerializeField] private GameObject _cupLid;
        private bool _empty = false;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

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
            if (used) return;

            transform.position = new Vector3(11.16f, -30.70f, -41.97f);
        }

        private void OnTriggerEnter(Collider other)
        {
            var script = other.gameObject.GetComponent<PipeBallController>();
            script.Reset();
            used = true;

            // erstes triggerevent
            if (_amount == 0)
            {
                audioSource.volume = other.gameObject.GetComponent<Renderer>().isVisible ? 1f : 0;

                _colorIndex = script._colorIndex;
                _targetZPos = 6 + (1 - _colorIndex) * 3;

                var otherColor = GameManager.Instance.GetColorForIndex(_colorIndex);
                _cupLid.GetComponent<Renderer>().material.color = otherColor;

                // if (otherColor.r > 0)
                //     _coins += 5;
                // if (otherColor.g > 0)
                //     _coins += 10;
                // if (otherColor.b > 0)
                //     _coins += 20;
            }

            audioSource.Play();
            audioSource.pitch += .1f;

            _amount++;

            if (_amount == 5)
            {
                _amount = 0;

                InstantiatNewBox();
                _readyToStore = true;
            }
        }

        private void Update()
        {
            if (!_needUpdate)
            {
                // only check if the pot is fallen ... than mark as lost

                if (transform.position.y < -50)
                {
                    Destroy(gameObject);
                }

                if (_empty)
                {
                    transform.Translate(-1 * Time.deltaTime, 0, 0);
                }

                return;
            }

            if (!_loadingPositionReached)
            {
                if (transform.localPosition.z < 9.53f)
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
                    if (GetComponent<Rigidbody>() == null)
                    {
                        // var rigidBody = new Rigidbody();
                        // rigidBody.freezeRotation = true;
                        gameObject.AddComponent<Rigidbody>();
                    }

                    transform.Translate(-3 * Time.deltaTime, 0, 0);
                    return;
                }

                var pos = transform.localPosition;

                transform.localPosition = new Vector3(7f, pos.y, pos.z);
                _needUpdate = false;
                EventManager.CupStored?.Invoke(gameObject);
            }
        }

        public void Dispose()
        {
            GetComponent<Rigidbody>().mass = .5f;
            _cupLid.GetComponent<Renderer>().material.color = Color.white;
            _empty = true;
        }

        private void InstantiatNewBox()
        {
            var storageRoom = GameObject.Find("storage_room");

            var kiste = Instantiate(
                GameManager.Instance.cupBp,
                storageRoom.transform);

            kiste.transform.position = new Vector3(11.16f, -30.70f, -32.50f);
        }
    }
}