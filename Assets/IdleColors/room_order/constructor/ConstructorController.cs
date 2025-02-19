using System.Collections;
using System.Collections.Generic;
using IdleColors.room_order.constructor.states;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace IdleColors.room_order.constructor
{
    public class ConstructorController : StateManager
    {
        public static ConstructorController instance;


        public SO_Int Speed;
        public GameObject idlePosition;
        [SerializeField] public GameObject _kopf;
        [SerializeField] private GameObject _armX;
        [SerializeField] private GameObject _armZ;
        public List<TargetMetaData> targets = new();
        public int[] imageColors = new int[8];
        public TextMeshProUGUI _imageColorStats;
        public Vector3 target;
        public Vector3 cubeTarget;
        public int targetIndex;
        public bool holdConstructor = true;
        public GameObject _missingColorImage;
        public AudioSource audioSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0;

            // _missingColorText.SetActive(false);
            _missingColorImage.SetActive(false);

            ChangeState(new Idle(this));
        }

        private void OnBecameVisible()
        {
            audioSource.volume = .4f;
        }

        private void OnBecameInvisible()
        {
            audioSource.volume = 0;
        }

        private void FixedUpdate()
        {
            _armX.transform.position =
                new Vector3(transform.position.x, _armX.transform.position.y, _armX.transform.position.z);
            _armZ.transform.position =
                new Vector3(_armZ.transform.position.x, _armZ.transform.position.y, transform.position.z);

            _currentState?.Update();
        }

        public void StartCounter()
        {
            StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            yield return new WaitForSeconds(3f);
            holdConstructor = false;
        }

        public void UpdateStatText()
        {
            string text = "";
            foreach (int idx in imageColors)
                text += "" + idx + "\n";
            _imageColorStats.text = text;
        }
    }
}