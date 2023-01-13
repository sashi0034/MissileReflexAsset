#nullable enable

using System;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    public record MissileSourceData(
        float Speed)
    {
        public static readonly MissileSourceData Empty = 
            new MissileSourceData(0);
    };

    public record MissileInitArg(
        MissileSourceData SourceData,
        Vector3 InitialPos,
        Vector3 InitialVel);
    
    [DisallowMultipleComponent]
    public class Missile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;

        private MissileSourceData _data = MissileSourceData.Empty;
        private Vector3 _velocityOld;

        public void Init(MissileInitArg arg)
        {
            _data = arg.SourceData;
            transform.position = arg.InitialPos;
            rigidBody.velocity = arg.InitialVel;
        }

        [EventFunction]
        private void Update()
        {
            _velocityOld = rigidBody.velocity;
        }

        [EventFunction]
        private void FixedUpdate()
        {
            rigidBody.velocity = rigidBody.velocity.normalized * _data.Speed;
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