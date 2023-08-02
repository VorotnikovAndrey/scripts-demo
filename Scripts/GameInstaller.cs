using InputSystem;
using UI;
using UnityEngine;
using Utils;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private MainCanvas _mainCanvasAsset;
        [SerializeField] private PreloadCamera _preloadCamera;
    
        public override void InstallBindings()
        {
            BindBase();
            BindStages();
            BindInterfaces();
            BindFactories();
            BindGameplay();
            BindCamera();
        }
    
        private void BindBase()
        {
            Container.Bind<EventAggregator>().AsSingle().NonLazy();
            Container.Bind<MainCanvas>().FromInstance(_mainCanvasAsset).AsSingle();
            Container.Bind<PopupManager>().AsSingle().NonLazy();
            Container.Bind<UserManager>().AsSingle().NonLazy();
            Container.Bind<Wallet>().AsSingle().NonLazy();
            Container.Bind<UnityServicesInitializer>().AsSingle().NonLazy();
            Container.Bind<DebugManager>().AsSingle().NonLazy();
        }
    
        private void BindStages()
        {
            Container.Bind<AbstractStageBase>().To<PreloadStage>().AsSingle();
            Container.Bind<AbstractStageBase>().To<GameplayStage>().AsSingle();
            Container.Bind<StageController>().AsSingle().NonLazy();
        }
        
        private void BindFactories()
        {
            Container.BindFactory<ScreenFaderBase, ScreenFaderFactory>().AsSingle();
        }
    
        private void BindInterfaces()
        {
            Container.BindInterfacesAndSelfTo<TimeTicker>().AsSingle();
        }
        
        private void BindGameplay()
        {
            Container.Bind<IInput>().To<LocationInput>().AsSingle();
            Container.Bind<LocationsManager>().AsSingle();
        }
    
        private void BindCamera()
        {
            Container.Bind<ICamera>().WithId(CameraType.PreloaderCamera).To<PreloadCamera>().FromInstance(_preloadCamera).AsSingle();
            Container.Bind<CameraManager>().AsSingle().NonLazy();
        }
    }
}