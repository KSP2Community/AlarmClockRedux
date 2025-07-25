﻿using System;
using AlarmClock.AlarmClock.Code.Managers;
using AlarmClock.AlarmClock.Code.UI.Controllers;
using UnityEngine.UIElements;

namespace AlarmClock.AlarmClock.Code.UI.Components
{
    public class NewAlarmContext : ContextElement
    {

        public Button ManeuverButton;
        public Button SoiButton;
        public Button TransferWindowButton;
        public Button CustomAlarmButton;
        public Button TimerButton;

        public Button SettingsButton;

        public NewAlarmContext(Action<int> swapContext) : base(swapContext, "UI/NewAlarmMenu.uxml")
        {
            ManeuverButton = this.Q<Button>("maneuver-button");
            ManeuverButton.clicked += ManeuverButtonClicked;

            SoiButton = this.Q<Button>("soi-button");
            SoiButton.clicked += SOIButtonClicked;

            TransferWindowButton = this.Q<Button>("transfer-window-button");
            TransferWindowButton.clicked += TransferWindowButtonClicked;

            CustomAlarmButton = this.Q<Button>("custom-button");
            CustomAlarmButton.clicked += CustomAlarmButtonClicked;

            TimerButton = this.Q<Button>("timer-button");
            TimerButton.clicked += TimerButtonClicked;

            SettingsButton = this.Q<Button>("options-button");
            SettingsButton.clicked += SettingsClicked;
        }
        private void DefaultToListView()
        {
            _swapContext((int)MainWindowContext.AlarmsList);
        }

        private void ManeuverButtonClicked()
        {
            if (SimulationManager.CurrentManeuver == null)
            {
                _swapContext((int)MainWindowContext.AlarmsList);
                return;
            }

            double maneuverTimeSeconds = SimulationManager.CurrentManeuver.Time;
            TimeManager.Instance.AddAlarm($"{SimulationManager.ActiveVessel.Name} reaches maneuver", maneuverTimeSeconds);
            _swapContext((int)MainWindowContext.AlarmsList);
        }

        private void SOIButtonClicked()
        {
            SimulationManager.UpdateSOIChangePrediction();
            if (!SimulationManager.SOIChangePredictionExists)
            {
                _swapContext((int)MainWindowContext.AlarmsList);
                return;
            }

            double soiChangeTimeSeconds = SimulationManager.SOIChangePrediction;
            TimeManager.Instance.AddAlarm($"{SimulationManager.ActiveVessel.Name} changes SOI", soiChangeTimeSeconds);
            _swapContext((int)MainWindowContext.AlarmsList);
        }

        private void TransferWindowButtonClicked()
        {
            _swapContext((int)MainWindowContext.TransferWindow);
        }

        private void CustomAlarmButtonClicked()
        {
            _swapContext((int)MainWindowContext.Custom);
        }

        private void TimerButtonClicked()
        {
            _swapContext((int)MainWindowContext.Timer);
        }

        private void SettingsClicked()
        {
            _swapContext((int)MainWindowContext.Settings);
        }
    }
}
