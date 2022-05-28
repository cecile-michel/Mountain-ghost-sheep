using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages a cellulo agent in the game
/// </summary>
public class CelluloAgent : SteeringAgent
{
    public Cellulo _celluloRobot; //!< Reference to the Real Cellulo Robot
    [Header ("Real Robot Settings")]
    public bool isConnected = false; //!<Whether isConnected to Real Cellulo Robot or not.
    public bool updateTheta = true; //!<Whether update the rotation of the robot in unity 
    public bool isMoved = true; //!<Whether is Moved by the use or not 
    [Header("Touch and Led Settings")]
    public KeyCode[] simulatedTouch = new KeyCode[Config.CELLULO_KEYS] {KeyCode.T,KeyCode.R,KeyCode.F,KeyCode.V,KeyCode.B,KeyCode.H};
    public Color ledColorWhenTouched = Color.white; 
    public int agentID { get; protected set;} //Agent Unique ID
    protected static int agentCount_static = 0;
    protected float clock_t = 0.0f; //!< Internal Clock in seconds.

    private Vector3 goalPose;
    private bool trackingGoalPose, trackTheta;
    private bool applyingHapticFeedback = false; 

    private GameObject _leds;
    [SerializeField]
    public Color initialColor; //!< Initial LED Color
    private float controlPeriod = Config.DEFAULT_CONTROL_PERIOD;  //!< Contol loop perid
    private Color[] previousColor = new Color[Config.CELLULO_KEYS];

	/// <summary>
	/// Called at the start to (1) assign the correct agent ID (2) set intial color. 
	/// </summary>
    protected virtual void Awake()
    {
        AssignID();
        _leds = this.transform.Find("Leds").gameObject;
        SetVisualEffect(0, initialColor, 0);
        for (int i = 0; i < Config.CELLULO_KEYS; i++)
            previousColor[i] = initialColor;
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

    protected virtual void Update()
    { 
        for (int i = 0; i < Config.CELLULO_KEYS; i++)
        {
            if (Input.GetKeyDown(simulatedTouch[i]))
                OnTouchBegan(i);
            if (Input.GetKeyUp(simulatedTouch[i]))
                OnTouchReleased(i);
        }
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
    public void SetRobotToNull()
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
            SubscribeToRobotEvents();
            SetVisualEffect(0, initialColor, 0);
            StartCoroutine(SendVelocityCommandsToRobot());
        }
        else if(_celluloRobot.ConnectionStatus == ConnectionStatus.ConnectionStatusDisconnected)
        {
            UnsubscribeFromRobotEvents();
            StopCoroutine(SendVelocityCommandsToRobot());
            if (isConnected)
            {
                _celluloRobot = null;
                isConnected = false;
            }
        }
    }
    public void SubscribeToRobotEvents(){
        _celluloRobot.OnKidnapped += OnKidnapped;
        _celluloRobot.OnUnKidnapped +=OnUnKidnapped;
        _celluloRobot.OnShutDown += OnShutDown;
        _celluloRobot.OnTouchBegan += OnTouchBegan;
        _celluloRobot.OnTouchReleased += OnTouchReleased;
        _celluloRobot.OnLongTouch += OnLongTouch;
    }
    public void UnsubscribeFromRobotEvents(){
        _celluloRobot.OnKidnapped -= OnKidnapped;
        _celluloRobot.OnUnKidnapped -=OnUnKidnapped;
        _celluloRobot.OnShutDown -= OnShutDown;
        _celluloRobot.OnTouchBegan -= OnTouchBegan;
        _celluloRobot.OnTouchReleased -= OnTouchReleased;
        _celluloRobot.OnLongTouch -= OnLongTouch;
        _celluloRobot.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        _celluloRobot.OnShutDown -= OnShutDown;
    }

    public virtual void OnKidnapped(object sender, EventArgs e){
        SendMessage("OnCelluloKidnapped",SendMessageOptions.DontRequireReceiver);
    }
    public virtual void OnUnKidnapped(object sender, EventArgs e){
        SendMessage("OnCelluloUnKidnapped",SendMessageOptions.DontRequireReceiver);
    }
    public virtual void OnShutDown(object sender, EventArgs e){
        Invoke("SetRobotToNull", 1);
    }

    protected virtual void OnLongTouch(int key)
    {
        SendMessage("OnCelluloLongTouch",key,SendMessageOptions.DontRequireReceiver);
    }
    protected virtual void OnTouchBegan(int key)
    {
        previousColor[key] = GetLedColor(key);
        SetVisualEffect(VisualEffect.VisualEffectConstSingle, ledColorWhenTouched, key);
        SendMessage("OnCelluloTouchBegan",key,SendMessageOptions.DontRequireReceiver);
    }
    
    protected virtual void OnTouchReleased(int key)
    {

        SetVisualEffect(VisualEffect.VisualEffectConstSingle, previousColor[key], key);
        SendMessage("OnCelluloTouchReleased",key,SendMessageOptions.DontRequireReceiver);
    }
    
