using System.Collections;
using System.Collections.Generic;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.hud;
using IdleColors.room_storage.drone.states;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.room_storage.drone
{
    public class DroneController : StateMachine, IPointerClickHandler, IUnityAdsShowListener
    {
        public readonly          Queue<GameObject> cupsToLift = new();
        [SerializeField] private Animator          _animator;
        [SerializeField] private GameObject        _droneBody;
        [SerializeField] private GameObject[]      _destinationPositions;
        [SerializeField] private GameObject        _droneMenu;

        [SerializeField] private GameObject[] _dronePropeller;

        [SerializeField] private SO_Int _droneSpeed;
        // [SerializeField] private ParticleSystem _coinPartikel;

        [SerializeField] private GameObject      _noMoreUpdatesButtonText;
        [SerializeField] private TextMeshProUGUI _speedStatusText;

        [SerializeField] private GameObject _speedButtonCanvas;

        private                  Button          _speedButton;
        [SerializeField] private Text            _speedButtonText;
        [SerializeField] private TextMeshProUGUI _speedUpdateInfoText;
        [SerializeField] private Button          _speedbyadsbutton;
        [SerializeField] private AudioSource     _updateSound;
        private                  Color           _color;
        private                  int             _colorIndex;
        public                   AudioSource     audioSource;
        public                   Vector3         targetPos;
        public                   GameObject      cup;
        public                   Vector3         destinationPufferPos;
        private                  Vector3         _velocity = Vector3.zero;
        public                   Vector3         idlePosition;
        private                  float           accelerationFactor;
        private                  float           _propellerSpeed          = 800f;
        public const             string          STATE_IDLE               = "Idle";
        public const             string          STATE_MOVETOCUP          = "MoveToBox";
        public const             string          STATE_LIFTBOX            = "LiftBox";
        public const             string          STATE_MOVETODESTINATION  = "MoveToDestination";
        public const             string          STATE_UNLOADING          = "Unloading";
        public const             string          STATE_MOVETOIDLEPOSITION = "MoveToIdlePosition";

        private void Awake()
        {
            if (_speedButton == null)
            {
                _speedButton = _speedButtonText.GetComponent<Button>();
            }
        }

        private void Start()
        {
            ChangeState(new Idle(this));
            var storageRoom = GameObject.Find("storage_room");

            var cupInstance = Instantiate(
                GameManager.Instance.cupBp,
                storageRoom.transform);
            cupInstance.transform.position = new Vector3(11.16f, -30.70f, -32.50f);
        }

        private void OnEnable()
        {
            EventManager.CupStored += HandleStoredBoxes;
            _droneMenu.SetActive(false);
            audioSource.volume      =  0;
            EventManager.CoinsAdded += updateMenuView;
        }

        private void OnDisable()
        {
            EventManager.CupStored  -= HandleStoredBoxes;
            EventManager.CoinsAdded -= updateMenuView;
        }

        private void OnBecameVisible()
        {
            StartCoroutine(AktivateSound());
        }

        IEnumerator AktivateSound()
        {
            yield return new WaitForSeconds(2f);
            if (gameObject.GetComponent<Renderer>().isVisible)
                audioSource.volume = 1;
        }

        private void OnBecameInvisible()
        {
            StopCoroutine(AktivateSound());
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
                var droneSpeedFactor = (1.7f - _droneSpeed.value * .1f);
                accelerationFactor =
                    Mathf.Clamp01(accelerationFactor + Time.deltaTime / droneSpeedFactor);

                transform.position =
                    Vector3.SmoothDamp(transform.position,
                        targetPos,
                        ref _velocity,
                        droneSpeedFactor / accelerationFactor);
                var factor = ((distance / 50f) * accelerationFactor);
                audioSource.pitch = 1f + factor;
                _propellerSpeed   = 800f + factor * 1000;

                // aus der methode raus ... bis drohne am ziel ist und dann die update des aktuellen states aufrufen
                return;
            }

            accelerationFactor = 0f;
            _propellerSpeed    = 800f;

            // Update-Methode des aktuellen Zustands ausführen
            _currentState?.Update();
        }

        public void SetAnimation(string animationName)
        {
            _animator.Play(animationName);
        }

        private void HandleStoredBoxes(GameObject box)
        {
            cupsToLift.Enqueue(box);
        }

        public void SetCupColor(int colorIndex)
        {
            _color = GameManager.Instance.GetColorForIndex(colorIndex);
        }

        public void SetDroneColor(int colorIndex)
        {
            _droneBody.GetComponent<Renderer>().material.color =
                colorIndex == 0 ? Color.white : _color;
        }

        // called by animation event ( drone -> liftbox )
        public void OnLiftBoxAnimationEnd()
        {
            ChangeState(new MoveToDestination(this));
            cup.GetComponent<CupController>().Dispose();
        }

        // called by animation event ( drone -> unloading)
        public void OnUnloadingAnimationEnd()
        {
            var coinTextPos = new Vector3(destinationPufferPos.x, destinationPufferPos.y, destinationPufferPos.z - 3);
            GameManager.Instance.AddCoins(OrderPanelController.CoinValues[_colorIndex], coinTextPos);
            GameManager.Instance.FinalColorCounts[_colorIndex] += 10;

            // _coinPartikel.Play();
            SetDroneColor(0);
            ChangeState(new MoveToIdlePosition(this));
        }

        public void DetermineTargetPosition(int colorIndex)
        {
            _colorIndex          = colorIndex;
            destinationPufferPos = _destinationPositions[colorIndex - 1].transform.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
            _droneMenu.SetActive(true);

            updateMenuView();
        }

        private void updateMenuView()
        {
            if (_droneSpeed.value < GLOB.DRONE_SPEED_MAX)
            {
                _speedButtonCanvas.SetActive(true);
                _noMoreUpdatesButtonText.SetActive(false);

                _speedUpdateInfoText.text = $"{_droneSpeed.value} -> {_droneSpeed.value + 1}";

                var upgradeCost =
                    Mathf.RoundToInt(GLOB.DRONE_SPEED_BASE_PRICE * Mathf.Pow(1.5f, _droneSpeed.value - 1));
                _speedButtonText.text =
                    "" + upgradeCost;
                _speedButton.interactable = GameManager.Instance.GetCoins() >=
                                            upgradeCost;
                if (Advertisement.isInitialized && GameManager.Instance.AdsRewardedLoaded)
                {
                    _speedbyadsbutton.gameObject.SetActive(true);
                }
                else
                {
                    _speedbyadsbutton.gameObject.SetActive(false);
                }
            }
            else
            {
                _speedButtonCanvas.SetActive(false);
                _noMoreUpdatesButtonText.SetActive(true);
                _speedStatusText.text = "" + _droneSpeed.value;
            }
        }

        public void ShowAdsToUpgradeSpeed()
        {
            if (!Advertisement.isInitialized || !GameManager.Instance.AdsRewardedLoaded)
            {
                return;
            }

            Time.timeScale = 0;

            GameManager.MenuBlocked = true;

            Advertisement.Show(GameManager.REWARDED_ANDROID, this);
        }

        public void UpdateSpeed(bool subCoins = false)
        {
            if (_droneSpeed.value < GLOB.DRONE_SPEED_MAX)
            {
                if (subCoins)
                {
                    GameManager.Instance.SubCoins(
                        Mathf.RoundToInt(GLOB.DRONE_SPEED_BASE_PRICE * Mathf.Pow(1.5f, _droneSpeed.value - 1)));
                }

                _droneSpeed.value += 1;
                _updateSound.Play();
                updateMenuView();
            }
        }

        public void HideMenu()
        {
            CameraController.Instance.UnsetLockedTarget();
            _droneMenu.SetActive(false);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Time.timeScale = 1;

            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                UpdateSpeed();
            }

            GameManager.MenuBlocked = false;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Time.timeScale = 1;

            Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            // Debug.Log("Unity Ads Rewarded Ad Started");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            // Debug.Log("Unity Ads Rewarded Ad Clicked");
        }
    }
}