using System;
using Cysharp.Threading.Tasks;
using MissileReflex.Src.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MissileReflex.Src.Battle
{
    public class EnemyAgent : MonoBehaviour, ITankAgent
    {
        [SerializeField] private TankFighter selfTank;
        private TankFighterInput tankIn => selfTank.Input;

        [SerializeField] private BattleRoot battleRoot;
        public BattleRoot BattleRoot => battleRoot;

        [SerializeField] private Material enemyMaterial;

        [SerializeField] private NavMeshAgent navAi;

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
                await UniTask.Delay(0.1f.ToIntMilli());
                
                if (Random.Range(0, 2) == 0) tankIn.ShotRequest.UpFlag();
                
                navAi.nextPosition = selfTank.transform.position;
                navAi.SetDestination(battleRoot.Player.TankPos);
                
                var destPos = navAi.steeringTarget;
                var currPos = selfTank.transform.position;
                var destVec = destPos - currPos;

                tankIn.SetMoveVec(destVec.normalized);
            }
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