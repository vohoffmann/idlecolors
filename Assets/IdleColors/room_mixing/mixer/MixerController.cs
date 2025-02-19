using System.Collections;
using System.Collections.Generic;
using IdleColors.Globals;
using IdleColors.helper;
using IdleColors.room_mixing.pipeball;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdleColors.room_mixing.mixer
{
    public class MixerController : MonoBehaviour
    {
        [SerializeField] private GameObject _innen;
        [SerializeField] private RotateMixer _mixerRotator;
        [SerializeField] private GameObject[] _wayPoints;
        [SerializeField] private GameObject _pipeBall;
        private int _red;
        private int _green;
        private int _blue;

        private List<GameObject> _pipeBalls;
        private Color color;
        private int _colorIndex;

        [HideInInspector] public bool _mixing;

        private bool _emptiing;
        private int _outgoingPipeBallsAmount = 5;

        private void Awake()
        {
            _pipeBalls = new List<GameObject>();

            for (int i = 0; i < _outgoingPipeBallsAmount; i++)
            {
                var pb = Instantiate(_pipeBall);

                pb.GetComponent<PipeBallController>().wayPoints = _wayPoints;
                pb.GetComponent<PipeBallController>().Reset();
                _pipeBalls.Add(pb);
            }
        }

        public void InitOrderMixing(int colorIndex, int red, int green, int blue)
        {
            _mixing = true;
            _red = red;
            _green = green;
            _blue = blue;
            _colorIndex = colorIndex;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.GetComponent<PipeBallController>().Reset();
            if (_emptiing) return;
            _mixerRotator.enabled = true;
            ProcessLogic(other.gameObject);
        }

        public void ProcessLogic(GameObject other)
        {
            color = _innen.GetComponent<Renderer>().material.color;
            if (other.name.IndexOf("red") != -1)
            {
                _red--;
                color.r += 0.045f;
            }

            if (other.name.IndexOf("green") != -1)
            {
                _green--;
                color.g += 0.045f;
            }

            if (other.name.IndexOf("blue") != -1)
            {
                _blue--;
                color.b += 0.045f;
            }

            _innen.GetComponent<Renderer>().material.color = color;

            if (_red <= 0 &&
                _green <= 0 &&
                _blue <= 0)
            {
                EmptyMixer();
            }
        }

        public void EmptyMixer()
        {
            _emptiing = true;
            _mixerRotator.enabled = false;
            EventManager.SetBoxPosition.Invoke();
            StartCoroutine(nameof(RemoveMinerals));
        }

        private IEnumerator RemoveMinerals()
        {
            int loops = _outgoingPipeBallsAmount;
            while (loops > 0)
            {
                yield return Helper.GetWait(.3f);

                foreach (var pb in _pipeBalls)
                {
                    var script = pb.GetComponent<PipeBallController>();
                    if (!script.active)
                    {
                        script.Activate();
                        script._colorIndex = _colorIndex;
                        pb.GetComponent<Renderer>().material.color = color;
                        break;
                    }
                }

                loops--;
            }

            _mixing = false;
            _emptiing = false;
            _innen.GetComponent<Renderer>().material.color = Color.black;
        }
    }
}