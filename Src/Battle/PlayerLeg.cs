using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace MissileReflex.Src.Utils.Battle
{
    [Serializable]
    public struct PlayerLeg
    {
        [SerializeField] private GameObject legView;
        [SerializeField] private Animator legAnimator;
        
        private static readonly AnimHash hashRun = new AnimHash("run");

        public void AnimRun(bool hasInput)
        {
            legAnimator.SetBool(hashRun.Code, hasInput);
        }

        public float GetLegRotRadY()
        {
            return (90 - legView.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;
        }
        
        public void LerpLegRotation(float deltaTime, Vector3 inputVec)
        {
            legView.transform.localRotation = Quaternion.Euler(
                0,
                Mathf.LerpAngle(
                    legView.transform.localRotation.eulerAngles.y,
                    Mathf.Atan2(inputVec.x, inputVec.z) * Mathf.Rad2Deg,
                    deltaTime * 10),
                0);
        }
    }
}