using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// This class represents a real cellulo robot
/// </summary>
/// <remarks>
/// This is dependent on an external library "cellulolib" to work correctly
/// Depends on CelluloConfig.cs file as it defines enums
/// </remarks>
public class Cellulo
{
    private long id;

    //!<Properties
    public bool Kidnapped { get; private set; }
    public Touch[] TouchKeys { get; private set; }
    public string MacAddr { get; private set; }
    public ConnectionStatus ConnectionStatus { get; private set; }
    public VisualEffect VisualEffect { get; private set; }
    public LEDResponseMode LedResponseMode { get; private set; }
    public bool CasualBackdriveAssistEnabled { get; private set; }
    public LocomotionInteractivityMode LocomotionInteractivityMode { get; private set; }
    public bool GestureEnabled { get; private set; }
    public int _id; //!<Id inside the game
    public Vector3 pose; //!<Position(x,y,theta) of Cellulo

    private bool IsShuttingDown = false;
    private bool IsLowBattery = false;

    //Events 
    public event EventHandler OnKidnapped; //!< Event when Kidnapping occurs
 
    public event EventHandler OnUnKidnapped; //!< Event when Kidnapping ends   
    public event EventHandler OnShutDown; //!< Event when the robot is shutting down 
    public event EventHandler OnConnectionStatusChanged; //!< Event when connection status is changed 
    public delegate void TouchEventDelegate(int key); //!< Delegate for Touch Event with one parameter being the key number. 
    public event TouchEventDelegate OnTouchBegan; //!<Event when touch began
    public event TouchEventDelegate OnTouchReleased; //!<Event when touch release
    public event TouchEventDelegate OnLongTouch; //!<Event when long touch detected
    

