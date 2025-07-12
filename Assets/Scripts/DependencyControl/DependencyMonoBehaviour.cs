using UnityEngine;

namespace DependencyControl
{
    public class DependencyMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Dependency.Register(GetType(), this);
        }

        protected virtual void OnDestroy()
        {
            Dependency.UnRegister(GetType(), this);
        }
    }
}
