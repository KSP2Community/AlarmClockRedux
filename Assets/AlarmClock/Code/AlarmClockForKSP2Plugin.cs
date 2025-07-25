using System;
using System.IO;
using AlarmClock.AlarmClock.Code.Managers;
using AlarmClock.AlarmClock.Code.UI.Controllers;
using KSP.Messages;
using Redux.API.Game;
using Redux.ExtraModTypes;
using ReduxLib.Configuration;
using ReduxLib.Input;
using SpaceWarp2.UI.API.Appbar;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;
using Input = UnityEngine.Input;

namespace AlarmClock.AlarmClock.Code
{
    public class AlarmClockForKSP2Plugin : KerbalMod
    {

        // AppBar button IDs
        internal const string ToolbarFlightButtonID = "BTN-AlarmClockForKSP2Flight";
        internal const string ToolbarOabButtonID = "BTN-AlarmClockForKSP2OAB";
        internal const string ToolbarKscButtonID = "BTN-AlarmClockForKSP2KSC";

        // Singleton instance of the plugin class
        public static AlarmClockForKSP2Plugin Instance { get; set; }

        // Window Controller Reference
        internal WindowController AlarmWindowController;
        internal AlertController AlertWindowController;

        internal bool GameStateValid = false;

        internal ConfigValue<KeyboardShortcut> OpenAlarmClockKey;

        public override void OnPreInitialized()
        {
            OpenAlarmClockKey = new(SWConfiguration.Bind("Keybindings", "Open Alarm Clock",
                new KeyboardShortcut(KeyCode.A, KeyCode.LeftAlt),
                "Keyboard shortcut used for opening the alarm clock window"));
        }

        internal static AssetBundle ModBundle;
        /// <summary>
        /// Runs when the mod is first initialized.
        /// </summary>
        public override void OnInitialized()
        {
            Instance = this;

            MessageManager.InitializeMessageCenter();

            // Import uxml and set up the window
            // VisualTreeAsset uxml = AssetManager.GetAsset<VisualTreeAsset>($"{ModGuid}/" + "alarmclock-resources/UI/MainWindow.uxml");
            var bundle =
                AssetBundle.LoadFromFile(SWMetadata.Folder.FullName + "/assets/bundles/alarmclock-resources.bundle");
            ModBundle = bundle;
            var uxml = bundle.LoadAsset<VisualTreeAsset>("Assets/UI/MainWindow.uxml");
            
            WindowOptions windowOptions = new WindowOptions
            {
                WindowId = "AlarmClockForKSP2_Window",

                Parent = null,

                IsHidingEnabled = true,

                DisableGameInputForTextFields = true,

                MoveOptions = new MoveOptions
                {
                    IsMovingEnabled = true,
                    CheckScreenBounds = true,
                }
            };

            var alarmWindow = Window.Create(windowOptions, uxml);

            var alertuxml = bundle.LoadAsset<VisualTreeAsset>("Assets/UI/AlertWindow.uxml");
            var iconFileName = SWMetadata.Folder.FullName + "/assets/images/icon.png";
            var icon = new Texture2D(2,2);
            icon.LoadImage(File.ReadAllBytes(iconFileName));
            
            WindowOptions alertWindowOptions = new WindowOptions
            {
                WindowId = "AlarmClockForKSP2_Alert",

                Parent = null,

                IsHidingEnabled = true,

                DisableGameInputForTextFields = true,

                MoveOptions = new MoveOptions
                {
                    IsMovingEnabled = false,
                    CheckScreenBounds = true,
                }
            };

            UIDocument alertWindow = Window.Create(alertWindowOptions, alertuxml);

            AlarmWindowController = alarmWindow.gameObject.AddComponent<WindowController>();
            AlertWindowController = alertWindow.gameObject.AddComponent<AlertController>();

            // Register Flight AppBar button
            Appbar.RegisterAppButton(
                SWMetadata.Name,
                ToolbarFlightButtonID,
                icon,
                isOpen => AlarmWindowController.IsWindowOpen = isOpen
            );

            // Register OAB AppBar Button
            Appbar.RegisterOABAppButton(
                SWMetadata.Name,
                ToolbarOabButtonID,
                icon,
                isOpen => AlarmWindowController.IsWindowOpen = isOpen
            );

            // Try to get the currently active vessel, set its throttle to 100% and toggle on the landing gear
            try
            {
                var currentVessel = Vehicle.ActiveVesselVehicle;
                if (currentVessel != null)
                {
                    currentVessel.SetMainThrottle(1.0f);
                    currentVessel.SetGearState(true);
                }
            }
            catch (Exception){}

            PersistentDataManager.InititializePersistentDataManager(SWMetadata.Guid);

            LinkManagersToMessages();

        }

        public override void OnPostInitialized()
        {
        }

        private void LinkManagersToMessages()
        {
            MessageManager.MessageCenter.PersistentSubscribe<GameStateChangedMessage>(HideWindowOnInvalidState);

            MessageManager.MessageCenter.PersistentSubscribe<ManeuverCreatedMessage>(SimulationManager.UpdateCurrentManeuver);
            MessageManager.MessageCenter.PersistentSubscribe<ManeuverMessageBase>(SimulationManager.UpdateCurrentManeuver);

            //Add if ManeuverMessageBase doesn't work
            //MessageManager.MessageCenter.PersistentSubscribe<ManeuverRemovedMessage>(SimulationManager.UpdateCurrentManeuver);
            //MessageManager.MessageCenter.PersistentSubscribe<ManeuverFinishedMessage>(SimulationManager.UpdateCurrentManeuver);

            MessageManager.MessageCenter.PersistentSubscribe<ActiveVesselDestroyedMessage>(SimulationManager.UpdateCurrentManeuver);
            MessageManager.MessageCenter.PersistentSubscribe<GameStateChangedMessage>(SimulationManager.UpdateCurrentManeuver);
            MessageManager.MessageCenter.PersistentSubscribe<VesselChangedMessage>(SimulationManager.UpdateCurrentManeuver);

            MessageManager.MessageCenter.PersistentSubscribe<SOIChangePredictedMessage>(SimulationManager.UpdateSOIChangePrediction);

        }

        public void HideWindowOnInvalidState(MessageCenterMessage obj)
        {
            GameStateValid = GameStateManager.IsGameStateValid();
            if (!GameStateValid)
            {
                AlarmWindowController.IsWindowOpen = false;
            }
            Instance.SWLogger.LogMessage($"Game state validity: {GameStateValid}");
        }

        public void Update()
        {
            if (GameStateValid)
            {
                if (OpenAlarmClockKey is { Value: { Down: true } })
                {
                    
                    AlarmWindowController.IsWindowOpen = !AlarmWindowController.IsWindowOpen;
                }
                TimeManager.Instance.Update();
            }
        }

        public void OpenMainWindow()
        {
            AlarmWindowController.IsWindowOpen = true;
        }

        public void CreateAlert(string title)
        {
            AlertWindowController.DisplayAlert(title);
        }

    }
}
