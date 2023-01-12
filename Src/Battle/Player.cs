#nullable enable

using System;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        
        [SerializeField] private float accelSize = 10;
        [SerializeField] private float maxSpeed = 5;
        [SerializeField] private float velocityAttenuation = 0.5f;

        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Update()
        {
            checkInputMove();

            // カメラ位置調整
            mainCamera.transform.position = gameObject.transform.position.FixY(mainCamera.transform.position.y);
        }

        private void checkInputMove()
        {
            var inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            // 加速
            rigidbody.velocity += inputVec * accelSize * Time.deltaTime;
            if (rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;

            // 減衰
            if (inputVec == Vector3.zero) rigidbody.velocity *= velocityAttenuation;
        }

        [EventFunction]
        private void FixedUpdate()
        { }

        [EventFunction]
        private void OnCollisionEnter(Collision collision)
        {}
    }
}