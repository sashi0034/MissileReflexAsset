using System;
using UnityEngine;

namespace MissileReflex.Src.Utils.Battle
{
    [Serializable]
    public struct PlayerCannon
    {
        [SerializeField] private GameObject cannonView;
        [SerializeField] private Animator cannonAnimator;

        public void LerpCannonRotation(float deltaTime, Vector3 direction)
        {
            cannonView.transform.rotation = Quaternion.Euler(
                0,
                Mathf.LerpAngle(
                    cannonView.transform.rotation.eulerAngles.y,
                    Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg,
                    deltaTime * 20),
                0);
        }
    }
}