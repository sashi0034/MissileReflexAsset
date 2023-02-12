#nullable enable
using System;
using MissileReflex.Src.Params;
using MissileReflex.Src.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public class TankFighterInput
    {
        private Vector3 _moveVec = Vector3.zero;
        public Vector3 MoveVec => _moveVec;

        private float _shotRad = 0;
        public float ShotRad => _shotRad;
        
        private readonly BoolFlag _shotRequest = new BoolFlag();
        public BoolFlag ShotRequest => _shotRequest;

        public void Init()
        {
            _moveVec = Vector3.zero;
            _shotRad = 0;
            _shotRequest.Clear();
        }

        public void SetMoveVec(Vector3 move)
        {
            Debug.Assert(Vector3.SqrMagnitude(move) <= 1 + ConstParam.DeltaMilliF);
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

        [SerializeField] private float maxShotCoolingTime = 0.5f;
        private float _shotCoolingTime = 0;
        
        [EventFunction]
        private void Update()
        {
            checkInputMove();
            
            updateInputShoot(Time.deltaTime);
        }

        public void Init(
            ITankAgent agent,
            Material? material)
        {
            _parentAgent = agent;
            _input.Init();
            _shotCoolingTime = 0;

            if (material != null) ChangeMaterial(material);
        }

        [Button]
        public void ChangeMaterial(Material material)
        {
            tankFighterLeg.ChangeMaterial(material);
            tankFighterCannon.ChangeMaterial(material);
        }
        
        private void checkInputMove()
        {
            var inputVec = _input.MoveVec;

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
            const float trickDefaultAngle = 30;
            const float trickDeltaAngle = 15;
            
            viewObject.transform.localRotation =
                Quaternion.Euler(Vector3.right * (trickDefaultAngle - Mathf.Max(trickDeltaAngle * Mathf.Sin(tankFighterLeg.GetLegRotRadY()), 0)));
        }
        
        private void updateInputShoot(float deltaTime)
        {
            var shotDirection = new Vector3(Mathf.Cos(_input.ShotRad), 0, Mathf.Sin(_input.ShotRad));

            tankFighterCannon.LerpCannonRotation(Time.deltaTime, shotDirection);

            _shotCoolingTime = Math.Max(_shotCoolingTime - deltaTime, 0);

            // ミサイルを打つか確認
            if (_shotCoolingTime > 0 || _input.ShotRequest.PeekFlag() == false) return;
            
            // 発射
            _shotCoolingTime = maxShotCoolingTime;
            shootMissile(shotDirection);
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