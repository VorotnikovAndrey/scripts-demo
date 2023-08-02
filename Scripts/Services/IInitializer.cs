using Cysharp.Threading.Tasks;

namespace PlayVibe
{
    public interface IInitializer
    {
        UniTask Initialize();
        UniTask DeInitialize();
    }
}