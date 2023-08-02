using DG.Tweening;
using InputSystem;
using NTC.Global.Cache;
using Source;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    [RequireComponent(typeof(Camera))]
    public class LocationCamera : MonoCache, ICamera
    {
        public CameraType CameraType => CameraType.LocationCamera;

        [HideInInspector] [SerializeField] private Camera _cam;

        [SerializeField] private float _defaultDampSmoothTime = 0.2f;
        [SerializeField] private float _dampMaxSpeed = 40f;
        [SerializeField] private float _successiveVelocityMult = 0.2f;
        [SerializeField] private float _successiveVelocityThreshold = 1f;
        [SerializeField] private float _smoothTimeVelocityDependenceModif = 15;
        [SerializeField] private float _smoothTimeVelocityDependenceClamp = 0.5f;
        [SerializeField] private float _camDistanceFromTarget = 50;
        [SerializeField] private float _closeViewZoom = 3f;
        [SerializeField] private float _followingZoom = 6f;
        [Space]
        [SerializeField] private float _zoomSensitivity = 1f;
        [SerializeField] private float _zoomMinSize = 3f;
        [SerializeField] private float _zoomMaxSize = 5f;
        [SerializeField] private float _zoomElasticity = 0.2f;
        [SerializeField] private float _zoomLerpSpeed = 2f;
        [Space]
        [SerializeField] private float _targetMoveInDuration = 0.4f;
        [SerializeField] private AnimationCurve _targetMoveInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _targetMoveOutDuration = 0.4f;
        [SerializeField] private AnimationCurve _targetMoveOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _targetZoomInDuration = 0.4f;
        [SerializeField] private AnimationCurve _targetZoomInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _targetZoomOutDuration = 0.4f;
        [SerializeField] private AnimationCurve _targetZoomOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Plane _plane;
        private Vector3 _prevCursorPos;
        private bool _prevIsFocused;
        private Bounds _bounds;
        private bool _inputBlocked;
        private Vector3 _defaultStatePosition;
        private float _defaultStateOrthoSize;
        private Sequence _switchStateSeq;
        private Transform _currentViewTarget;
        private Vector3 _currentTargetPoint;
        private float _currentTargetZoom;
        private Vector3 _currentDampVelocity;
        private int _prevTouchCount;
        private float _currentDampSmoothTime;
        private Transform _followTarget;
        private LocationInput _locationInput;
        private bool _lockFollowing;
        private bool _inited;
        private Vector3 _screenCenter;
        private EventAggregator _eventAggregator;

        public CameraStates CameraState { get; private set; }
        public Camera Camera => _cam;
        public GameObject GameObject => gameObject;
        public Vector3 CurrentCenterPlanePosition { get; private set; }

        private void OnValidate()
        {
            if (_cam == null)
            {
                _cam = GetComponent<Camera>();
            }
        }

        [Inject]
        private void Construct(
            IInput locationInput,
            EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _locationInput = locationInput as LocationInput;
        }
        
        public void Initialize(CameraSettings cameraSettings)
        {
            _bounds = cameraSettings.Bounds;
            _plane = new Plane(Vector3.up, Vector3.zero);
            Camera.transform.position = _currentTargetPoint = cameraSettings.Position;
            Camera.orthographicSize = _currentTargetZoom = cameraSettings.OrthographicSize;
            _currentDampSmoothTime = _defaultDampSmoothTime;

            Subscriptions();
            
            _inited = true;
        }

        public void DeInitialize()
        {
            Unsubscribes();
        }
        
        private void Subscriptions()
        {
            _eventAggregator.Add<BuildingPointClickEvent>(OnBuildingPointClickEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<BuildingPointClickEvent>(OnBuildingPointClickEvent);
        }
        
        private void OnBuildingPointClickEvent(BuildingPointClickEvent sender)
        {
            SwitchToViewTransform(sender.Point.transform, sender.Point.Offset);
        }

        protected override void Run()
        {
            if (_inited == false || _locationInput == null || _locationInput.DisableHandler.IsDisabled || _switchStateSeq != null)
            {
                return;
            }

            CurrentCenterPlanePosition = PlanePosition(new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (CameraState == CameraStates.Default)
            {
                if (Application.isMobilePlatform)
                {
                    UpdateMobileInput();
                }
                else
                {
                    UpdateStandaloneInput();
                }
            }
        }

        protected override void LateRun()
        {
            if (!_inited)
            {
                return;
            }
            
            switch (CameraState)
            {
                case CameraStates.Default:
                    UpdateMovement();
                    UpdateZoom();
                    break;
                case CameraStates.Following:
                    if (!_lockFollowing)
                    {
                        _currentTargetPoint = _defaultStatePosition = _followTarget.position - Camera.transform.rotation * Vector3.forward * _camDistanceFromTarget;
                        _currentTargetPoint.x = Mathf.Clamp(_currentTargetPoint.x, _bounds.min.x, _bounds.max.x);
                        _currentTargetPoint.z = Mathf.Clamp(_currentTargetPoint.z, _bounds.min.z, _bounds.max.z);
                    }
                    UpdateMovement();
                    break;
                case CameraStates.BuildingView:
                    break;
                case CameraStates.Focus:
                    break;
            }

            _currentDampSmoothTime = _defaultDampSmoothTime * (1 - (_smoothTimeVelocityDependenceModif / _currentDampVelocity.magnitude).ClampTop(_smoothTimeVelocityDependenceClamp));
        }

        private void UpdateMovement()
        {
            Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, _currentTargetPoint, ref _currentDampVelocity, _currentDampSmoothTime, _dampMaxSpeed, Time.deltaTime);
        }

        private void UpdateZoom()
        {
            _currentTargetZoom = Mathf.Clamp(_currentTargetZoom, _zoomMinSize, _zoomMaxSize);
            Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, _currentTargetZoom, Time.deltaTime * _zoomLerpSpeed);
        }

        private void UpdateMobileInput()
        {
            if (Input.touchCount == 1)
            {
                Vector3 touchPos = Input.GetTouch(0).position;
                if (Input.GetTouch(0).phase == TouchPhase.Moved && _prevCursorPos != touchPos)
                {
                    Vector3 delta = PlanePositionDelta(touchPos);
                    AddDelta(delta);
                }

                _prevCursorPos = touchPos;
            }

            if (Input.touchCount >= 2)
            {
                Vector3 pos1 = PlanePosition(Input.GetTouch(0).position);
                Vector3 pos2 = PlanePosition(Input.GetTouch(1).position);
                Vector3 pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
                Vector3 pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

                var zoom = Vector3.Distance(pos1, pos2) / Vector3.Distance(pos1b, pos2b);
                var isEdgeCase = zoom == 0 || zoom > 10;
                if (!isEdgeCase)
                {
                    var orthoSize = _currentTargetZoom / zoom;
                    var elasticityMult = 1 + Mathf.Abs(_zoomElasticity);
                    
                    _currentTargetZoom = Mathf.Clamp(orthoSize, _zoomMinSize / elasticityMult, _zoomMaxSize * elasticityMult);
                }
            }

            if (Input.touchCount == 0 && _prevTouchCount > 0)
            {
                TrySuccessiveVelocity();
            }

            _prevTouchCount = Input.touchCount;
        }

        private void UpdateStandaloneInput()
        {
            var zoomAxisValue = Input.GetAxis("Mouse ScrollWheel");

            if (zoomAxisValue != 0)
            {
                _currentTargetZoom -= zoomAxisValue * _zoomSensitivity;
                var elasticityMult = 1 + Mathf.Abs(_zoomElasticity);
                _currentTargetZoom = Mathf.Clamp(_currentTargetZoom, _zoomMinSize / elasticityMult,
                    _zoomMaxSize * elasticityMult);
            }

            if (Input.GetMouseButton(0))
            {
                if (_prevIsFocused != Application.isFocused)
                {
                    _prevCursorPos = Input.mousePosition;
                }

                Vector3 delta = PlanePositionDelta(Input.mousePosition);

                if (Input.mousePosition != _prevCursorPos)
                {
                    AddDelta(delta);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                TrySuccessiveVelocity();
            }

            _prevCursorPos = Input.mousePosition;
            _prevIsFocused = Application.isFocused;
        }

        private void TrySuccessiveVelocity()
        {
            if (!(_currentDampVelocity.magnitude > _successiveVelocityThreshold))
            {
                return;
            }

            _currentTargetPoint += _currentDampVelocity * _successiveVelocityMult;
            _currentTargetPoint.x = Mathf.Clamp(_currentTargetPoint.x, _bounds.min.x, _bounds.max.x);
            _currentTargetPoint.z = Mathf.Clamp(_currentTargetPoint.z, _bounds.min.z, _bounds.max.z);
        }

        private void AddDelta(Vector3 delta)
        {
            Vector3 pos = _currentTargetPoint + delta;
            pos.x = Mathf.Clamp(pos.x, _bounds.min.x, _bounds.max.x);
            pos.z = Mathf.Clamp(pos.z, _bounds.min.z, _bounds.max.z);
            _currentTargetPoint = pos;
        }

        public void SwitchToViewTransform(Transform target, Vector3 offset)
        {
            SwitchToViewTransform(target, _closeViewZoom, offset);
        }

        private void SwitchToViewTransform(Transform target, float orthoSize, Vector3 offset)
        {
            if (_currentViewTarget == target)
            {
                return;
            }

            _currentViewTarget = target;
            _switchStateSeq?.Kill();

            if (CameraState == CameraStates.Default)
            {
                _currentTargetPoint = _defaultStatePosition = Camera.transform.position;
                _defaultStateOrthoSize = Camera.orthographicSize;
            }

            CameraState = CameraStates.BuildingView;

            _switchStateSeq = DOTween.Sequence();
            Vector3 targetPos = CalculateCameraTargetPosition(target.position, offset, orthoSize);
           
            _switchStateSeq.Append(Camera.DOOrthoSize(orthoSize, _targetZoomInDuration).SetEase(_targetZoomInCurve));
            _switchStateSeq.Join(Camera.transform.DOMove(targetPos, _targetMoveInDuration).SetEase(_targetMoveInCurve));
            _switchStateSeq.SetUpdate(UpdateType.Late);
            _switchStateSeq.onKill = () =>
            {
                _switchStateSeq = null;
            };
        }
        
        public void SwitchToDefaultState(bool returnToPrevPos = true)
        {
            if (CameraState == CameraStates.Default)
            {
                return;
            }
            
            _lockFollowing = true;
            _currentViewTarget = null;
            _switchStateSeq?.Kill();

            _switchStateSeq = DOTween.Sequence();
            if (!returnToPrevPos)
            {
                _currentTargetPoint = _defaultStatePosition = CurrentCenterPlanePosition - Camera.transform.forward * _camDistanceFromTarget;
                _currentTargetZoom = _defaultStateOrthoSize = Camera.orthographicSize;
            }
            else
            {
                _currentTargetPoint = _defaultStatePosition;
                _currentTargetZoom = _defaultStateOrthoSize;
            }

            _switchStateSeq.Join(Camera.transform.DOMove(_defaultStatePosition, _targetMoveOutDuration).SetEase(_targetMoveOutCurve));
            _switchStateSeq.Join(Camera.DOOrthoSize(_defaultStateOrthoSize, _targetZoomOutDuration).SetEase(_targetZoomOutCurve));
            _switchStateSeq.SetUpdate(UpdateType.Late);
            _switchStateSeq.onKill = () =>
            {
                _switchStateSeq = null;
                _prevCursorPos = Input.mousePosition;
                CameraState = CameraStates.Default;
            };
        }

        public void SwitchToFollow(Transform followTarget)
        {
            _lockFollowing = false;
            _followTarget = followTarget;

            _switchStateSeq?.Kill();

            if (CameraState == CameraStates.Default)
            {
                _defaultStateOrthoSize = Camera.orthographicSize;
            }

            CameraState = CameraStates.Following;

            _switchStateSeq = DOTween.Sequence();
            _switchStateSeq.Join(Camera.DOOrthoSize(_followingZoom, _targetZoomInDuration).SetEase(_targetZoomInCurve));
            _switchStateSeq.SetUpdate(UpdateType.Late);
            _switchStateSeq.onKill = () =>
            {
                _switchStateSeq = null;
            };
        }

        private Vector3 PlanePositionDelta(Vector3 mousePos)
        {
            if (_prevCursorPos == mousePos)
            {
                return Vector3.zero;
            }

            Ray rayBefore = Camera.ScreenPointToRay(mousePos - (mousePos - _prevCursorPos));
            Ray rayNow = Camera.ScreenPointToRay(mousePos);

            if (_plane.Raycast(rayBefore, out var enterBefore) && _plane.Raycast(rayNow, out var enterNow))
            {
                return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);
            }

            return Vector3.zero;
        }

        private Vector3 PlanePosition(Vector2 screenPos)
        {
            Ray rayNow = Camera.ScreenPointToRay(screenPos);
            return _plane.Raycast(rayNow, out var enterNow) ? rayNow.GetPoint(enterNow) : Vector3.zero;
        }

        public void MoveToPosition(Vector3 position)
        {
            _prevCursorPos = Input.mousePosition;
            CameraState = CameraStates.Default;
            _currentTargetPoint = CalculateCameraTargetPosition(position, Vector2.zero, _cam.orthographicSize);
        }

        public void ZoomTo(float value)
        {
            _currentTargetZoom = Mathf.Clamp(value, _zoomMinSize, _zoomMaxSize);
        }

        private Vector3 CalculateCameraTargetPosition(Vector3 position, Vector2 offset, float targetOrthoSize)
        {
            offset *= targetOrthoSize / _cam.orthographicSize;
            Vector3 worldCenter = _cam.ViewportToWorldPoint(new Vector3(0.5f + offset.x, 0.5f + offset.y, 0));

            return _cam.transform.position + position - worldCenter;
        }
    }
}