#nullable enable

using System;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        [SerializeField] private BattleRoot battleRoot;
        [SerializeField] private Rigidbody rigidbody;
        
        [SerializeField] private float accelSize = 10;
        [SerializeField] private float maxSpeed = 5;
        [SerializeField] private float velocityAttenuation = 0.5f;

        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Update()
        {
            checkInputMove();
            
            checkInputShoot();

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

        private void checkInputShoot()
        {
            if (Input.GetMouseButtonDown(0) == false) return;

            var playerPos = transform.position;
            var distancePlayerCamera = Vector3.Distance(playerPos, mainCamera.transform.position); 
            var mousePos = Input.mousePosition.FixZ(distancePlayerCamera);
            var worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);

            var shotDirection = (worldMousePos - playerPos).normalized;
                
            shootMissile(shotDirection);
        }

        private void shootMissile(Vector3 initialVel)
        {
            var initialPos = transform.position;

            const float missileSpeed = 10f;
            battleRoot.MissileManager.ShootMissile(new MissileInitArg(
                new MissileSourceData(missileSpeed),
                initialPos,
                initialVel));
        }

        [EventFunction]
        private void FixedUpdate()
        { }

        [EventFunction]
        private void OnCollisionEnter(Collision collision)
        {}
    }
}