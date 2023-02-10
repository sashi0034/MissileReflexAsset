#nullable enable

using MissileReflex.Src.Battle;
using MissileReflex.Src.Utils;
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

        [SerializeField] private PlayerCannon playerCannon;

        [SerializeField] private PlayerLeg playerLeg;

        [SerializeField] private GameObject viewObject;

        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Update()
        {
            checkInputMove();
            
            updateInputShoot();

            // カメラ位置調整
            mainCamera.transform.position = gameObject.transform.position.FixY(mainCamera.transform.position.y);
        }

        private void checkInputMove()
        {
            var inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            bool hasInput = inputVec != Vector3.zero;
            
            // 移動アニメ制御
            playerLeg.AnimRun(hasInput);
            
            if (hasInput)
            {
                playerLeg.LerpLegRotation(Time.deltaTime, inputVec);
                trickViewRotation();
            }
            
            // 加速
            rigidbody.velocity += inputVec * accelSize * Time.deltaTime;
            if (rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;

            // 減衰
            if (inputVec == Vector3.zero) rigidbody.velocity *= velocityAttenuation;
        }

        private void trickViewRotation()
        {
            viewObject.transform.localRotation =
                Quaternion.Euler(Vector3.right * (30 - 10 * Mathf.Sin(playerLeg.GetLegRotRadY())));
        }

        private void updateInputShoot()
        {
            var playerPos = transform.position;
            var distancePlayerCamera = Vector3.Distance(playerPos, mainCamera.transform.position); 
            var mousePos = Input.mousePosition.FixZ(distancePlayerCamera);
            var worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);

            var shotDirection = (worldMousePos - playerPos).normalized;

            playerCannon.LerpCannonRotation(Time.deltaTime, shotDirection);
            
            if (Input.GetMouseButtonDown(0)) shootMissile(shotDirection);
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