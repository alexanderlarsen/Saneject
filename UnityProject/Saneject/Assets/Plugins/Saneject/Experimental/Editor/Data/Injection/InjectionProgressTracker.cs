using System.ComponentModel;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Data.Injection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class InjectionProgressTracker
    {
        private readonly int totalSegments;

        private string title;
        private string infoText;
        private int currentSegment;
        private int totalSteps;
        private int currentStep;

        public InjectionProgressTracker(int totalSegments)
        {
            this.totalSegments = totalSegments;
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void BeginSegment(int stepCount)
        {
            currentSegment++;
            currentStep = 0;
            totalSteps = stepCount;
        }

        public void NextStep()
        {
            currentStep++;
            UpdateProgressBar();
        }

        public void UpdateInfoText(string infoText)
        {
            this.infoText = infoText;
            UpdateProgressBar();
        }

        public void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        private void UpdateProgressBar()
        {
            float overallProgress = (float)currentSegment / totalSegments;

            EditorUtility.DisplayProgressBar
            (
                title: $"Saneject: {title}",
                info: $"({currentStep}/{totalSteps}) {infoText}",
                progress: overallProgress
            );
        }
    }
}