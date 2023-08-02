using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class ResourcesBar : MonoBehaviour
    {
        [SerializeField] private List<ResourcesPair> _resources = new();
        
        private EventAggregator _eventAggregator;
        private UserManager _userManager;

        public List<ResourcesPair> Resources => _resources;

        [Inject]
        private void Construct(
            EventAggregator eventAggregator,
            UserManager userManager)
        {
            _userManager = userManager;
            _eventAggregator = eventAggregator;
        }

        private void OnValidate()
        {
            if (_resources.Count != 0)
            {
                return;
            }
    
            foreach (ResourceContainer element in transform.GetComponentsInChildren<ResourceContainer>())
            {
                if (_resources.Any(x => x.Type == element.ResourceType))
                {
                    continue;
                }
    
                _resources.Add(new ResourcesPair
                {
                    Container = element,
                    Type = element.ResourceType
                });
            }
        }
    
        private void Start()
        {
            _eventAggregator.Add<AddResourceEvent>(OnUpdateResource);
    
            foreach (ResourcesPair pair in _resources)
            {
                pair.Container.SetValue(_userManager.CurrentUser.Resources[pair.Type], true);
            }
        }
    
        private void OnUpdateResource(AddResourceEvent sender)
        {
            UpdateResource(sender.ResourceType);
        }
    
        private void UpdateResource(string type)
        {
            ResourcesPair element = _resources.FirstOrDefault(x => x.Type == type);
            element?.Container.SetValue(_userManager.CurrentUser.Resources[type]);
        }
        
        private void OnDestroy()
        {
            _eventAggregator.Remove<AddResourceEvent>(OnUpdateResource);
        }
    }
    
    [Serializable]
    public class ResourcesPair
    {
        public string Type;
        public ResourceContainer Container;
    }
}

