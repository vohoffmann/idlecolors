using UnityEngine;

namespace IdleColors.room_collect.belt
{
    public class Movement : MonoBehaviour
    {
        private Rigidbody _body;
        private Vector3 _pos;
        public float Speed;

        public void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _pos = transform.position;
        }

        public void FixedUpdate()
        {
            if (_body != null)
            {
                Vector3 movement = -transform.forward * Speed * Time.fixedDeltaTime;
                transform.position = _pos - movement;
                _body.MovePosition(_pos);
            }
        }

        public void OnCollisionStay(Collision other)
        {
            if (other.rigidbody != null &&
                !other.rigidbody.isKinematic)
            {
                Vector3 movement = -transform.forward * Time.deltaTime;
                other.rigidbody.MovePosition(other.transform.position + movement);
            }
        }
    }
}