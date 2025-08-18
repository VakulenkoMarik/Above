using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Level/Level settings", fileName = "New level settings data")]
    public class LevelSettingData : ScriptableObject
    {
        public List<GameObject> obstacles;
    }
}