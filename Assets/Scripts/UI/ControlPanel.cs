using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;

/// <summary>
/// This class manages the control panel which controls a cellulo
/// </summary>
public class ControlPanel : MonoBehaviour
{
    public CelluloAgent agent;
    private Cellulo robot;
    private int currentCellulo = 0; // The cellulo which is controlled with the control panel

    // Connection
    public Text macAddrText;
    public Text connectionStatusText;

    // Status
    public Text batteryText;
    public Text touchText;
    public Text kidnappedText;

    // Color effects
    public Dropdown visualEffectDropdown;
    public Dropdown LEDResponseModeDropdown;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    private int colorValue = 0;

    // Locomotion
    // Goal coordinates
    private float goalXPose = 0f;
    private float goalYPose = 0f;
    private float goalThetaPose = 0f;
    private float goalVPose = 0f;
    private float goalWPose = 0f;
    // Goal velocity
    private float goalVXVelocity = 0f;
    private float goalVYVelocity = 0f;
    private float goalWVelocity = 0f;

    // Backdrive assist
    public Toggle casualToggle;
    private float hapticXCoeff = 0f;
    private float hapticYCoeff = 0f;
    private float hapticThetaCoeff = 0f;

    // Haptics
    // Simple vibration
    private float simpleXIntensity = 0f;
    private float simpleYIntensity = 0f;
    private float simpleThetaIntensity = 0f;
    private long simplePeriod = 0;
    private long simpleDuration = 0;
    // Vibration on motion
    private float motionIntensityCoeff = 0f;
    private long motionPeriod = 0;

    // Gesture
    public Dropdown locomotionInteractivityDropdown;
    public Toggle gestureToggle;
    public Text currentGestureText;

    // Profiling
    public Toggle timestampsToggle;
    public Text framerateText;
    public Text lastTimestampText;
    private uint poseBcastPeriod = 0;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    { 
        // Initialize agent
        if (agent == null && CelluloManager._celluloList.Count > 0)
        {
            agent = CelluloManager._celluloList.First().Value.GetComponent<CelluloAgent>();
            robot = agent._celluloRobot;
            Initialize();
        }

        // Change agent when local view changes
        if (agent != null && currentCellulo != Config.controlPanelSelectedCellulo)
        {
            currentCellulo = Config.controlPanelSelectedCellulo;
            agent = CelluloManager._celluloList[currentCellulo].GetComponent<CelluloAgent>();
            robot = agent._celluloRobot;
            Initialize();
        }

        // Initialize robot
        if (agent != null && robot == null)
        {
            robot = agent._celluloRobot;
            Initialize();
        }

        // Update when robot disconnected
        if (robot != null && !agent.isConnected)
        {
            robot = null;
            Initialize();
        }

        // Update panel
        if (robot!=null){
            UpdateConnection();
            UpdateStatus();
            if (timestampsToggle.isOn)
            {
                UpdateProfiling();
            }
        }
    }

    /// <summary>
    /// Initialize the control panel
    /// </summary>
    private void Initialize()
    {
        InitializeWithRobot();
        InitializeWithoutRobot();
    }

    /// <summary>
    /// Initialize the control panel when a robot is connected
    /// </summary>
    private void InitializeWithRobot()
    {
        if (robot != null)
        {
            macAddrText.text = "MacAddress is " + robot.MacAddr;

            visualEffectDropdown.value = (int) robot.VisualEffect;
            LEDResponseModeDropdown.value = (int) robot.LedResponseMode;
            casualToggle.isOn = robot.CasualBackdriveAssistEnabled;
            locomotionInteractivityDropdown.value = (int) robot.LocomotionInteractivityMode;
            gestureToggle.isOn = robot.GestureEnabled;
            currentGestureText.text = "Current gesture : " + robot.GetGesture();
            timestampsToggle.isOn = robot.GetTimestampingEnabled();

            framerateText.gameObject.SetActive(timestampsToggle.isOn);
            lastTimestampText.gameObject.SetActive(timestampsToggle.isOn);
        }
    }

    /// <summary>
    /// Initialize the control panel when no robot is connected
    /// </summary>
    private void InitializeWithoutRobot()
    {
        if (robot == null)
        {
            macAddrText.text = "No MacAddress";
            connectionStatusText.text = "Connection Status is: " + ConnectionStatus.ConnectionStatusDisconnected.ToString();
            batteryText.text = "Battery Status is unknown";
            touchText.text = "Touch Keys are unknown";
            kidnappedText.gameObject.SetActive(false);
            visualEffectDropdown.value = 0;
            LEDResponseModeDropdown.value = 0;
            casualToggle.isOn = false;
            locomotionInteractivityDropdown.value = 0;
            gestureToggle.isOn = false;
            currentGestureText.text = "No current gesture";
            timestampsToggle.isOn = false;
            framerateText.gameObject.SetActive(timestampsToggle.isOn);
            lastTimestampText.gameObject.SetActive(timestampsToggle.isOn);
        }
    }

