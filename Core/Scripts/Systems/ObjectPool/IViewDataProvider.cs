using UnityEngine;

namespace Defong.ObjectPool
{
    public interface IViewDataProvider
    {
        object GetDataForView();

        Vector3 GetPositionForView();
    }
}