using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class FaceUI : MonoCache
{
    [SerializeField] private bool _zeroY;

    private MainCamera _mainCamera;
    private Transform _transformCamera;

    protected override void OnEnabled()
    {
        _mainCamera = ProjectContext.Instance.Container.Resolve<MainCamera>();
        _transformCamera = _mainCamera.Camera.transform;
    }

    protected override void Run()
    {
        Vector3 result = 2 * transform.position - _transformCamera.position;
        transform.LookAt(result);

        if (_zeroY)
        {
            var rot = transform.localEulerAngles;
            rot.y = 0;
            transform.localEulerAngles = rot;
        }
    }
}