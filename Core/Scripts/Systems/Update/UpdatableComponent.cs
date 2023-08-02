using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public abstract class UpdateableComponent<TInitializeType, TUpdateType> : BaseUpdateableComponent
    {
        public abstract void OnInitialize(TInitializeType data);
        public abstract void OnUpdate(TUpdateType updateData);
    }

    public abstract class UpdateableComponent<TInitializeType> : BaseUpdateableComponent
    {
        public abstract void OnInitialize(TInitializeType data);
        public abstract void OnUpdate();
    }

    public abstract class UpdateableComponent : BaseUpdateableComponent
    {
        public virtual void OnInitialize() {}
        public override void OnDeInitialize() {}
        public abstract void OnUpdate();
    }

    public abstract class BaseUpdateableComponent
    {
        protected EventAggregator _eventAggregator;

        protected BaseUpdateableComponent()
        {
            _eventAggregator = ProjectContext.Instance.Container.Resolve<EventAggregator>();
        }

        public abstract void OnDeInitialize();

    }

}