using System.Collections.Generic;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.room_storage.drone.states;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleColors.room_storage.drone
{
    public class DroneController : StateMachine, IPointerClickHandler
    {
        public readonly Queue<GameObject> boxesToLift = new();
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _kiste;
        [SerializeField] private GameObject _droneBody;
        [SerializeField] private GameObject[] _destinationPositions;
        [SerializeField] private GameObject _droneMenu;
        [SerializeField] private GameObject[] _dronePropeller;
        public AudioSource audioSource;
        public Vector3 targetPos;
        public Vector3 destinationPufferPos;
        private readonly float _smoothTime = 1f;
        private Vector3 _velocity = Vector3.zero;
        public Vector3 idlePosition;
        private float accelerationFactor = 0f;
        private float _propellerSpeed = 800f;
        public const string STATE_IDLE = "Idle";
        public const string STATE_MOVETOBOX = "MoveToBox";
        public const string STATE_LIFTBOX = "LiftBox";
        public const string STATE_MOVETODESTINATION = "MoveToDestination";
        public const string STATE_UNLOADING = "Unloading";
        public const string STATE_MOVETOIDLEPOSITION = "MoveToIdlePosition";

        private void Start()
        {
            ChangeState(new Idle(this));
        }

        private void OnEnable()
        {
            EventManager.BoxStored += HandleStoredBoxes;
            _droneMenu.SetActive(false);
            audioSource.volume = 0;
        }

        private void OnDisable()
        {
            EventManager.BoxStored -= HandleStoredBoxes;
        }

        private void OnBecameVisible()
        {
            audioSource.volume = 1;
        }

        private void OnBecameInvisible()
        {
            audioSource.volume = 0;
        }

        /// <summary>
        /// die drone muss IMMER erst an dem zielort angekommen sein ...
        /// am ende der methode wird dann das update des states aufgerufen 
        /// </summary>
        void Update()
        {
            var distance = Vector3.Distance(targetPos, transform.position);

            foreach (GameObject propeller in _dronePropeller)
            {
                propeller.transform.RotateAround(propeller.transform.position, Vector3.up,
                    Time.deltaTime * _propellerSpeed);
            }

            if (_currentState is not Idle && distance > .01f)
            {
                accelerationFactor = Mathf.Clamp01(accelerationFactor + Time.deltaTime / _smoothTime);

                transform.position =
                    Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, _smoothTime / accelerationFactor);

                var factor = ((distance / 50f) * accelerationFactor);
                audioSource.pitch = 1f + factor;
                _propellerSpeed = 800f + factor * 1000;

                // aus der methode raus ... bis drohne am ziel ist und dann die update des aktuellen states aufrufen
                return;
            }

            accelerationFactor = 0f;
            _propellerSpeed = 800f;

            // Update-Methode des aktuellen Zustands ausführen
            _currentState?.Update();
        }

        public void SetAnimation(string animationName)
        {
            _animator.Play(animationName);
        }

        private void HandleStoredBoxes(GameObject box)
        {
            boxesToLift.Enqueue(box);
            box.GetComponent<KisteController>().enabled = false;
        }

        public void SetKisteColor(int colorIndex)
        {
            var color = GameManager.Instance.GetColorForIndex(colorIndex);
            _kiste.GetComponent<Renderer>().material.color = color;
        }

        public void SetDroneColor(int colorIndex)
        {
            _droneBody.GetComponent<Renderer>().material.color =
                colorIndex == 0 ? Color.white : _kiste.GetComponent<Renderer>().material.color;
        }

        // called by animation event ( drone -> liftbox )
        public void OnLiftBoxAnimationEnd()
        {
            ChangeState(new MoveToDestination(this));
        }

        // called by animation event ( drone -> unloading)
        public void OnUnloadingAnimationEnd()
        {
            SetDroneColor(0);

            if (boxesToLift.Count > 0)
            {
                ChangeState(new MoveToBox(this));
                return;
            }

            ChangeState(new MoveToIdlePosition(this));
        }

        public void DetermineTargetPosition(int colorIndex)
        {
            destinationPufferPos = _destinationPositions[colorIndex - 1].transform.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
            _droneMenu.SetActive(true);
        }

        public void HideMenu()
        {
            CameraController.Instance.UnsetLockedTarget();
            _droneMenu.SetActive(false);
        }
    }
}