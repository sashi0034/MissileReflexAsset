using MissileReflex.Src.Battle;
using MissileReflex.Src.Utils;
using UnityEngine;

namespace MissileReflex.Src.Battle
{
    [DisallowMultipleComponent]
    public class MissileManager : MonoBehaviour
    {
        [SerializeField] private Missile missilePrefab;
        public Missile MissilePrefab => missilePrefab;

        public void ShootMissile(MissileInitArg arg)
        {
            var missile = Instantiate(missilePrefab, this.transform);
            missile.transform.position = missile.transform.position.FixY(0.5f);
            missile.Init(arg);
        }
        
    }
}