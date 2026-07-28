using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreSystems;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingUIController : AbstractUIScreenController
    {
        [SerializeField] private float minDisplaySeconds = 1.2f; //스플래시 스크린이 너무 빨리 지나가지 않도록 

        private Label _statusLabel;
        private VisualElement _progressFill;

        private List<(string labelText, Func<Task> workFunc)> BuildSteps() => new()
        {
            ("시스템 초기화...", () => Task.Delay(200)),
            ("리소스 준비중...", LoadAssets)
        };

        private async Task LoadAssets()
        {
            //실제 에셋이나 데이터들을 로드하는 자리로 남겨둔다.
            await Task.Delay(400);
        }

        protected override void Bind()
        {
            _statusLabel = Lbl("status-label");
            _progressFill = Q<VisualElement>("progress-fill");
            SetProgress(0);
            _ = RunLoadingAsync();
        }

        private async Task RunLoadingAsync()
        {
            float startTime = Time.realtimeSinceStartup;
            List<(string labelText, Func<Task> workFunc)> steps = BuildSteps();

            for (int i = 0; i < steps.Count; i++)
            {
                SetStatus(steps[i].labelText);
                SetProgress((float)i / steps.Count);
                try
                {
                    await steps[i].workFunc();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Loading UI] Step : {steps[i].labelText} 진행중, 경고 : {e.Message}");
                }
                SetProgress((float)(i+1) / steps.Count);
            }
            
            float elapsed = Time.realtimeSinceStartup - startTime;
            if (elapsed < minDisplaySeconds) 
                await Task.Delay(TimeSpan.FromSeconds(minDisplaySeconds - elapsed));
            
            SetStatus("Complete!");
            //씬전환.
            SceneRouter.Go(SceneRouter.Login);
        }
        
        #region Update UI methods

        protected void SetStatus(string text)
        {
            if(_statusLabel == null) return;
            _statusLabel.text = text;
        }
        
        private void SetProgress(float t01)
        {
            if (_progressFill != null)
                _progressFill.style.width = Length.Percent(Mathf.Clamp01(t01) * 100f);
        }

        #endregion
    }
}

