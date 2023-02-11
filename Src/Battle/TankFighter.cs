#nullable enable
using System;
using MissileReflex.Src.Utils;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public class TankFighterInput
    {
        private Vector2 _moveVec = Vector2.zero;
        public Vector2 MoveVec => _moveVec;

        private float _shotRad = 0;
        public float ShotRad => _shotRad;
        
        private readonly BoolFlag _shotRequest = new BoolFlag();
        public BoolFlag ShotRequest => _shotRequest;

        public void Init()
        {
            _moveVec = Vector2.zero;
            _shotRad = 0;
            _shotRequest.Clear();
        }

        public void SetMoveVec(Vector2 move)
        {
            Debug.Assert(move.SqrMagnitude() <= 1);
            _moveVec = move;
        }

        public void SetShotRad(float rad)
        {
            _shotRad = rad;
        }
    }

    public interface ITankAgent
    {
        public BattleRoot BattleRoot { get; }
    }
    
    // TankFighterはAgentから動かす
    public class TankFighter : MonoBehaviour
    {
        [SerializeField] private Rigidbody tankRigidbody;
        
        [SerializeField] private float accelSize = 10;
        [SerializeField] private float maxSpeed = 5;
        [SerializeField] private float velocityAttenuation = 0.5f;

        [SerializeField] private TankFighterCannon tankFighterCannon;
        [SerializeField] private TankFighterLeg tankFighterLeg;

        [SerializeField] private GameObject viewObject;

        private ITankAgent? _parentAgent;

        private readonly TankFighterInput _input = new TankFighterInput();
        public TankFighterInput Input => _input;
        
        [EventFunction]
        private void Update()
        {
            checkInputMove();
            
            updateInputShoot();
        }

        public void Init(ITankAgent agent)
        {
            _parentAgent = agent;
            _input.Init();
        }
        
        private void checkInputMove()
        {
            var inputVec = new Vector3(UnityEngine.Input.GetAxisRaw("Horizontal"), 0, UnityEngine.Input.GetAxisRaw("Vertical")).normalized;

            bool hasInput = inputVec != Vector3.zero;
            
            // 移動アニメ制御
            tankFighterLeg.AnimRun(hasInput);
            
            if (hasInput)
            {
                tankFighterLeg.LerpLegRotation(Time.deltaTime, inputVec);
                trickViewRotation();
            }
            
            // 加速
            tankRigidbody.velocity += inputVec * accelSize * Time.deltaTime;
            if (tankRigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                tankRigidbody.velocity = tankRigidbody.velocity.normalized * maxSpeed;

            // 減衰
            if (inputVec == Vector3.zero) tankRigidbody.velocity *= velocityAttenuation;
        }
        
        private void trickViewRotation()
        {
            viewObject.transform.localRotation =
                Quaternion.Euler(Vector3.right * (30 - 10 * Mathf.Sin(tankFighterLeg.GetLegRotRadY())));
        }
        
        private void updateInputShoot()
        {
            var shotDirection = new Vector3(Mathf.Cos(_input.ShotRad), 0, Mathf.Sin(_input.ShotRad));

            tankFighterCannon.LerpCannonRotation(Time.deltaTime, shotDirection);
            
            if (_input.ShotRequest.PeekFlag()) shootMissile(shotDirection);
        }
        
        private void shootMissile(Vector3 initialVel)
        {
            var initialPos = transform.position;

            const float missileSpeed = 10f;
            
            Debug.Assert(_parentAgent != null);
            _parentAgent.BattleRoot.MissileManager.ShootMissile(new MissileInitArg(
                new MissileSourceData(missileSpeed),
                initialPos,
                initialVel));
        }
    }
}