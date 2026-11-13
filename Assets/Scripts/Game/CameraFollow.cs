using DependencyControl;
using Firebase.DataControl;
using UnityEngine;

namespace Game
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform doodlePos;
        [SerializeField] private GameObject boost;
        [SerializeField] private bool isTutorial;

        void Start()
        {
            if (isTutorial) return;

            if (!Dependency.Get<JsonStorage>().data.otherSettings.showSelectedBoostInGame)
            {
                boost.SetActive(false);
            }
        }

        void Update()
        {
            if (doodlePos.position.y > transform.position.y)
            {
                transform.position = new Vector3(transform.position.x,
                    doodlePos.position.y, transform.position.z);
            }
        }
    }
}
