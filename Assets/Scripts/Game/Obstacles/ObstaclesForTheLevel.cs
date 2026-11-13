using System.Collections.Generic;
using UnityEngine;

namespace Game.Obstacles
{
    [CreateAssetMenu(menuName = "Obstacles/Obstacles data for level", fileName = "New obstacles data")]
    public class ObstaclesForTheLevel : ScriptableObject
    {
        public List<GameObject> obstacles;
    }
}