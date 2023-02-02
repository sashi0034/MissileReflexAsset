using UnityEngine;

namespace MissileReflex.Src.Utils.Battle
{
    [DisallowMultipleComponent]
    public class BattleRoot : MonoBehaviour
    {
        [SerializeField] private MissileManager missileManager;
        public MissileManager MissileManager => missileManager;

        [SerializeField] private Player player;
        public Player Player => player;
        
    }
}