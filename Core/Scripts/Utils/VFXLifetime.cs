using System.Collections;
using UnityEngine;
using Utils.ObjectPool;

public class VFXLifetime : AbstractBaseView
{
    [SerializeField] private float _lifeTime;

    private Coroutine _coroutine;

    protected override void OnEnabled()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(_lifeTime);

        this.ReleaseItemView();

        _coroutine = null;
    }
}