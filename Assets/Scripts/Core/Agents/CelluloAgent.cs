using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class manages a cellulo agent in the game
/// </summary>
public class CelluloAgent : SteeringAgent
{
    public Cellulo _celluloRobot; //!< Reference to the Real Cellulo Robot
    public bool isConnected  = false; //!<Whether isConnected to Real Cellulo Robot or not.
    public bool isMoved  = true; //!<Whether is Moved by the use or not 
    
    public int agentID { get; protected set;} //Agent Unique ID
    protected static int agentCount_static = 0;
    protected float clock_t = 0.0f; //!< Internal Clock in seconds.

    private GameObject _leds;
    [SerializeField]
    private Color initialColor; //!< Initial LED Color
    private float controlPeriod = Config.DEFAULT_CONTROL_PERIOD;  //!< Contol loop perid

	/// <summary>
	/// Called at the start to (1) assign the correct agent ID (2) set intial color. 
	/// </summary>
    protected virtual void Awake()
    {
        AssignID();
        _leds = this.transform.Find("Leds").gameObject;
        SetVisualEffect(0, initialColor, 0);

    }


    /// <summary>
    /// Fixed update loop
    /// </summary>
    protected override void FixedUpdate() {
        clock_t += Time.deltaTime;

        if (_celluloRobot!=null) {
            _celluloRobot.UpdateProperties();
            if(isConnected) RealToUnity();
        }
        base.FixedUpdate();
    }

    /// <summary>
    /// Spawns Cellulo Agent at a specific position and sets its parent.   
    /// </summary>
    /// <param name="parent">
    /// The parent GameObject
    /// </param>
    /// <param name="position">
    /// The position where the agent should be spawned.
    /// </param>

    public void Spawn(GameObject parent, Vector3 position)
    {
        transform.SetParent(parent.transform);
        this.transform.localPosition = position;
    }

	/// <summary>
	/// Assign the correct agent ID
	/// </summary>
    private void AssignID()
    {
        agentID = ++agentCount_static;
        this.name = "CelluloAgent_"+agentID;
    }

    /// <summary>
    /// Sets the control loop period, i.e, the rate (in seconds) at which the velocity commands are sent to the robot.  
    /// </summary>
    /// <param name="controlPeriod">
    /// The control Period in seconds
    /// </param>
    public void SetControlPeriod(float controlPeriod)
    {
        this.controlPeriod = controlPeriod;
    }

    /// <summary>
    /// Reset the Real Robot
    /// </summary>
    public void ResetRobot()
    {
        if (isConnected) _celluloRobot.Reset();
        
    }
    /// <summary>
	/// Set the reference to the robot to Null
	/// </summary>
    private void SetRobotToNull()
    {
        isConnected = false;
        _celluloRobot = null;
    }
	/// <summary>
	/// Shutdown the robot
	/// </summary>
    public void Shutdown()
    {
        if (isConnected) _celluloRobot.Shutdown();
    }

    /// <summary>
    /// Called when the status is changed, properly set the needed parameters.
    /// </summary>
    public void OnConnectionStatusChanged(object sender, EventArgs e)
    {
        if (_celluloRobot.ConnectionStatus == ConnectionStatus.ConnectionStatusConnected)
        {
            isConnected = true; 
            _celluloRobot.OnKidnapped += OnKidnapped;
            _celluloRobot.OnUnKidnapped +=OnUnKidnapped;
            _celluloRobot.OnShutDown += OnShutDown;
            _celluloRobot.OnTouchBegan += OnTouchBegan;
            _celluloRobot.OnTouchReleased += OnTouchReleased;
            _celluloRobot.OnLongTouch += OnLongTouch;
            SetVisualEffect(0, initialColor, 0);
            StartCoroutine(SendVelocityCommandsToRobot());
        }
        else if(_celluloRobot.ConnectionStatus == ConnectionStatus.ConnectionStatusDisconnected)
        {
            _celluloRobot.OnKidnapped -= OnKidnapped;
            _celluloRobot.OnUnKidnapped -=OnUnKidnapped;
            _celluloRobot.OnShutDown -= OnShutDown;
            _celluloRobot.OnTouchBegan -= OnTouchBegan;
            _celluloRobot.OnTouchReleased -= OnTouchReleased;
            _celluloRobot.OnLongTouch -= OnLongTouch;
            StopCoroutine(SendVelocityCommandsToRobot());
            if (isConnected)
            {
                _celluloRobot = null;
                isConnected = false;
            }
        }
    }

    public virtual void OnKidnapped(object sender, EventArgs e){
        BroadcastMessage("OnCelluloKidnapped",SendMessageOptions.DontRequireReceiver);
    }
    public virtual void OnUnKidnapped(object sender, EventArgs e){
        BroadcastMessage("OnCelluloUnKidnapped",SendMessageOptions.DontRequireReceiver);
    }
    public virtual void OnShutDown(object sender, EventArgs e){
        Invoke("SetRobotToNull", 1);
    }