    /// <summary>
    /// Update the connection status
    /// </summary>
    private void UpdateConnection()
    {
        connectionStatusText.text = "Connection Status is: " + ((ConnectionStatus)robot.GetConnectionStatus()).ToString();
    }

    /// <summary>
    /// Update the battery, touched buttons and kidnapped text
    /// </summary>
    private void UpdateStatus()
    {
        batteryText.text = "Battery Status:" + ((BatteryStatus)robot.GetBatteryState()).ToString();

        String s = "";
        for (int k = 0; k < 6; k++)
        {
            switch (robot.TouchKeys[k])
            {
                case Touch.TouchReleased: s = s + k + ":Released "; break;
                case Touch.TouchBegan: s = s + k + ":Touched "; break;
                case Touch.LongTouch: s = s + k + ":Long Touch "; break;
                default: break;
            }
        }

        touchText.text = "Touch Keys: " + s;

        kidnappedText.gameObject.SetActive(robot.Kidnapped);
        currentGestureText.text = "Gesture" + robot.GetGesture();
    }

    /// <summary>
    /// Updates the framerate and last timestamp
    /// </summary>
    private void UpdateProfiling()
    {
        framerateText.text = "Framerate : " + robot.GetFramerate().ToString("0.00") +" fps";
        lastTimestampText.text = "Last timestamp : " + robot.GetLastTimeStamp().ToString() + "ms";
    }

	//////////////////////////////
    /// All click events below ///
	//////////////////////////////

    public void ResetOnClick()
    {
        if (robot != null)
        {
            agent.ResetRobot();
        }
    }

    public void ShutdownOnClick()
    {
        if (robot != null)
        {
            agent.Shutdown();
        }
    }

    public void QueryBatteryStateOnClick()
    {
        if (robot != null)
        {
            robot.QueryBatteryState();
        }
    }

    public void ColorSendOnClick()
    {
        if (robot != null)
        {
            robot.SetLEDResponseMode(LEDResponseModeDropdown.value);
            robot.SetVisualEffect(visualEffectDropdown.value, (long)(redSlider.value * 255), (long)(greenSlider.value * 255), (long) (blueSlider.value * 255), colorValue);
        }
    }

