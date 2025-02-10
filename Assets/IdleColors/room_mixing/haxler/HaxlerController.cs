using System;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.room_mixing.haxler
{
    public class HaxlerController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioClip _upgradeSound;
        [SerializeField] private AudioClip _dropInSound;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI mineralsAmountText;
        [SerializeField] private SO_Int _speedLevel;
        [SerializeField] private GameObject chrushedSpawnPoint;
        [SerializeField] private GameObject chrushedMineralBP;
        private AudioSource _audioSource;
        private MoveHaxlerHammer[] hammerScripts;
        public float costFactor;
        private float sliderValue;
        private float startTime;
        private bool crushing;
        private bool pufferFull;
        public SO_Int _minerals;

        [SerializeField] private Transform _tmpMineralsContainer;
        // private  Stopwatch stopwatch = new();

        private void Awake()
        {
            hammerScripts = GetComponentsInChildren<MoveHaxlerHammer>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = 0;
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
            SetDisplay();
        }

        public void SetDisplay()
        {
            if (_minerals.value > 0)
            {
                mineralsAmountText.gameObject.SetActive(true);
                mineralsAmountText.text = "" + _minerals.value;
            }
            else
            {
                mineralsAmountText.gameObject.SetActive(false);
                mineralsAmountText.text = "" + _minerals.value;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
            _audioSource.PlayOneShot(_dropInSound);
            _minerals.value++;
            SetDisplay();
        }

        private void Update()
        {
            slider.gameObject.SetActive(sliderValue is < 99 and > 1);

            // Nur weiter, wenn material im mixer ODER puffer für Auswurf nicht voll
            if (_minerals.value == 0 || pufferFull)
            {
                setStatus(false);
                return;
            }

            if (!crushing)
            {
                // stopwatch.Restart();
                startTime = Time.time;
                setStatus(true);
            }

            // slider animieren
            sliderValue = Mathf.Lerp(0, 100, (Time.time - startTime) / (20 - _speedLevel.value));
            slider.value = sliderValue;

            // wenn slider 'voll' material auswerfen und crushingmode deaktivieren
            if (Math.Abs(sliderValue - 100) < .2f)
            {
                // stopwatch.Stop();

                // GameManager.Log(
                //     $"Die Entleerung bie unloadSpeed : {_speedLevel.value} hat {stopwatch.ElapsedMilliseconds / 1000} Sekunden gedauert.");
                // stopwatch.Reset();
                setStatus(false);

                _minerals.value--;

                if (_minerals.value == 0)
                {
                    mineralsAmountText.gameObject.SetActive(false);
                }

                mineralsAmountText.text = "" + _minerals.value;

                var position = chrushedSpawnPoint.transform.position;
                var rotation = chrushedSpawnPoint.transform.rotation;
                var mp_1 = Instantiate(chrushedMineralBP, new Vector3(position.x, position.y, position.z), rotation);
                var mp_2 = Instantiate(chrushedMineralBP, new Vector3(position.x, position.y + .5f, position.z), rotation);
                var mp_3 = Instantiate(chrushedMineralBP, new Vector3(position.x, position.y + 1f, position.z), rotation);
                var mp_4 = Instantiate(chrushedMineralBP, new Vector3(position.x, position.y + 1.5f, position.z), rotation);
                
                mp_1.transform.SetParent(_tmpMineralsContainer, true);
                mp_2.transform.SetParent(_tmpMineralsContainer, true);
                mp_3.transform.SetParent(_tmpMineralsContainer, true);
                mp_4.transform.SetParent(_tmpMineralsContainer, true);
            }
        }

        private void setStatus(bool status)
        {
            if (crushing == status)
            {
                return;
            }

            foreach (MoveHaxlerHammer moveScript in hammerScripts)
            {
                moveScript.enabled = status;
            }

            crushing = status;
        }

        public void SetPufferFilled(bool status)
        {
            pufferFull = status;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
        }

        public int GetSpeedLevel()
        {
            return _speedLevel.value;
        }

        public void UpgradeSpeed()
        {
            
            GameManager.Instance.SubCoins(Mathf.RoundToInt(GLOB.HAXLER_SPEED_BASE_PRICE * Mathf.Pow(costFactor, _speedLevel.value - 1)));
            // GameManager.Instance.SubCoins(costFactor * GLOB.HAXLER_SPEED_BASE_PRICE * _speedLevel.value);
            _speedLevel.value += 1;

            _audioSource.PlayOneShot(_upgradeSound);
        }
    }
}