    protected virtual void OnLongTouch(int key)
    {
        BroadcastMessage("OnCelluloLongTouch",key,SendMessageOptions.DontRequireReceiver);
    }
    protected virtual void OnTouchBegan(int key)
    {
        BroadcastMessage("OnCelluloTouchBegan",key,SendMessageOptions.DontRequireReceiver);
    }
    
    protected virtual void OnTouchReleased(int key)
    {
        BroadcastMessage("OnCelluloTouchReleased",key,SendMessageOptions.DontRequireReceiver);
    }
    
    /// <summary>
    /// Sets the real robot velocity from the steering agent velocity
    /// </summary>
    IEnumerator SendVelocityCommandsToRobot() {
    while (true) { 
        if(isConnected && _celluloRobot!=null && !isMoved)
        {
            _celluloRobot.SetGoalVelocity(Config.UnityToRealScaleInX(velocity.x),Config.UnityToRealScaleInY(velocity.z),0);
        }
        yield return new WaitForSecondsRealtime(controlPeriod);
        }
    }

    /// <summary>
    /// Converts the position from real map to unity map
    /// </summary>
    public void RealToUnity(){
        if (isConnected && _celluloRobot.pose.sqrMagnitude>0)
        {
            transform.localPosition = new Vector3(Config.RealToUnityScaleInX(_celluloRobot.pose.x), 0, Config.RealToUnityScaleInY(_celluloRobot.pose.y));
            transform.localRotation = Quaternion.Euler(0, _celluloRobot.pose.z, 0);
        }
    }

    /// <summary>
    /// Sets the visual effect on the robot, changing LED illumination.
    /// </summary>
    /// <remarks>
	/// On virtual robot only VisualEffectConstSingle and VisualEffectConstAll are implemented. The rest are not yet implemented. 
	/// </remarks>
    /// <param name="effect">
    /// The effect ordinal (check VisualEffect enum)
    /// </param>
    /// <param name="color ">
    /// The unity Color value. 
    /// </param>
    /// <param name="value">
    /// A value possibly meaningful for the effect (check VisualEffect)
    /// </param>
    public void SetVisualEffect(VisualEffect effect, Color color, int value){
        switch(effect){
            case VisualEffect.VisualEffectConstSingle:
                _leds.transform.GetChild(value%6).gameObject.GetComponent<Renderer>().materials[0].color = color;
            break;

            case VisualEffect.VisualEffectConstAll:
            default:
                for(int i =0 ; i< _leds.transform.childCount; i++)
                    _leds.transform.GetChild(i).gameObject.GetComponent<Renderer>().materials[0].color = color;
            break; 
        }

        if(isConnected){
            _celluloRobot.SetVisualEffect((long)effect,(int)(color.r*255),(int)(color.g*255),(int)(color.b*255),value);
        }
    }
	
    public void SetSimpleVibrate(float iX, float iY, float iTheta, long period, long duration)
    {
        if (isConnected)
        {
            _celluloRobot.SimpleVibrate(iX, iY, iTheta, period, duration);
        }
    }

    /// <summary>
    /// Enables/disables assist for easy backdriving.
    /// </summary>
    /// <param name="enable">
    /// Whether to enable
    /// </param>

    public void SetCasualBackdriveAssistEnabled(bool enabled)
    {
        if (isConnected && isMoved) _celluloRobot.SetCasualBackdriveAssistEnabled(enabled);
    }

    /// <summary>
    /// Sets the haptic BackDrive Assist
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
        if(isConnected) _celluloRobot.SetHapticBackdriveAssist(xAssist,yAssist,thetaAssist);
    }    

    public void ClearHapticFeedback()
    {
        if (isConnected) _celluloRobot.ClearHapticFeedback();
    }
    public void MoveOnStone()
    {
        if (isConnected)
        {
            _celluloRobot.SetHapticBackdriveAssist(0.6f, 0.6f, 0);
            _celluloRobot.VibrateOnMotion(1, 40);
        }
    }

    public void MoveOnIce()
    {
        if (isConnected)
        {
            _celluloRobot.SetHapticBackdriveAssist(0.9f, 0.9f, 0);
            _celluloRobot.VibrateOnMotion(0, 40);
        }
    }

    public void MoveOnMud(){
        if(isConnected) {
            _celluloRobot.SetHapticBackdriveAssist(0.3f,0.3f,0);
            _celluloRobot.VibrateOnMotion(0, 40);}
    }

    public void MoveOnSandpaper(){
        if(isConnected){
            _celluloRobot.SetHapticBackdriveAssist(-0.3f,-0.3f,0);
            _celluloRobot.VibrateOnMotion(0, 40);
        }
    }
}
