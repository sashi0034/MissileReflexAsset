#nullable enable

using System;
using DG.Tweening;
using UnityEngine;

namespace MissileReflex.Src.Utils.Battle
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
        [SerializeField] private GameObject view;

        private Vector3 viewInitialRotation;
        private float viewRotationAnimX = 0;

        private MissileSourceData _data = MissileSourceData.Empty;
        private Vector3 _velocityOld;
        private Vector3 _positionOld;

        public void Init(MissileInitArg arg)
        {
            _data = arg.SourceData;
            transform.position = arg.InitialPos;
            rigidBody.velocity = arg.InitialVel;

            viewInitialRotation = view.transform.localRotation.eulerAngles;
        }

        [EventFunction]
        private void Update()
        {
            _positionOld = transform.position;
            _velocityOld = rigidBody.velocity;
            gameObject.transform.rotation = Quaternion.Euler(
                0,
                -Mathf.Atan2(_velocityOld.z, _velocityOld.x) * Mathf.Rad2Deg,
                0);
            
            updateViewAnim(Time.deltaTime);
        }

        private void updateViewAnim(float deltaTime)
        {
            viewRotationAnimX += deltaTime * 360;
            view.transform.localRotation = Quaternion.Euler(viewInitialRotation + Vector3.right * viewRotationAnimX);
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
            transform.position = _positionOld;
        }
    }
}