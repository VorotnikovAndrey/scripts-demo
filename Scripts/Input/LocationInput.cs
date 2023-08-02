using System.Collections.Generic;
using System.Linq;
using InputSyatem;
using PlayVibe;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Events;

namespace InputSystem
{
    public class LocationInput : IInput
    {
        private const float MinDistance = 15f;
       
        private Vector3 _cursorDownPosition;
        private LocationCamera _camera;
        private EventAggregator _eventAggregator;
        
        private readonly CompositeDisposable _compositeDisposable = new();

        public DisableHandler DisableHandler { get; } = new();

        public LocationInput(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        public void Initialize(ICamera camera)
        {
            _camera = camera as LocationCamera;
            
            Observable.EveryUpdate().Subscribe(Update).AddTo(_compositeDisposable);
            
            Subscriptions();
        }

        public void DeInitialize()
        {
            _compositeDisposable.Dispose();

            Unsubscribes();
        }
        
        private void Subscriptions()
        {
            
        }

        private void Unsubscribes()
        {
           
        }

        public void Update(long delta)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _cursorDownPosition = Input.mousePosition;
            }

            if (!Input.GetMouseButtonUp(0) || !(Vector3.Distance(_cursorDownPosition, Input.mousePosition) < MinDistance))
            {
                return;
            }

            if (!TryPointerCast(out RaycastHit hit))
            {
                return;
            }

            ProcessHit(hit);
        }

        private void ProcessHit(RaycastHit hit)
        {
            if (DisableHandler.IsDisabled)
            {
                return;
            }

            hit.transform.GetComponent<IClickableView>()?.ProcessClick();
        }

        private bool TryPointerCast(out RaycastHit hit)
        {
            hit = default;
            var results = new List<RaycastResult>();
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            EventSystem.current.RaycastAll(pointer, results);
            var canProcess = results.All(result => result.gameObject.GetComponent<ICanvasRaycastFilter>() == null);

            if (!canProcess)
            {
                return false;
            }

            Ray ray = _camera.Camera.ScreenPointToRay(Input.mousePosition);

            return Physics.Raycast(ray, out hit);
        }

        // private bool TrySelectBuildingPoint(RaycastHit hit)
        // {
        //     var point = hit.transform.GetComponent<BuildingPoint>();
        //     if (point == null)
        //     {
        //         return false;
        //     }
        //
        //    
        //
        //     return true;
        // }
    }
}