    /// <summary>
    /// Sets the real robot velocity from the steering agent velocity
    /// </summary>
    IEnumerator SendVelocityCommandsToRobot() {
    while (true) { 
        
        if(isConnected && _celluloRobot!=null && !trackingGoalPose && (!isMoved ||applyingHapticFeedback))
        {   
            Vector2 velocityToSend = Config.UnityToRealVelocityScale(velocity.x, velocity.z);
            _celluloRobot.SetGoalVelocity(velocityToSend.x,velocityToSend.y,rotation);
        }
        yield return new WaitForSecondsRealtime(controlPeriod);
        }
    }

    /// <summary>
    /// Sets the real robot velocity from the steering agent velocity
    /// </summary>
    IEnumerator CheckIfPoseReached() {
    while (true) { 
        if(isConnected && _celluloRobot!=null && trackingGoalPose)
        {
            if(Math.Abs(goalPose.x - transform.localPosition.x)< Config.goalPoseThreshold
            && Math.Abs(goalPose.y - transform.localPosition.z)< Config.goalPoseThreshold
            && (!trackTheta || Math.Abs(goalPose.z - transform.localRotation.y)< Config.goalRotationThreshold))
                SendMessage("OnGoalPoseReached",SendMessageOptions.DontRequireReceiver);
        }
        yield return new WaitForSecondsRealtime(controlPeriod);
        }
    }
    public virtual void OnGoalPoseReached(){
        StopCoroutine(CheckIfPoseReached());
        trackingGoalPose = false;
        ClearTracking();
    }

    /// <summary>
    /// Set goal pose (position + angle) of the cellulo robot. Disables all the velocity commands sent by unity. 
    /// </summary>
    public void SetGoalPose(float x, float y, float theta, float v, float w){
        goalPose = new Vector3 (x,y,theta);
        trackTheta = true;
        if(isConnected){
            trackingGoalPose = true;
            Vector2 positionToSend = Config.UnityToRealPositionScale(x,y);
            _celluloRobot.SetGoalPose(positionToSend.x,positionToSend.y,theta,v*Config.GetRatioUnityToRealInX(),w);
            StartCoroutine(CheckIfPoseReached());
            
        }
        else{ 
            transform.localPosition = new Vector3(x,0,y);
            transform.localRotation = Quaternion.Euler(0,theta,0);
        }
    }
        /// <summary>
    /// Set goal position of the cellulo robot. Disables all the velocity commands sent by unity. 
    /// </summary>
    public void SetGoalPosition(float x, float y, float v){
        goalPose = new Vector3 (x,y,0);
        trackTheta = false; 
        if(isConnected){
            trackingGoalPose = true;
            Vector2 positionToSend = Config.UnityToRealPositionScale(x,y);
            _celluloRobot.SetGoalPosition(positionToSend.x,positionToSend.y,v*Config.GetRatioUnityToRealInX());
            StartCoroutine(CheckIfPoseReached());
        }
        else{ 
            transform.localPosition = new Vector3(x,0,y);
        }
    }

    /// <summary>
    /// Converts the position from real map to unity map
    /// </summary>
    public void RealToUnity(){
        if (isConnected && _celluloRobot.pose.sqrMagnitude>0)
        {
            transform.localPosition = Config.RealToUnityPositionScale(_celluloRobot.pose.x,_celluloRobot.pose.y);
            if(updateTheta) transform.localRotation = Quaternion.Euler(0, _celluloRobot.pose.z, 0);
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
        SetVisualEffectSimulated(effect, color, value);
        if(isConnected){
            _celluloRobot.SetVisualEffect((long)effect,(int)(color.r*255),(int)(color.g*255),(int)(color.b*255),value);
        }
    }
    public void SetVisualEffectSimulated(VisualEffect effect, Color color, int value)
    {
        switch (effect)
        {
            case VisualEffect.VisualEffectConstSingle:
                _leds.transform.GetChild(value % Config.CELLULO_KEYS).gameObject.GetComponent<Renderer>().materials[0].color = color;
                break;

            case VisualEffect.VisualEffectConstAll:
            default:
                for (int i = 0; i < _leds.transform.childCount; i++)
                    _leds.transform.GetChild(i).gameObject.GetComponent<Renderer>().materials[0].color = color;
                break;
        }
    }

    public Color GetLedColor(int i)
    {
        return _leds.transform.GetChild(i % Config.CELLULO_KEYS).gameObject.GetComponent<Renderer>().materials[0].color;
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
    public void ClearTracking(){
        if(isConnected){
            _celluloRobot.ClearTracking();
        }
    }

    public virtual void OnDestroy(){
        if(isConnected){
            UnsubscribeFromRobotEvents(); 
            SetRobotToNull();
        }

    }
    public void SetRobotScale(){
        transform.localScale = Config.GetCelluloScale()*Vector3.one;
    }
    public void ActivateDirectionalHapticFeedback(){
        applyingHapticFeedback = true; 
    }
    public void DeActivateDirectionalHapticFeedback(){
        applyingHapticFeedback = false; 
        ClearTracking();
    }
}
