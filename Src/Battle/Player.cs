#nullable enable

using System;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private float forceSize = 5;

        private Vector3 _velocityOld;
        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Update()
        {
            _velocityOld = rigidbody.velocity;
        }

        [EventFunction]
        private void FixedUpdate()
        {
            var inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            
            rigidbody.AddForce(inputVec * forceSize);
            
            mainCamera.transform.position = gameObject.transform.position.FixY(mainCamera.transform.position.y);
        }

        [EventFunction]
        private void OnCollisionEnter(Collision collision)
        {}
    }
}