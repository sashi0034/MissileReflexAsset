using MissileReflex.Src.Battle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MissileReflex.Src.Params
{
    [CreateAssetMenu(fileName = nameof(ConstParam), menuName = "ScriptableObjects/Create" + nameof(ConstParam))]
    public class ConstParam : SingletonScriptableObject<ConstParam>
    {
        [SerializeField] private TankAiAgentParam tankAiAgentParam;
        public TankAiAgentParam TankAiAgentParam => tankAiAgentParam;
        
        
        public const float DeltaMilliF = 1e-3f;
    }
}