using System.Collections;
using System.Collections.Generic;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.helper;
using IdleColors.room_mixing.haxler;
using IdleColors.room_mixing.pipeball;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleColors.room_mixing.puffer
{
    public class PufferController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private HaxlerController haxlerControler;
        [SerializeField] private SO_Int _level;
        [SerializeField] private GameObject _amountIndicator;
        [SerializeField] private SO_Int _minerals;
        [SerializeField] public TextMeshProUGUI mineralsAmountText;
        [SerializeField] private GameObject _pipeBall;
        [SerializeField] private GameObject[] _wayPoints;
        private float _amountIndicator_Y;
        private float _pufferFillIndicatorStep;
        private int _pufferMaxValue;
        public int costFactor;
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _upgradeSound;
        [SerializeField] private AudioClip _dropInSound;
        private int _reservedMinerals;
        private List<GameObject> _pipeBalls;


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = 0;

            _amountIndicator_Y = _amountIndicator.transform.position.y;

            _pipeBalls = new List<GameObject>();

            for (int i = 0; i < 25; i++)
            {
                var pb = Instantiate(_pipeBall, _wayPoints[0].transform, true);

                pb.GetComponent<PipeBallController>().wayPoints = _wayPoints;
                pb.GetComponent<PipeBallController>().Reset();
                _pipeBalls.Add(pb);
            }
        }

        private void OnBecameVisible()
        {
            _audioSource.volume = 1;
            SetDisplay();
        }

        private void OnBecameInvisible()
        {
            _audioSource.volume = 0;
        }

        public void TakeInitValues()
        {
            _pufferMaxValue = 24 * _level.value;
            _pufferFillIndicatorStep = 5.76f / _pufferMaxValue;

            SetDisplay();
        }

        private void SetDisplay()
        {
            SetIndicatorPosition();
            haxlerControler.SetPufferFilled(_minerals.value >= _pufferMaxValue);
        }

        public int GetAvailableMinerals()
        {
            return _minerals.value - _reservedMinerals;
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
            _audioSource.PlayOneShot(_dropInSound);

            _minerals.value++;

            haxlerControler.SetPufferFilled(_minerals.value >= _pufferMaxValue);

            SetIndicatorPosition();
            GameManager.Instance.AddCoins(1);
        }

        private void SetIndicatorPosition()
        {
            var indicatorPosition = _amountIndicator.transform.position;
            _amountIndicator.transform.position = new Vector3(indicatorPosition.x,
                _amountIndicator_Y + _minerals.value * _pufferFillIndicatorStep,
                indicatorPosition.z);

            // GameManager.Log("_amountIndicator_Y : " + _amountIndicator_Y);
            // GameManager.Log("_minerals.value : " + _minerals.value);
            // GameManager.Log("_pufferFillIndicatorStep : " + _pufferFillIndicatorStep);
        }

        public void OrderMinerals(int amount)
        {
            // FIXME : CHECK THIS !!!
            _reservedMinerals = amount;
            StartCoroutine(nameof(RemoveMinerals));
        }

        private IEnumerator RemoveMinerals()
        {
            int loopSize = _reservedMinerals;

            for (int i = 0; i < loopSize; i++)
            {
                yield return Helper.GetWait(.15f);

                foreach (var pb in _pipeBalls)
                {
                    var script = pb.GetComponent<PipeBallController>();
                    if (!script.active)
                    {
                        script.Activate();
                        break;
                    }
                }

                _minerals.value--;
                _reservedMinerals--;

                haxlerControler.SetPufferFilled(false);

                var transformPosition = _amountIndicator.transform.position;
                _amountIndicator.transform.position = new Vector3(transformPosition.x,
                    _amountIndicator_Y + _minerals.value * _pufferFillIndicatorStep, transformPosition.z);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
        }

        public void UpgradeCapacity()
        {
            GameManager.Instance.SubCoins(costFactor * GLOB.PUFFER_CAPACITY_BASE_PRICE * _level.value);
            _level.value += 1;
            haxlerControler.SetPufferFilled(false);
            _audioSource.PlayOneShot(_upgradeSound);
            TakeInitValues();
        }

        public int GetLevel()
        {
            return _level.value;
        }
    }
}