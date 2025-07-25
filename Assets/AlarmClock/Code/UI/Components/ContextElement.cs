using System;

namespace AlarmClock.AlarmClock.Code.UI.Components
{
    public class ContextElement : UITKElement
    {
        protected Action<int> _swapContext; 
        public ContextElement(Action<int> swapContext, string path) : base(path)
        {
            _swapContext= swapContext;
        }
    }
}
