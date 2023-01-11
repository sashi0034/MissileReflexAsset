using System;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;

        private Vector3 _velocityOld;

        [EventFunction]
        private void Update()
        {
            _velocityOld = rigidBody.velocity;
        }

        [EventFunction]
        private void OnCollisionEnter(Collision collision)
        {
            var vecIn = _velocityOld.FixY(0);
            var vecNorm = collision.contacts[0].normal.FixY(0);
            var vecReflect = Vector3.Reflect(vecIn, vecNorm);
            rigidBody.velocity = vecReflect;
        }
    }
}