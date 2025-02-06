using IdleColors.hud;
using IdleColors.room_collect.collector;
using IdleColors.room_mixing.haxler;
using IdleColors.room_mixing.puffer;
using IdleColors.room_storage.drone;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace IdleColors.camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] public GameObject[] _roomPositions;
        [SerializeField] private CollectorMenuController collectorMenu;
        [SerializeField] private HaxlerMenuController haxlerMenu;
        [SerializeField] private PufferMenuController pufferMenu;
        [SerializeField] private GameObject droneMenu;
        [SerializeField] private GameObject _orderImagePanel;
        [SerializeField] private GameObject _bodyCamView;
        private Light _light;

        public static CameraController Instance;

        [SerializeField] private GameObject _lockedTarget;
        private int _currentSelection;
        private bool _isMoving;
        private readonly float _smoothTime = .5f;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _targetPos;

        [SerializeField] private Camera _camera;

        private void Awake()
        {
            Instance = this;
            _light = _camera.GetComponentInChildren<Light>();
            _bodyCamView.SetActive(false);
        }

        private void Start()
        {
            _camera.gameObject.SetActive(true);
            setTarget(_roomPositions[0]);
            _isMoving = true;
        }

        public void SetLockedTarget(CollectorController collectorScript)
        {
            UnsetLockedTarget();
            _lockedTarget = collectorScript.gameObject;
            collectorScript.SetBodyCam(true);
            _bodyCamView.SetActive(true);

            haxlerMenu.gameObject.SetActive(false);
            pufferMenu.gameObject.SetActive(false);
            droneMenu.SetActive(false);

            collectorMenu.SetCollector(collectorScript);
        }

        public void SetLockedTarget(HaxlerController haxlerScript)
        {
            UnsetLockedTarget();
            _lockedTarget = haxlerScript.gameObject;

            collectorMenu.gameObject.SetActive(false);
            pufferMenu.gameObject.SetActive(false);
            droneMenu.SetActive(false);

            haxlerMenu.SetHaxler(haxlerScript);
        }

        public void SetLockedTarget(DroneController droneScript)
        {
            UnsetLockedTarget();
            _lockedTarget = droneScript.gameObject;

            collectorMenu.gameObject.SetActive(false);
            haxlerMenu.gameObject.SetActive(false);
            pufferMenu.gameObject.SetActive(false);
        }

        public void SetLockedTarget(PufferController pufferScript)
        {
            UnsetLockedTarget();
            _lockedTarget = pufferScript.gameObject;

            collectorMenu.gameObject.SetActive(false);
            haxlerMenu.gameObject.SetActive(false);
            droneMenu.SetActive(false);

            pufferMenu.setPufferScript(pufferScript);
        }

        public void UnsetLockedTarget()
        {
            var script = _lockedTarget ? _lockedTarget.GetComponent<CollectorController>() : null;

            if (script != null)
            {
                script.SetBodyCam(false);
            }

            _lockedTarget = null;
            _bodyCamView.SetActive(false);

            haxlerMenu.gameObject.SetActive(false);
            collectorMenu.gameObject.SetActive(false);
            pufferMenu.gameObject.SetActive(false);
            droneMenu.SetActive(false);

            setTarget(_roomPositions[_currentSelection]);
        }

        public void MoveCamUp()
        {
            if (_lockedTarget)
                UnsetLockedTarget();

            MoveCam(false);
        }

        public void MoveCamDown()
        {
            if (_lockedTarget) UnsetLockedTarget();

            MoveCam(true);
        }

        public void ShowOrderImagePanel()
        {
            if (_lockedTarget) UnsetLockedTarget();

            // set to last ( oder) room
            SetCamToOrderroom();

            _orderImagePanel.SetActive(true);
        }

        public void SetCamToOrderroom()
        {
            _currentSelection = _roomPositions.Length - 1;
            setTarget(_roomPositions[_currentSelection]);
            _isMoving = true;
        }

        // true = down
        private void MoveCam(bool down)
        {
            if (down)
            {
                if (_currentSelection == _roomPositions.Length - 1)
                {
                    return;
                }

                _currentSelection++;
            }
            else
            {
                if (_currentSelection == 0)
                {
                    return;
                }

                _currentSelection--;
            }

            setTarget(_roomPositions[_currentSelection]);

            _isMoving = true;
        }

        void FixedUpdate()
        {
            if (!_isMoving)
            {
                if (_lockedTarget != null)
                {
                    setTarget(_lockedTarget);
                    _isMoving = true;
                }
            }
            else
            {
                if (_lockedTarget)
                {
                    setTarget(_lockedTarget);
                }
                else if (Vector3.Distance(_targetPos, _camera.transform.position) < .1f)
                {
                    _isMoving = false;
                    // _smoothTime = 1f;
                }

                _camera.transform.position =
                    Vector3.SmoothDamp(_camera.transform.position, _targetPos, ref _velocity, _smoothTime);
            }
        }

        private void setTarget(GameObject target)
        {
            var position = target.transform.position;

            _targetPos = new Vector3(
                position.x - 25,
                position.y + 40,
                position.z - 42
            );
        }

        public void SetLightIntensity(float value)
        {
            if (_light) _light.intensity = value;
        }
    }
}