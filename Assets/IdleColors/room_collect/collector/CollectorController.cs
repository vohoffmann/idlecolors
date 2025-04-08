using System.Collections;
using System.Collections.Generic;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.helper;
using IdleColors.mineral;
using IdleColors.room_mixing.haxler;
using IdleColors.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IdleColors.room_collect.collector
{
    public class CollectorController : MonoBehaviour, IPointerClickHandler
    {
        public                   int          costFactor;
        private                  int          _collectedMinerals;
        private                  int          _mineralsAvailable;
        private                  bool         _animate;
        private                  bool         _active;
        private                  bool         _returning;
        private                  bool         _scaleDirection;
        private                  bool         _draining;
        private                  bool         _emptiing;
        private                  float        _yVelocity;
        private                  float        _xVelocity;
        private                  float        _sliderValue;
        private                  string       _targetName;
        private                  GameObject   _target;
        private                  NavMeshAgent _agent;
        private                  AudioSource  _audioSource;
        [SerializeField] private GameObject   _collectorTrail;
        [SerializeField] private GameObject   _innerCube;
        [SerializeField] private GameObject   _dustFinSpawnPosition;
        [SerializeField] private GameObject   _beltToSpace;
        [SerializeField] private GameObject   _collectedMineralBP;
        [SerializeField] private SO_Bool      _unlocked;
        [SerializeField] private SO_Int       _capacity;
        [SerializeField] private SO_Int       _speedLevel;
        [SerializeField] private SO_Int       _unloadSpeed;
        [SerializeField] private AudioClip[]  _clickClips;

        [SerializeField] private AudioClip[] _updateClips;

        // [SerializeField] private AudioClip _dropMineral;
        [SerializeField] private AudioClip         _takeMineral;
        [SerializeField] private HaxlerController  haxlerController;
        [SerializeField] private MineralController mineralController;
        [SerializeField] private Slider            slider;
        [SerializeField] private GameObject        _bodyCam;
        [SerializeField] private ParticleSystem    _coinPartikel;
        [SerializeField] private Transform         _tmpMineralsContainer;


        private void Awake()
        {
            _unlocked.value = false;
            _agent          = GetComponent<NavMeshAgent>();

            var localScale = transform.localScale;
            localScale.y += Random.value;

            _audioSource        = GetComponent<AudioSource>();
            _audioSource.volume = 0;
        }

        void Start()
        {
            InvokeRepeating(nameof(checkTarget), 5f, 1f);
        }

        private void OnBecameVisible()
        {
            _audioSource.volume = 1;
        }

        private void OnBecameInvisible()
        {
            _audioSource.volume = 0;
        }

        public void TakeInitValues()
        {
            _agent.speed = _speedLevel.value * 0.5f;
        }

        void Update()
        {
            _collectorTrail.gameObject.transform.localScale = Vector3.one;
            _collectorTrail.gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            if (!_unlocked.value)
            {
                return;
            }

            if (_emptiing)
            {
                slider.value = _sliderValue;

                if (_sliderValue > 99)
                {
                    _emptiing    = false;
                    slider.value = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    _sliderValue += _unloadSpeed.value * 5 * Time.deltaTime;
                }
            }

            _innerCube.SetActive(_collectedMinerals != 0);

            // wenn limit erreicht ODER keine weiteren zum einsammeln,
            // dann zum space belt
            if (!_active &&
                (
                    _collectedMinerals >= _capacity.value ||
                    (_collectedMinerals != 0 && _mineralsAvailable == 0))
               )
            {
                _active     = true;
                _returning  = true;
                _target     = _beltToSpace;
                _targetName = _beltToSpace.name;
                _agent.SetDestination(_target.transform.position);
            }

            if (_active)
            {
                return;
            }

            _mineralsAvailable = 0;

            List<GameObject> minerals = mineralController.GetMinerals();


            if (minerals != null &&
                minerals.Count != 0)
            {
                var        closestDist = 0.0f;
                GameObject closestGO   = null;

                foreach (var speckOfDust in minerals)
                {
                    if (speckOfDust.GetComponent<MineralHandler>().targeted)
                    {
                        continue;
                    }

                    _mineralsAvailable++;

                    var dist = Vector3.Distance(transform.position,
                        speckOfDust.transform.position);

                    if (closestDist == 0.0f ||
                        closestGO == null)
                    {
                        closestDist = dist;
                        closestGO   = speckOfDust;
                        continue;
                    }

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestGO   = speckOfDust;
                    }
                }

                if (closestGO != null)
                {
                    _active     = true;
                    _target     = closestGO;
                    _targetName = closestGO.name;
                    if (!_agent.isActiveAndEnabled)
                    {
                        _agent.enabled = true;
                    }

                    _agent.SetDestination(_target.transform.position);
                    _target.GetComponent<MineralHandler>().targeted = true;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _target)
            {
                if (_draining)
                {
                    return;
                }

                if (_returning)
                {
                    if (_collectedMinerals != 0)
                    {
                        _draining = true;
                        StartCoroutine(nameof(EmptyCleaner));
                    }
                }
                else
                {
                    _collectedMinerals++;
                    _target     = null;
                    _targetName = null;
                    other.gameObject.SetActive(false);
                    other.gameObject.GetComponent<MineralHandler>().dead = true;
                    _active                                              = false;
                    _audioSource.PlayOneShot(_takeMineral);
                }
            }
        }

        public IEnumerator EmptyCleaner()
        {
            _emptiing    = true;
            _sliderValue = 0f;
            slider.gameObject.SetActive(true);

            while (_collectedMinerals > 0)
            {
                yield return Helper.GetWait(.5f);

                if (haxlerController._minerals.value > 5)
                {
                    continue;
                }

                if (_emptiing)
                {
                    continue;
                }

                _collectedMinerals--;
                var coinTextPos = new Vector3(
                    _coinPartikel.transform.position.x,
                    _coinPartikel.transform.position.y + 2,
                    _coinPartikel.transform.position.z);
                GameManager.Instance.AddCoins(5, coinTextPos);
                // _audioSource.PlayOneShot(_dropMineral);
                _coinPartikel.Play();

                var tmpCube = Instantiate(_collectedMineralBP,
                    _dustFinSpawnPosition.transform.position,
                    _dustFinSpawnPosition.transform.rotation);

                tmpCube.transform.SetParent(_tmpMineralsContainer, true);

                if (_collectedMinerals == 0)
                {
                    reset();
                    break;
                }

                _emptiing    = true;
                _sliderValue = 0f;
                slider.gameObject.SetActive(true);
            }
        }

        public void UpgradeCapacity()
        {
            GameManager.Instance.SubCoins(costFactor * GLOB.COLLECTOR_CAPACITY_BASE_PRICE * _capacity.value);
            _capacity.value++;
            PlayUpdateSound();
        }

        public int GetCapacity()
        {
            return _capacity.value;
        }

        public void UpgradeSpeed(bool subCouns = false)
        {
            if (subCouns)
                GameManager.Instance.SubCoins(costFactor * GLOB.COLLECTOR_SPEED_BASE_PRICE *
                                              _speedLevel.value);
            _agent.speed      += .5f;
            _speedLevel.value += 1;
            PlayUpdateSound();
        }

        public int GetSpeedLevel()
        {
            return _speedLevel.value;
        }

        public void UpgradeUnloadSpeed()
        {
            GameManager.Instance.SubCoins(costFactor * GLOB.COLLECTOR_UNLOADSPEED_BASE_PRICE * _unloadSpeed.value);
            _unloadSpeed.value++;
            PlayUpdateSound();
        }

        public int GetUnloadSpeed()
        {
            return _unloadSpeed.value;
        }

        private void PlayUpdateSound()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(
                    _updateClips[Random.Range(0, _updateClips.Length)]);
            }
        }

        private void reset()
        {
            _target            = null;
            _targetName        = null;
            _returning         = false;
            _draining          = false;
            _collectedMinerals = 0;
            _active            = false;
        }

        private void checkTarget()
        {
            if (_targetName == null)
            {
                return;
            }

            var go = GameObject.Find(_targetName);
            if (null == go)
            {
                _target     = null;
                _targetName = null;
                _active     = false;
                _agent.SetDestination(transform.position);
                return;
            }

            if (null != _target &&
                !_returning)
            {
                if (_target.GetComponent<MineralHandler>().dead)
                {
                    _target     = null;
                    _targetName = null;
                    _active     = false;
                    _agent.SetDestination(transform.position);
                    return;
                }

                if (_agent.destination != _target.transform.position)
                {
                    if (!_agent.isActiveAndEnabled)
                    {
                        _agent.enabled = true;
                    }

                    _agent.SetDestination(_target.transform.position);
                }
            }
        }

        public bool IsUnlocked()
        {
            return _unlocked.value;
        }

        public void Unlock()
        {
            _unlocked.value = true;
        }

        private void OnDrawGizmos()
        {
            if (_target)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _target.transform.position);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(
                    _clickClips[Random.Range(0, _clickClips.Length)]);
            }
        }

        public void SetBodyCam(bool value)
        {
            _bodyCam.SetActive(value);
        }
    }
}