using Sirenix.OdinInspector;
using UnityEngine;

namespace MissileReflex.Src.Params
{
    [CreateAssetMenu(fileName = nameof(ConstParam), menuName = "ScriptableObjects/Create" + nameof(ConstParam))]
    public class ConstParam : SingletonScriptableObject<ConstParam>
    {
        // [SerializeField] private float missileOffsetY = 0.5f;
        // public float MissileOffsetY => missileOffsetY;
        
        public const float DeltaMilliF = 1e-3f;
    }
}