    public void TrackCompletePoseOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalPose(goalXPose, goalYPose, goalThetaPose, goalVPose, goalWPose);
        }
    }

    public void TrackPositionOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalPosition(goalXPose, goalYPose, goalVPose);
        }
    }

    public void TrackOrientationOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalOrientation(goalThetaPose, goalWPose);
        }
    }

    public void TrackXOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalXCoordinate(goalXPose, goalVPose);
        }
    }

    public void TrackYOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalYCoordinate(goalYPose, goalVPose);
        }
    }

    public void TrackXThetaOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalXThetaCoordinate(goalXPose, goalThetaPose, goalVPose, goalWPose);
        }
    }

    public void TrackYThetaOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalYThetaCoordinate(goalYPose, goalThetaPose, goalVPose, goalWPose);
        }
    }

    public void SetVelocityGoalsOnClick()
    {
        if (robot != null)
        {
            robot.SetGoalVelocity(goalVXVelocity, goalVYVelocity, goalWVelocity);
        }
    }

    public void ClearTrackingGoalsOnClick()
    {
        if (robot != null)
        {
            robot.ClearTracking();
        }
    }

    public void GoHapticOnClick()
    {
        if (robot != null)
        {
            robot.SetHapticBackdriveAssist(hapticXCoeff, hapticYCoeff, hapticThetaCoeff);
            casualToggle.isOn = robot.CasualBackdriveAssistEnabled;
        }
    }

    public void GoSimpleVibrationOnClick()
    {
        if (robot != null)
        {
            robot.SimpleVibrate(simpleXIntensity, simpleYIntensity, simpleThetaIntensity, simplePeriod, simpleDuration);
        }
    }

    public void MoveOnIceOnClick(){
        agent.MoveOnIce();
    }

    public void MoveOnMudOnClick(){
        agent.MoveOnMud();
    }

        public void MoveOnStoneOnClick(){
        agent.MoveOnStone();
    }
        public void MoveOnSandpaperOnClick(){
        agent.MoveOnSandpaper();
    }
    public void GoVibrationOnMotionOnClick()
    {
        if (robot != null)
        {
            robot.VibrateOnMotion(motionIntensityCoeff, (uint) motionPeriod);
        }
    }

    public void ClearHapticFeedbackOnClick()
    {
        if (robot != null)
        {
            robot.ClearHapticFeedback();
        }
    }

    public void SetBcastPeriodOnClick()
    {
        if (robot != null)
        {
            robot.SetPoseBcastPeriod(poseBcastPeriod);
        }
    }

	/////////////////////////////////////
    /// All input field entries below ///
	/////////////////////////////////////

    public void OnColorValueChanged(string value)
    {
        bool success = int.TryParse(value, out colorValue);
        if (!success)
        {
            colorValue = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalXPoseChanged(string xg)
    {
        bool success = float.TryParse(xg, out goalXPose);
        if (!success)
        {
            goalXPose = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalYPoseChanged(string yg)
    {
        bool success = float.TryParse(yg, out goalYPose);
        if (!success)
        {
            goalYPose = 0;
            Debug.Log("parsing failed");
        }
    }
    public void OnGoalThetaPoseChanged(string tg)
    {
        bool success = float.TryParse(tg, out goalThetaPose);
        if (!success)
        {
            goalThetaPose = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalVPoseChanged(string vg)
    {
        bool success = float.TryParse(vg, out goalVPose);
        if (!success)
        {
            goalVPose = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalWPoseChanged(string wg)
    {
        bool success = float.TryParse(wg, out goalWPose);
        if (!success)
        {
            goalWPose = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalVXVelocityChanged(string vxg)
    {
        bool success = float.TryParse(vxg, out goalVXVelocity);
        if (!success)
        {
            goalVXVelocity = 0;
            Debug.Log("parsing failed");
        }

    }

    public void OnGoalVYVelocityChanged(string vyg)
    {
        bool success = float.TryParse(vyg, out goalVYVelocity);
        if (!success)
        {
            goalVYVelocity = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnGoalWVelocityChanged(string wg)
    {
        bool success = float.TryParse(wg, out goalWVelocity);
        if (!success)
        {
            goalWVelocity = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnCasualBackdriveAssistChanged(bool isOn)
    {
        if (robot != null)
        {
            robot.SetCasualBackdriveAssistEnabled(isOn);
        }
    }

    public void OnHapticXCoeffChanged(string x)
    {
        bool success = float.TryParse(x, out hapticXCoeff);
        if (!success)
        {
            hapticXCoeff = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnHapticYCoeffChanged(string y)
    {
        bool success = float.TryParse(y, out hapticYCoeff);
        if (!success)
        {
            hapticYCoeff = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnHapticThetaCoeffChanged(string theta)
    {
        bool success = float.TryParse(theta, out hapticThetaCoeff);
        if (!success)
        {
            hapticThetaCoeff = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnSimpleXIntensityChanged(string x)
    {
        bool success = float.TryParse(x, out simpleXIntensity);
        if (!success)
        {
            simpleXIntensity = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnSimpleYIntensityChanged(string y)
    {
        bool success = float.TryParse(y, out simpleYIntensity);
        if (!success)
        {
            simpleYIntensity = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnSimpleThetaIntensityChanged(string theta)
    {
        bool success = float.TryParse(theta, out simpleThetaIntensity);
        if (!success)
        {
            simpleThetaIntensity = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnSimplePeriodChanged(string period)
    {
        bool success = long.TryParse(period, out simplePeriod);
        if (!success)
        {
            simplePeriod = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnSimpleDurationChanged(string duration)
    {
        bool success = long.TryParse(duration, out simpleDuration);
        if (!success)
        {
            simpleDuration = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnMotionIntensityCoeffChanged(string intensity)
    {
        bool success = float.TryParse(intensity, out motionIntensityCoeff);
        if (!success)
        {
            motionIntensityCoeff = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnMotionPeriodChanged(string period)
    {
        bool success = long.TryParse(period, out motionPeriod);
        if (!success)
        {
            motionPeriod = 0;
            Debug.Log("parsing failed");
        }
    }

    public void OnLocomotionInteractivityChanged(int value)
    {
        if (robot != null)
        {
            robot.SetLocomotionInteractivityMode(value);
        }
    }

    public void OnGestureEnabledChanged(bool isOn)
    {
        if (robot != null)
        {
            robot.SetGestureEnabled(isOn);
        }
    }

    public void OnGetOnboardTimestampChanged(bool isOn)
    {
        if (robot != null)
        {
            robot.SetTimestampingEnabled(isOn);

            framerateText.gameObject.SetActive(isOn);
            lastTimestampText.gameObject.SetActive(isOn);
        }
    }

    public void OnPoseBroadcastPeriodChanged(string period)
    {
        bool success = uint.TryParse(period, out poseBcastPeriod);
        if (!success)
        {
            poseBcastPeriod = 0;
            Debug.Log("parsing failed");
        }
    }

}