using UnityEngine.UIElements;

namespace AlarmClock.AlarmClock.Code.UI.Components
{
    public class UITKElement : VisualElement
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        public UITKElement(string assetPath)
        {
            if (AlarmClockForKSP2Plugin.ModBundle.LoadAsset<VisualTreeAsset>($"Assets/{assetPath}").CloneTree() is { } template) Add(template);
            // if (AssetManager.GetAsset<VisualTreeAsset>($"{AlarmClockForKSP2Plugin.ModGuid}/{assetPath}").CloneTree() is TemplateContainer template) Add(template);
        }
    }
}