    /// <summary>
    /// Cellulo contructor with no default id or pose
    /// </summary>
    public Cellulo()
    {
        this.id = 0;
        this._id = 0;
        this.pose = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Cellulo contructor
    /// </summary>
    /// <param name="_id">
    /// The id of the robot inside the game
    /// </param>
    /// <param name="pose">
    /// The Initial position of the robot
    /// </param>
    public Cellulo(int _id, Vector3 pose)
    {
        this.id = 0;
        this._id = _id;
        this.pose = pose;
    }

    /// <summary>
    /// Initialize a real robot and connect to it
    /// </summary>
    /// <param name="macAddr">
    /// The MAC address of the robot to connect to
    /// </param>
    public void InitRealCellulo(string macAddr)
    {
        id = newRobot();
        if (id == 0)
        {
            Debug.LogWarning("WARNING: using zero pointer as a Cellulo object. It means that Cellulo object creation failed.");
            return;
        }
        this.MacAddr = macAddr;

        setMacAddr(id,macAddr);
        ConnectToServer();

        // Initial values
        TouchKeys = new Touch[6] { Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased };
        VisualEffect = VisualEffect.VisualEffectConstAll;
        LedResponseMode = LEDResponseMode.LEDResponseModeResponsiveIndividual;
        CasualBackdriveAssistEnabled = false;
        LocomotionInteractivityMode = LocomotionInteractivityMode.LocomotionInteractivityModeNormal;
        GestureEnabled = false;
        IsShuttingDown = false; 
        IsLowBattery = false;

    }

    /// <summary>
    /// Initialize a real robot and connect to it through a specific local adapter. 
    /// </summary>
    /// <param name="macAddr">
    /// The MAC address of the robot to connect to
    /// </param>
    /// <param name="localAdapterMacAddr">
    /// The MAC address of the local adapter
    /// </param>
    public void InitRealCelluloWithLocalAdapter(string macAddr, string localAdapterMacAddr)
    {
        id = newRobot();
        if (id == 0)
        {
            Debug.LogWarning("WARNING: using zero pointer as a Cellulo object. It means that Cellulo object creation failed.");
            return;
        }
        this.MacAddr = macAddr;
        setLocalAdapterMacAddr(id, localAdapterMacAddr);
        setMacAddr(id, macAddr);
        ConnectToServer();

        // Initial values
        TouchKeys = new Touch[6] { Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased, Touch.TouchReleased };
        VisualEffect = VisualEffect.VisualEffectConstAll;
        LedResponseMode = LEDResponseMode.LEDResponseModeResponsiveIndividual;
        CasualBackdriveAssistEnabled = false;
        LocomotionInteractivityMode = LocomotionInteractivityMode.LocomotionInteractivityModeNormal;
        GestureEnabled = false;
        Kidnapped = false;
    }

    /// <summary>
    /// Destroys the cellulo
    /// </summary>
    ~Cellulo()
    {
        if (id!= 0)
        {
            destroyRobot(id);
        }
        id = 0;
    }

    /// <summary>
    /// Update the properties of the cellulo with the latest values
    /// </summary>
    public void UpdateProperties()
    {
        if (id != 0)
        {   
            if (GetKidnapped() != Kidnapped)
            {
                Kidnapped = GetKidnapped();
                if(Kidnapped) OnKidnapped?.Invoke(this, EventArgs.Empty);
                else OnUnKidnapped?.Invoke(this, EventArgs.Empty);
            }
            if (ConnectionStatus != GetConnectionStatus())
            {
                ConnectionStatus = GetConnectionStatus();
                OnConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            }

            pose = new Vector3(GetX(), GetY(), GetTheta());

            if (IsShuttingDown != GetShuttingdown()) {
                IsShuttingDown = GetShuttingdown();
                OnShutDown?.Invoke(this, EventArgs.Empty);
            }
            if(IsLowBattery != GetLowBattery()) {
                IsLowBattery = GetLowBattery();
                OnShutDown?.Invoke(this, EventArgs.Empty);
            }
            for (int i = 0; i < Config.CELLULO_KEYS; i++)
            {
                if (TouchKeys[i] != GetTouch(i))
                {
                    TouchKeys[i] = GetTouch(i);
                    switch (TouchKeys[i])
                    {
                        case Touch.TouchBegan:
                            OnTouchBegan?.Invoke(i);
                            break;
                        case Touch.TouchReleased:
                            OnTouchReleased?.Invoke(i);
                            break;
                        case Touch.LongTouch:
                            OnLongTouch?.Invoke(i);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
    }

    // Cellulo methods export below

    /// <summary>
    /// Initialize the cellulo library, create the cellulo thread, must be called once at the start on the game before connecting to real cellulos
    /// </summary>
    [DllImport("cellulolib")]
    public static extern void initialize();

    /// <summary>
    /// Get a new robot from the pool
    /// </summary>
    /// <remarks>
    /// It is not used on this project
    /// </remarks>
    /// <returns>
    /// The id of the robot in the library
    /// </returns>
    [DllImport("cellulolib")]
    private static extern long newRobotFromPool();

    /// <summary>
    /// Get the total number of robots in the pool
    /// </summary>
    /// <remarks>
    /// It is not used on this project
    /// </remarks>
    /// <returns>
    /// The number of robots
    /// </returns>
    [DllImport("cellulolib")]
    private static extern long totalRobots();

    /// <summary>
    /// Get the number of robots available in the pool
    /// </summary>
    /// <remarks>
    /// It is not used on this project
    /// </remarks>
    /// <returns>
    /// The number of robots available
    /// </returns>
    [DllImport("cellulolib")]
    private static extern long robotsRemaining();

    /// <summary>
    /// Get a new robot
    /// </summary>
    /// <returns>
    /// The id of the robot in the library
    /// </returns>
    [DllImport("cellulolib")]
    private static extern long newRobot();


    /// <summary>
    /// Destroy a robot
    /// </summary>
    /// <param name="id">
    /// The id of the robot in the library
    /// </param>
    [DllImport("cellulolib")]
    private static extern void destroyRobot(long id);

    [DllImport("cellulolib")]
    private static extern void setLocalAdapterMacAddr(long id, string localAdapterMacAddr);
    /// <summary>
    /// Set the local adapater MAC address
    /// </summary>
    /// <param name="localAdapterMacAddr">
    /// The local adapater MAC address
    /// </param>
    public void SetLocalAdapterMacAddr(string localAdapterMacAddr)
    {
        //Debug.Log(id.ToString() + " SetLocalAdapterMacAddr " + localAdapterMacAddr);
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setLocalAdapterMacAddr(id, localAdapterMacAddr);
    }

    [DllImport("cellulolib")]
    private static extern void setAutoConnect(long id, long autoConnect);
    /// <summary>
    /// Set whether it tries to auto connect
    /// </summary>
    /// <param name="autoConnect">
    /// True if it auto connects
    /// </param>
    public void SetAutoConnect(bool autoConnect)
    {
        //Debug.Log(id.ToString() + " SetAutoConnect " + autoConnect.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        long x = 0;
        if (autoConnect)
        {
            x = 1;
        }
        setAutoConnect(id, x);
    }

    [DllImport("cellulolib")]
    private static extern void setMacAddr(long id, string macAddr);
    /// <summary>
    /// Set the real robot MAC address
    /// </summary>
    /// <param name="macAddr">
    /// The MAC address of the robot
    /// </param>
    public void setMacAddr(string macAddr)
    {
        //Debug.Log(id.ToString() + " SetMacAddr " + macAddr);
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setMacAddr(id, macAddr);
    }

    [DllImport("cellulolib")]
    private static extern void connectToServer(long id);
    /// <summary>
    /// Tries to connect to the real robot
    /// </summary>
    public void ConnectToServer()
    {
        //Debug.Log(id.ToString() + " ConnectToServer");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        connectToServer(id);
    }

    [DllImport("cellulolib")]
    private static extern void disconnectFromServer(long id);
    /// <summary>
    /// Disconnects from the connected robot
    /// </summary>
    public void DisconnectFromServer()
    {
        //Debug.Log(id.ToString() + " disconnectFromServer");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        disconnectFromServer(id);
    }

    /////////////////////
    /// Robot Setters ///
    /////////////////////

    [DllImport("cellulolib")]
    private static extern void setGoalVelocity(long robot, float vx, float vy, float w);
    /// <summary>
    /// Sets the goal velocity
    /// </summary>
    /// <param name="vx">
    /// X velocity in mm/s
    /// </param>
    /// <param name="vy">
    /// Y velocity in mm/s
    /// </param>
    /// <param name="w">
    /// Angular velocity in rad/s
    /// </param>
    public void SetGoalVelocity(float vx, float vy, float w)
    {
        //Debug.Log(id.ToString() + " SetGoalVelocity");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalVelocity(id, vx, vy, w);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalPose(long robot, float x, float y, float theta, float v, float w);
    /// <summary>
    /// Sets a pose goal to track
    /// </summary>
    /// <param name="x">
    /// X goal in mm
    /// </param>
    /// <param name="y">
    /// Y goal in mm
    /// </param>
    /// <param name="theta">
    /// Theta goal in degrees
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    /// <param name="w">
    /// Maximum angular speed to track pose in rad/s
    /// </param>
    public void SetGoalPose(float x, float y, float theta, float v, float w)
    {
        //Debug.Log(id.ToString() + " SetGoalPose");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalPose(id, x, y, theta, v, w);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalPosition(long robot, float x, float y, float v);
    /// <summary>
    /// Sets a position goal to track.
    /// </summary>
    /// <param name="x">
    /// X goal in mm
    /// </param>
    /// <param name="y">
    /// Y goal in mm
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    public void SetGoalPosition(float x, float y, float v)
    {
        //Debug.Log(id.ToString() + " SetGoalPosition");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalPosition(id, x, y, v);
    }

    [DllImport("cellulolib")]
    private static extern void clearTracking(long robot);
    /// <summary>
    /// Clears pose/position/velocity goals.
    /// </summary>
    public void ClearTracking()
    {
        //Debug.Log(id.ToString() + " clearTracking");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        clearTracking(id);
    }

    [DllImport("cellulolib")]
    private static extern void clearHapticFeedback(long robot);
    /// <summary>
    /// Clears all haptic feedbacks.
    /// </summary>
    public void ClearHapticFeedback()
    {
        //Debug.Log(id.ToString() + " clearHapticFeedback");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        clearHapticFeedback(id);
    }

    [DllImport("cellulolib")]
    private static extern void setVisualEffect(long robot, long effect, long r, long g, long b, long value);
    /// <summary>
    /// Sets the visual effect on the robot, changing LED illumination.
    /// </summary>
    /// <param name="effect">
    /// The effect ordinal
    /// </param>
    /// <param name="r">
    /// The amount of red color (between 0 and 255)
    /// </param>
    /// <param name="g">
    /// The amount of green color (between 0 and 255)
    /// </param>
    /// <param name="b">
    /// The amount of blue color (between 0 and 255)
    /// </param>
    /// <param name="value">
    /// A value possibly meaningful for the effect (between 0 and 255)
    /// </param>
    public void SetVisualEffect(long effect, long r, long g, long b, long value)
    {
        //Debug.Log(id.ToString() + " SetVisualEffect");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        VisualEffect = (VisualEffect)effect;
        setVisualEffect(id, effect, r, g, b, value);
    }


    [DllImport("cellulolib")]
    private static extern void setCasualBackdriveAssistEnabled(long robot, long enable);
    /// <summary>
    /// Enables/disables assist for easy backdriving.
    /// </summary>
    /// <param name="enable">
    /// Whether to enable
    /// </param>
    public void SetCasualBackdriveAssistEnabled(bool enable)
    {
        //Debug.Log(id.ToString() + " SetCasualBackdriveAssistEnabled " + enable.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        long val = 0;
        if (enable)
        {
            val = 1;
        }
        CasualBackdriveAssistEnabled = enable;
        setCasualBackdriveAssistEnabled(id, val);
    }

    [DllImport("cellulolib")]
    private static extern void setHapticBackdriveAssist(long robot, float xAssist, float yAssist, float thetaAssist);
    /// <summary>
    /// Haptic feedback-oriented backdrive assist.
    /// </summary>
	/// <remarks>
	/// Pass 0 to all parameters to disable. Disables casual backdrive assist upon enabling.
	/// </remarks>
    /// <param name="xAssist">
    /// X assist with respect to x drive velocity, can be negative
    /// </param>
    /// <param name="yAssist">
    /// Y assist with respect to y drive velocity, can be negative
    /// </param>
    /// <param name="thetaAssist">
    /// Theta assist with respect to w drive, can be negative
    /// </param>
    public void SetHapticBackdriveAssist(float xAssist, float yAssist, float thetaAssist)
    {
        //Debug.Log(id.ToString() + " SetHapticBackdriveAssist");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        CasualBackdriveAssistEnabled = false;
        setHapticBackdriveAssist(id, xAssist, yAssist, thetaAssist);
    }

    [DllImport("cellulolib")]
    private static extern void setTimestampingEnabled(long robot, bool enable);
    /// <summary>
    /// Whether the robot will send its own timestamp along with its pose, default false.
    /// </summary>
    /// <param name="enable">
    /// Whether timestamping along with pose is enabled and idling disabled.
    /// </param>
    public void SetTimestampingEnabled(bool enable)
    {
        //Debug.Log(id.ToString() + " SetTimestampingEnabled " + enable.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setTimestampingEnabled(id, enable);
    }

    [DllImport("cellulolib")]
    private static extern void reset(long robot);
    /// <summary>
    /// Initiates a software reSet on the robot.
    /// </summary>
    public void Reset()
    {
        //Debug.Log(id.ToString() + " reSet");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        reset(id);
    }

    [DllImport("cellulolib")]
    private static extern void shutdown(long robot);
    /// <summary>
    /// Initiates sleep on the robot.
    /// </summary>
    public void Shutdown()
    {
        //Debug.Log(id.ToString() + " shutdown");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        shutdown(id);
        OnShutDown?.Invoke(this, EventArgs.Empty);
    }

    [DllImport("cellulolib")]
    private static extern void simpleVibrate(long robot, float iX, float iY, float iTheta, long period, long duration);
    /// <summary>
    /// Constantly vibrates the robot.
    /// </summary>
    /// <param name="iX">
    /// X intensity, scale is the same as linear velocity
    /// </param>
    /// <param name="iY">
    /// Y intensity, scale is the same as linear velocity
    /// </param>
    /// <param name="iTheta">
    /// Theta intensity, scale is the same as angular velocity
    /// </param>
    /// <param name="period">
    /// Period of vibration in milliseconds, maximum is 0xFFFF
    /// </param>
    /// <param name="duration">
    /// Duration of vibration in milliseconds, maximum is 0xFFFF, 0 for vibrate forever
    /// </param>
    public void SimpleVibrate(float iX, float iY, float iTheta, long period, long duration)
    {
        //Debug.Log(id.ToString() + " simpleVibrate");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        simpleVibrate(id, iX, iY, iTheta, period, duration);
    }

    [DllImport("cellulolib")]
    private static extern void setPoseBcastPeriod(long robot, uint period);
    /// <summary>
    /// Sets the pose broadcast period.
    /// </summary>
    /// <param name="period">
    /// Desired period in milliseconds, Set to 0 for as fast as possible, i.e around 93Hz
    /// </param>
    public void SetPoseBcastPeriod(uint period)
    {
        //Debug.Log(id.ToString() + " SetPoseBcastPeriod " + period.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setPoseBcastPeriod(id, period);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalXCoordinate(long robot, float x, float v);
    /// <summary>
    /// Sets an X coordinate to track.
    /// </summary>
    /// <param name="x">
    /// X goal in mm
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    public void SetGoalXCoordinate(float x, float v)
    {
        //Debug.Log(id.ToString() + " SetGoalXCoordinate");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalXCoordinate(id, x, v);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalYCoordinate(long robot, float y, float v);
    /// <summary>
    /// Sets an Y coordinate to track.
    /// </summary>
    /// <param name="y">
    /// Y goal in mm
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    public void SetGoalYCoordinate(float y, float v)
    {
        //Debug.Log(id.ToString() + " SetGoalYCoordinate");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalYCoordinate(id, y, v);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalOrientation(long robot, float theta, float w);
    /// <summary>
    /// Sets a position goal to track.
    /// </summary>
    /// <param name="theta">
    /// Theta goal in degrees
    /// </param>
    /// <param name="w">
    /// Maximum angular speed to track pose in rad/s
    /// </param>
    public void SetGoalOrientation(float theta, float w)
    {
        //Debug.Log(id.ToString() + " SetGoalOrientation");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalOrientation(id, theta, w);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalXThetaCoordinate(long robot, float x, float theta, float v, float w);
    /// <summary>
    /// Sets an X and Theta goal at the same time.
    /// </summary>
    /// <param name="x">
    /// X goal in mm
    /// </param>
    /// <param name="theta">
    /// Theta goal in degrees
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    /// <param name="w">
    /// Maximum angular speed to track pose in rad/s
    /// </param>
    public void SetGoalXThetaCoordinate(float x, float theta, float v, float w)
    {
        //Debug.Log(id.ToString() + " SetGoalXThetaCoordinate");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalXThetaCoordinate(id, x, theta, v, w);
    }

    [DllImport("cellulolib")]
    private static extern void setGoalYThetaCoordinate(long robot, float y, float theta, float v, float w);
    /// <summary>
    /// Sets an Y and Theta goal at the same time.
    /// </summary>
    /// <param name="y">
    /// Y goal in mm
    /// </param>
    /// <param name="theta">
    /// Theta goal in degrees
    /// </param>
    /// <param name="v">
    /// Maximum linear speed to track pose in mm/s
    /// </param>
    /// <param name="w">
    /// Maximum angular speed to track pose in rad/s
    /// </param>
    public void SetGoalYThetaCoordinate(float y, float theta, float v, float w)
    {
        // Debug.Log(id.ToString() + " SetGoalYThetaCoordinate");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setGoalYThetaCoordinate(id, y, theta, v, w);
    }

    [DllImport("cellulolib")]
    private static extern void setExposureTime(long robot, int pixels);
    /// <summary>
    /// Sets the exposure time for super-fast unkidnap detection for uniform and known paper colors known or enables autoexposure.
    /// </summary>
    /// <remarks>
    /// 460 pixels for the latest white plastic frame and white paper is a good value.
    /// </remarks>
    /// <param name="pixels">
    /// Exposure time in pixels, must be larger than 260; 0 is a special value that enables autoexposure
    /// </param>
    public void SetExposureTime(int pixels)
    {
        // Debug.Log(id.ToString() + " SetExposureTime " + pixels.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setExposureTime(id, pixels);
    }

    [DllImport("cellulolib")]
    private static extern void queryBatteryState(long robot);
    /// <summary>
    /// Sends a battery state query.
    /// </summary>
    public void QueryBatteryState()
    {
        // Debug.Log(id.ToString() + " queryBatteryState");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        queryBatteryState(id);
    }

    [DllImport("cellulolib")]
    private static extern void setLEDResponseMode(long robot, int mode);
    /// <summary>
    /// Sets the LED response mode, i.e the LED visual response of the robot to touches.
    /// </summary>
    /// <param name="mode">
    /// LED response mode
    /// </param>
    public void SetLEDResponseMode(int mode)
    {
        // Debug.Log(id.ToString() + " SetLEDResponseMode");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        LedResponseMode = (LEDResponseMode)mode;
        setLEDResponseMode(id, mode);
    }

    [DllImport("cellulolib")]
    private static extern void setLocomotionInteractivityMode(long robot, int mode);
    /// <summary>
    /// Sets the locomotion interactivity mode, i.e the dependance of locomotion to user input.
    /// </summary>
    /// <param name="mode">
    /// Locomotion interactivity mode
    /// </param>
    public void SetLocomotionInteractivityMode(int mode)
    {
        // Debug.Log(id.ToString() + " SetLocomotionInteractivityMode " + mode.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        setLocomotionInteractivityMode(id, mode);
    }

    [DllImport("cellulolib")]
    private static extern void setGestureEnabled(long robot, bool enabled);
    /// <summary>
    /// Enables/disables raw touch signal offSet querying and processing.
    /// </summary>
    /// <param name="enable">
    /// Whether to enable
    /// </param>
    public void SetGestureEnabled(bool enabled)
    {
        // Debug.Log(id.ToString() + " SetGestureEnabled " + enabled.ToString());
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        GestureEnabled = enabled;
        setGestureEnabled(id, enabled);
    }

    [DllImport("cellulolib")]
    private static extern void vibrateOnMotion(long robot, float iCoeff, uint period);
    /// <summary>
    /// Enables vibration against motion.
    /// </summary>
    /// <param name="iCoeff">
    /// Vibration intensity with respect to the drive velocities
    /// </param>
    /// <param name="period">
    /// Period of vibration in milliseconds, maximum is 0xFFFF
    /// </param>
    public void VibrateOnMotion(float iCoeff, uint period)
    {
        // Debug.Log(id.ToString() + " vibrateOnMotion");
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return;
        }
        vibrateOnMotion(id, iCoeff, period);
    }

    /////////////////////
    /// Robot Getters ///
    /////////////////////

    [DllImport("cellulolib")]
    private static extern float getX(long robot);
    /// <summary>
    /// Returns the robot's x coordinate in mm.
    /// </summary>
    /// <returns>
    /// Current x position in mm.
    /// </returns>
    public float GetX()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (0.0f);
        }
        return getX(id);
    }

    [DllImport("cellulolib")]
    private static extern float getY(long robot);
    /// <summary>
    /// Returns the robot's y coordinate in mm.
    /// </summary>
    /// <returns>
    /// Current y position in mm.
    /// </returns>
    public float GetY()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (0.0f);
        }
        return getY(id);
    }

    [DllImport("cellulolib")]
    private static extern float getTheta(long robot);
    /// <summary>
    /// Returns the robot's orientation in degrees.
    /// </summary>
    /// <returns>
    /// Current orientation in degrees.
    /// </returns>
    public float GetTheta()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (0.0f);
        }
        return getTheta(id);
    }

    [DllImport("cellulolib")]
    private static extern bool getKidnapped(long robot);
    /// <summary>
    /// Returns whether the robot is not on encoded paper.
    /// </summary>
    /// <returns>
    /// Whether currently kidnapped.
    /// </returns>
    public bool GetKidnapped()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (false);
        }
        return getKidnapped(id);
    }

    [DllImport("cellulolib")]
    private static extern bool getShuttingdown(long robot);
    /// <summary>
    /// Returns whether the robot is shutting down.
    /// </summary>
    /// <returns>
    /// Whether currently shutting down.
    /// </returns>
    public bool GetShuttingdown()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (false);
        }
        return getShuttingdown(id);
    }

