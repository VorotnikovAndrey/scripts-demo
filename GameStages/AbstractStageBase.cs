using System.Collections.Generic;
using Defong.GameStageSystem;
using UnityEngine;
using Utils;
using Utils.Events;

namespace PlayVibe
{
    public abstract class AbstractStageBase : IStage
    {
        public abstract string StageType { get; }

        public Dictionary<object, IStage> SubStages { get; } = new Dictionary<object, IStage>();

        protected EventAggregator _eventAggregator;

        protected AbstractStageBase(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public virtual void Initialize(object data)
        {
            Debug.Log($"{StageType.AddColorTag(Color.yellow)} Initialized".AddColorTag(Color.cyan));

            foreach (IStage value in SubStages.Values)
            {
                value.Initialize(data);
            }
        }

        public virtual void DeInitialize()
        {
            Debug.Log($"{StageType.AddColorTag(Color.yellow)} DeInitialized".AddColorTag(Color.cyan));

            foreach (IStage value in SubStages.Values)
            {
                value.DeInitialize();
            }
        }
    }
}
