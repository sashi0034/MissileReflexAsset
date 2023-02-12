using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MissileReflex.Src.Params;
using MissileReflex.Src.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MissileReflex.Src.Battle
{
    [Serializable]
    public class TankAiAgentParam
    {
        [SerializeField] private float updateInterval = 0.1f;
        public float UpdateInterval => updateInterval;
        
        [SerializeField] private float shotRange = 5;
        public float ShotRangeSqrMag => shotRange * shotRange;

        [SerializeField] private float retrieveDuration = 0.3f;
        public float RetrieveDuration => retrieveDuration;
    }
    
    
    public class TankAiAgent : MonoBehaviour, ITankAgent
    {
        [SerializeField] private TankFighter selfTank;
        private TankFighterInput tankIn => selfTank.Input;

        [SerializeField] private BattleRoot battleRoot;
        public BattleRoot BattleRoot => battleRoot;

        [SerializeField] private Material enemyMaterial;

        [SerializeField] private NavMeshAgent navAi;

        private TankAiAgentParam param => ConstParam.Instance.TankAiAgentParam;

        [EventFunction]
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            selfTank.Init(this, enemyMaterial);
            processAiRoutine().Forget();

            navAi.speed = 0;
            navAi.angularSpeed = 0;
            navAi.acceleration = 0;
            navAi.updatePosition = false;
            navAi.updateRotation = false;
        }

        [EventFunction]
        private void Update()
        {
        }

        private async UniTask processAiRoutine()
        {
            while (gameObject != null)
            {
                await UniTask.Delay(param.UpdateInterval.ToIntMilli());

                var targetTank = battleRoot.Player.Tank;
                var targetSqrMag = calcSqrMagSelfWithTargetTank(targetTank);

                var shotRangeSqrMag = param.ShotRangeSqrMag;

                if (targetSqrMag < shotRangeSqrMag)
                {
                    // 射程内に入ってるので退き撃ち
                    await shotWithRetreat(targetTank);
                }
                else
                {
                    // 射程内に入ってないので目標に近づく
                    approachrTargetTank(targetTank);
                }
                
            }
        }

        private async UniTask shotWithRetreat(TankFighter targetTank)
        {
            var destVec = calcDestVecToTarget(targetTank);
            var rotatedDestVec = Quaternion.Euler(0, Random.Range(0, 2) == 0 ? 90 : -90, 0) * destVec;
            tankIn.SetMoveVec(rotatedDestVec.normalized);
            tankIn.SetShotRadFromVec3(destVec);
            tankIn.ShotRequest.UpFlag();
            await UniTask.Delay(param.RetrieveDuration.ToIntMilli());
        }

        private float calcSqrMagSelfWithTargetTank(TankFighter target)
        {
            return (target.transform.position - selfTank.transform.position).sqrMagnitude;
        }

        private void approachrTargetTank(TankFighter target)
        {
            var destVec = calcDestVecToTarget(target);

            tankIn.SetMoveVec(destVec.normalized);
        }

        private Vector3 calcDestVecToTarget(TankFighter target)
        {
            navAi.nextPosition = selfTank.transform.position;
            navAi.SetDestination(target.transform.position);

            var destPos = navAi.steeringTarget;
            var currPos = selfTank.transform.position;
            var destVec = destPos - currPos;
            return destVec;
        }

        private static Vector3 randVecCross()
        {
            return Random.Range(0, 4) switch
            {
                0 => Vector3.forward,
                1 => Vector3.back,
                2 => Vector3.left,
                3 => Vector3.right,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}