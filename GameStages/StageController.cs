using System.Collections.Generic;
using Defong.GameStageSystem;
using UnityEngine;

namespace PlayVibe
{
    public class StageController
    {
        private readonly Dictionary<string, AbstractStageBase> _stages = new();
        private readonly EventAggregator _eventAggregator;
        
        public IStage Stage { get; private set; }

        public StageController(EventAggregator eventAggregator, IEnumerable<AbstractStageBase> stages)
        {
            _eventAggregator = eventAggregator;

            foreach (AbstractStageBase stageBase in stages)
            {
                _stages.Add(stageBase.StageType, stageBase);
            }

            _eventAggregator.Add<ChangeStageEvent>(OnChangeStageEvent);
            _eventAggregator.SendEvent(new ChangeStageEvent
            {
                Stage = Constants.Stages.Preload
            });
        }

        ~StageController()
        {
            _eventAggregator.Remove<ChangeStageEvent>(OnChangeStageEvent);
        }

        private void OnChangeStageEvent(ChangeStageEvent sender)
        {
            ChangeStage(sender.Stage, sender.Data);
        }

        public void ChangeStage(string stage, object data)
        {
            if (!_stages.ContainsKey(stage))
            {
                Debug.LogError($"{stage} is not found!".AddColorTag(Color.red));
                return;
            }
            
            Stage?.DeInitialize();
            Stage = _stages[stage];
            Stage.Initialize(data);
        }
    }
}