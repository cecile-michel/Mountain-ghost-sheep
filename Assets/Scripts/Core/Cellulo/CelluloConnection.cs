using UnityEngine;
using System;

/// <summary>
/// Manage Connection and its UI of the Cellulo Robot
/// </summary>
public class CelluloConnection : MonoBehaviour
{
    private CelluloAgent agent; //!<The corresponding Cellulo Agent
    public string MacAddr {get; private set;} = null; //!<Mac address of the robot it is connected to
    private ScannerListElement scannerElementList = null;//!<List elements related to this agent
    private CelluloListElement celluloElementList = null;//!< The corresponding Element List (UI) 

    protected void Awake(){
        agent = GetComponent<CelluloAgent>();
    }
    private void Update(){
        // !< Assign the cellulo list element
        if (celluloElementList == null)
        {
            celluloElementList = CelluloManager._celluloPanelList[agent.agentID].GetComponent<CelluloListElement>();
        }
        
        //!< Disconnect in case the robot is null
        if (agent._celluloRobot == null)
        {
            celluloElementList.ResetPanel(ConnectionStatus.ConnectionStatusDisconnected);
            if (scannerElementList != null)
            {
                scannerElementList.SetConnected(ConnectionStatus.ConnectionStatusDisconnected);
                scannerElementList = null;
            }
        }
    }

    /// <summary>
	/// Assigns the corresponding scanner list element when it is connecting or connected to a robot
	/// </summary>
    private void AssignScannerElementList()
    {
        if (MacAddr == null)
        {
            scannerElementList = null;
        }
        else
        {
            scannerElementList = ScanPanel._celluloScannedList[MacAddr].GetComponent<ScannerListElement>();
        }
    }
    	/// <summary>
	/// Button CallBack to Connect to the currently selected robot
	/// </summary>
    public void ConnectToCelluloRobot() {
        ConnectToCelluloRobot(CelluloManager._selectedCelluloToConnectTo);
    }

    /// <summary>
    /// Connects to a robot given the MAC address.
    /// </summary>
    /// <param name="addr">
    /// The Bluetooth MAC address of the real robot.
    /// </param>
    public void ConnectToCelluloRobot(string addr)
    {
        if (addr != null)
        {
            CelluloManager._connectedCellulos.Add(celluloElementList.GetId());
            MacAddr= addr;
            AssignScannerElementList();
            if (!scannerElementList._isConnected)
            {
                agent._celluloRobot = new Cellulo(agent.agentID, this.transform.localPosition);
                agent._celluloRobot.InitRealCellulo(addr);
                (agent._celluloRobot).OnConnectionStatusChanged+=OnConnectionStatusChanged;
                (agent._celluloRobot).OnConnectionStatusChanged+= agent.OnConnectionStatusChanged;
                (agent._celluloRobot).OnShutDown += OnShutDown;
            }
        }
    }

    /// <summary>
    /// Implements the response to a shutdown event: disconnect from a the cellulo robot and update UI. 
    /// </summary>
    private void OnShutDown(object sender, EventArgs e)
    {
        celluloElementList.ResetPanel(ConnectionStatus.ConnectionStatusDisconnected);
        scannerElementList.SetConnected(ConnectionStatus.ConnectionStatusDisconnected);
        Invoke("DisconnectFromCelluloRobot", 1);
    }
    
    /// <summary>
    /// Implements the response to a connection status event: update the connection status and its corresponding UI 
    /// </summary>
    private void OnConnectionStatusChanged(object sender, EventArgs e){
        ConnectionStatus state = agent._celluloRobot.ConnectionStatus;
        celluloElementList.ResetPanel(state);
        scannerElementList.SetConnected(state);
        if(state == ConnectionStatus.ConnectionStatusDisconnected)
            agent._celluloRobot.OnConnectionStatusChanged -= OnConnectionStatusChanged;

    }

    /// <summary>
    /// Disconnect from the robot
    /// </summary>
    public void DisconnectFromCelluloRobot()
    {
        if(agent._celluloRobot!=null)
        {
            CelluloManager._connectedCellulos.Remove(celluloElementList.GetId());
            agent._celluloRobot.DisconnectFromServer();
            AssignScannerElementList();
            MacAddr = null;
        }
    }

    public void OnDestroy(){
#if !(UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
        DisconnectFromCelluloRobot();
#endif
    }

}