    [DllImport("cellulolib")]
    private static extern bool getLowBattery(long robot);
    /// <summary>
    /// Returns whether the robot has low battery and it will shutdown
    /// </summary>
    /// <returns>
    /// Whether currently it has low battery
    /// </returns>
    public bool GetLowBattery()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (false);
        }
        return getLowBattery(id);
    }


    [DllImport("cellulolib")]
    private static extern int getTouch(long robot, int key);
    /// <summary>
    /// Returns how the robot's touch key is being touched.
    /// </summary>
    /// <param name="key">
    /// The key of the robot which we want the touch mode
    /// </param>
    /// <returns>
    /// The current mode which the key is being touched.
    /// </returns>
    public Touch GetTouch(int key)
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return Touch.TouchReleased;
        }
        return (Touch)getTouch(id, key);
    }

    [DllImport("cellulolib")]
    private static extern int getBatteryState(long robot);
    /// <summary>
    /// Returns the robot's battery state.
    /// </summary>
    /// <returns>
    /// The current robot's battery state.
    /// </returns>
    public int GetBatteryState()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (-1);
        }
        return getBatteryState(id);
    }

    [DllImport("cellulolib")]
    private static extern int getGesture(long robot);
    /// <summary>
    /// Returns the user's current gesture.
    /// </summary>
    /// <returns>
    /// The current user's current gesture.
    /// </returns>
    public Gesture GetGesture()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (Gesture.GestureNone);
        }
        return (Gesture) getGesture(id);
    }


    [DllImport("cellulolib")]
    private static extern bool getTimestampingEnabled(long robot);
    /// <summary>
    /// Returns whether the robot will send its own timestamp along with its pose.
    /// </summary>
    /// <returns>
    /// Whether timestamping along with pose is enabled and idling disabled.
    /// </returns>
    public bool GetTimestampingEnabled()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return false;
        }
        return getTimestampingEnabled(id);
    }

    [DllImport("cellulolib")]
    private static extern int getLastTimeStamp(long robot);
    /// <summary>
    /// Returns the last local timestamp received along with pose (is valid if timestampingEnabled is true).
    /// </summary>
    /// <returns>
    /// Latest received onboard timestamp (in milliseconds).
    /// </returns>
    public int GetLastTimeStamp()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return (-1);
        }
        return getLastTimeStamp(id);
    }

    [DllImport("cellulolib")]
    private static extern float getFramerate(long robot);
    /// <summary>
    /// Returns the localization framerate calculated from local timestamps received along with pose (is valid if timestampingEnabled is true).
    /// </summary>
    /// <returns>
    /// Framerate calculated over time.
    /// </returns>
    public float GetFramerate()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return 0.0f;
        }
        return getFramerate(id);
    }

    [DllImport("cellulolib", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getMacAddr(long robot);
    /// <summary>
    /// Returns the robot MAC address in the form "XX:XX:XX:XX:XX:XX".
    /// </summary>
    /// <returns>
    /// Bluetooth MAC address of the server.
    /// </returns>
    public string GetMacAddr()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            return "";
        }
        IntPtr intptr = (getMacAddr(id));
        MacAddr = Marshal.PtrToStringAnsi(intptr, 17);
        return MacAddr;
    }

    [DllImport("cellulolib")]
    private static extern int getConnectionStatus(long robot);
    /// <summary>
    /// Returns the current connection status to the robot.
    /// </summary>
    /// <returns>
    /// Current connection status.
    /// </returns>
    public ConnectionStatus GetConnectionStatus()
    {
        if (id == 0)
        {
            Debug.LogWarning("Robot is broken (connection to pool failed). Cannot do API call");
            throw new Exception("Invalid robot id");
        }
        return (ConnectionStatus)getConnectionStatus(id);
    }

}