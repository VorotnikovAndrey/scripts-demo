using UnityEngine;

namespace Utils.ObjectPool
{
    public interface IView 
    {
        bool isActive { get; set; }
        int Index { get; }
        
        Transform Transform { get; }
        GameObject GameObject { get; }
        
        string Name { get; set; }
        Vector3 WorldPosition { get; }
        void SetPosition(Vector2 target);
        void SetPosition(Vector3 target);
        void SetLocalPosition(Vector3 target);
        void SetViewActive(bool isActive);
        void SetParent(Transform parent);
        void Initialize(object data);
        void Deinitialize();
        void SetEulerAngles(Vector3 diretion);
        void SetRotation(Quaternion rotation);
    }
}