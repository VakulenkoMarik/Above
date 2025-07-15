using UnityEngine;

namespace Game
{
    public class DestroyPoint : MonoBehaviour
    {
        public static GameObject destroyPoint;

        void Update()
        {
            if (transform.position.y < destroyPoint.transform.position.y)
            {
                Destroy(gameObject);
            }
        }
    }
}
