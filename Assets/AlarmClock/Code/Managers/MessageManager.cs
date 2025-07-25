using KSP.Game;
using KSP.Messages;

namespace AlarmClock.AlarmClock.Code.Managers
{
    public static class MessageManager
    {
        public static MessageCenter MessageCenter;

        public static void InitializeMessageCenter()
        {
            MessageCenter = GameManager.Instance?.Game?.Messages;
        }
    }
}
