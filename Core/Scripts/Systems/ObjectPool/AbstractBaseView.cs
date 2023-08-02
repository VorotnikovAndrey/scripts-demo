using NTC.Global.Cache;
using UnityEngine;

namespace Utils.ObjectPool
{
    public abstract class AbstractBaseView : MonoCache, IView
    {
        private static int _index = 0;
        public bool isActive { get; set; }
        public int Index { get; } = _index++;
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        public virtual Vector3 WorldPosition => transform.position;
        
        public virtual string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public virtual void Initialize(object data)
        {
        }

        public virtual void Deinitialize()
        {
            
        }

        public virtual void SetPosition(Vector3 target)
        {
            Transform.position = target;
        }

        public virtual void SetLocalPosition(Vector3 target)
        {
            Transform.localPosition = target;
        }

        public virtual void SetEulerAngles(Vector3 diretion)
        {
            Transform.localEulerAngles = diretion;
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            Transform.rotation = rotation;
        }

        public void SetScale(float value)
        {
            Transform.localScale = Vector3.one * value;
        }

        public void SetScale(Vector3 value)
        {
            Transform.localScale = value;
        }

        public virtual void SetPosition(Vector2 target)
        {
            Transform.position = new Vector3(target.x, 0.1f, target.y);
        }

        public void SetViewActive(bool isActive)
        {
            gameObject.SetActive(isActive);

            if (isActive)
            {
                SwitchOn();
            }
            else
            {
                SwitchOff();
            }
        }

        public virtual void SetParent(Transform parent)
        {
            Transform.SetParent(parent);
        }

        protected virtual void SwitchOn()
        {
        }

        protected virtual void SwitchOff()
        {
        }

        private void OnDestroy()
        {
            this.DestroyAndRemoveFromPool();
        }
    }
}