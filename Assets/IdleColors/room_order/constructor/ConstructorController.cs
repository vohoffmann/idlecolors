using System.Collections;
using System.Collections.Generic;
using IdleColors.room_order.constructor.states;
using UnityEngine;

namespace IdleColors.room_order.constructor
{
    public class ConstructorController : StateManager
    {
        public static ConstructorController instance;


        public float Speed = 5;
        public GameObject idlePosition;
        [SerializeField] public GameObject _kopf;
        [SerializeField] private GameObject _armX;
        [SerializeField] private GameObject _armZ;
        public List<TargetMetaData> targets = new();
        public Vector3 target;
        public Vector3 cubeTarget;
        public int targetIndex;
        public bool holdConstructor = true;
        public GameObject _missingColorText;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            _missingColorText.SetActive(false);

            ChangeState(new Idle(this));
        }

        private void Update()
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
    }
}