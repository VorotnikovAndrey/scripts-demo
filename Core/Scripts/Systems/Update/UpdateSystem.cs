using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayVibe
{
    public class UpdateSystem<TInitializeType, TUpdateType>
    {
        private readonly List<UpdateableComponent<TInitializeType, TUpdateType>> _components =
            new List<UpdateableComponent<TInitializeType, TUpdateType>>();

        private bool _isInitialized;

        public void AddComponent(params UpdateableComponent<TInitializeType, TUpdateType>[] component)
        {
            _components.AddRange(component);
        }

        public void OnInitialize(TInitializeType initializeData)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            foreach (var component in _components)
            {
                component.OnInitialize(initializeData);
            }
        }

        public void OnDeInitialize()
        {
            if (!_isInitialized)
                return;
            _isInitialized = false;
            foreach (var component in _components)
            {
                component.OnDeInitialize();
            }
        }

        public void OnUpdate(TUpdateType updateType)
        {
            for (var i = 0; i < _components.Count; i++)
            {
                _components[i].OnUpdate(updateType);
            }
        }

        public void ClearSystem()
        {
            if (_isInitialized)
                Debug.LogError("Update system initialized");

            _components.Clear();
        }
    }

    public class UpdateSystem<TInitializeType>
    {
        private List<UpdateableComponent<TInitializeType>> _components =
            new List<UpdateableComponent<TInitializeType>>();

        private bool _isInitialized;

        public void AddComponent(params UpdateableComponent<TInitializeType>[] component)
        {
            _components.AddRange(component);
        }

        public void RemoveComponent(Type type, bool deinitialize = true)
        {
            var component = _components.FirstOrDefault(x => x.GetType() == type);
            if (component == null) return;

            if (deinitialize)
            {
                component.OnDeInitialize();
            }

            _components.Remove(component);
        }

        public void OnInitialize(TInitializeType initializeData)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            foreach (var component in _components)
            {
                component.OnInitialize(initializeData);
            }
        }

        public void OnDeInitialize()
        {
            if (!_isInitialized)
                return;
            _isInitialized = false;
            foreach (var component in _components)
            {
                component.OnDeInitialize();
            }
        }

        public void OnUpdate()
        {
            for (var i = 0; i < _components.Count; i++)
            {
                _components[i].OnUpdate();
            }
        }

        public void ClearSystem()
        {
            if (_isInitialized)
                Debug.LogError("Update system initialized");

            _components.Clear();
        }
    }

    public class UpdateSystem
    {
        private List<UpdateableComponent> _components = new List<UpdateableComponent>();
        private bool _isInitialized;

        public void AddComponent(params UpdateableComponent[] component)
        {
            _components.AddRange(component);
        }
        
        public void OnInitialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            foreach (var component in _components)
            {
                component.OnInitialize();
            }
        }

        public void OnDeInitialize()
        {
            if (!_isInitialized)
                return;
            _isInitialized = false;
            foreach (var component in _components)
            {
                component.OnDeInitialize();
            }
        }

        public void OnUpdate()
        {
            for (var i = 0; i < _components.Count; i++)
            {
                _components[i].OnUpdate();
            }
        }

        public void ClearSystem()
        {
            if (_isInitialized)
                Debug.LogError("Update system initialized");

            _components.Clear();
        }
    }
}