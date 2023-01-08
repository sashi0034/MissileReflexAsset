using Sirenix.OdinInspector;
using UnityEngine;

namespace MissileReflex.Src.Params
{
    public class DebugParam : SingletonScriptableObject<DebugParam>
    {
        private const string tagBuildIn = "BuildIn";
        
        [FoldoutGroup(tagBuildIn)][SerializeField] private bool isClearDebug = false;
        public bool IsClearDebug => isClearDebug;

#if UNITY_EDITOR
        // [SerializeField] private bool isStartBattleImmediately;
        // public bool IsStartBattleImmediately => isStartBattleImmediately;
#endif
    }
}