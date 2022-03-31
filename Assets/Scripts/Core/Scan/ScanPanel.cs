using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class manages the panel that allows the user to scan for cellulo robots
/// </summary>
public class ScanPanel : MonoBehaviour
{
    //!<Cellulos scanner
    private Scanner _scanner;

    //!<The list of the scanned cellulos, with its mac address and game object
    public static Dictionary<string, GameObject> _celluloScannedList = new Dictionary<string, GameObject>();

    //!<Reference to the scanner list panel and his elements
    public GameObject _scannerListPanel;
    public GameObject _scannerListElementPanel;

    //!<Reference to the scan button
    public Button _scanButton;
    //!<Reference to the Custom Addition UI
    public InputField _inputCelluloField;

    private float refreshTimer = Config.REFRESH_TIMER;

	/// <summary>
	/// Start is called before the first frame update
	/// </summary>
    void Start()
    {
        _scanner = new Scanner();
    }

	/// <summary>
	/// Update is called once per frame
	/// </summary>
    void Update()
    {
        if (_scanner.isScanning)
        {
            refreshTimer -= Time.deltaTime;
            if (refreshTimer <= 0f)
            {
                RefreshScanner();
                refreshTimer = Config.REFRESH_TIMER;
            }
            int n_robots = _scanner.GetNumberOfRobotsFound();
            for (int i = 0; i < n_robots; ++i)
            {
                string macAddr = _scanner.GetMacAddrOfRobot(i);
                if (CelluloManager._celluloNumbers.ContainsKey(macAddr))
                {
                    AddRobot(macAddr);
                }
            }
        }
    }

	/// <summary>
	/// Refreshes the scanner, helps to find new devices
	/// </summary>
    private void RefreshScanner()
    {
        _scanner.StopScanning();
        _scanner.StartScanning();
    }

    /// <summary>
	/// Clears the list of robots found
	/// </summary>
    public void ClearList()
    {
        _scanner.ClearRobotsFound();
        foreach (KeyValuePair<string, GameObject> entry in _celluloScannedList)
        {
            Destroy(_celluloScannedList[entry.Key]);
        }
        _celluloScannedList.Clear();
    }
	/// <summary>
	/// Switches the scanner from scanning/not scanning
	/// </summary>
    public void SwitchScannerState()
    {
        if (_scanner.isScanning)
        {
            _scanner.StopScanning();
            _scanButton.GetComponentInChildren<TextMeshProUGUI>().text = "Scan ";
        }
        else
        {
            _scanner.StartScanning();
            _scanButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop ";
        }
    }

	/// <summary>
	/// Connect to all available cellulos
	/// </summary>
    public void ConnectAll()
    {
        List<string> macAddresses = new List<string>();
        List<int> celluloAgentIds = new List<int>();
        
        foreach (KeyValuePair<string, GameObject> entry in _celluloScannedList)
        {
            if (!entry.Value.GetComponent<ScannerListElement>()._isConnected)
            {
                Debug.Log(entry.Value.GetComponent<ScannerListElement>()._isConnected + ": " + entry.Key);
                macAddresses.Add(entry.Key);
            }
        }

        foreach (KeyValuePair<int, GameObject> entry in CelluloManager._celluloList)
        {
            if (!entry.Value.GetComponent<CelluloAgent>().isConnected)
            {
                celluloAgentIds.Add(entry.Key);
            }
        }

        int availableScannedCellulos = macAddresses.Count;
        int availableCelluloAgents = celluloAgentIds.Count;
        int availableCellulos = availableScannedCellulos;
        if (availableScannedCellulos > availableCelluloAgents)
        {
            availableCellulos = availableCelluloAgents;
        }

        for (int i = 0; i < availableCellulos; i++)
        {
            CelluloManager._celluloList[celluloAgentIds[i]].GetComponent<CelluloConnection>().ConnectToCelluloRobot(macAddresses[i]);
        }

    }

    /// <summary>
    /// Disconnect all connected cellulos
    /// </summary>
    public void DisconnectAll()
    {
        List<int> list = new List<int>(CelluloManager._connectedCellulos);
        foreach (int id in list)
        {
            CelluloManager._celluloList[id].GetComponent<CelluloConnection>().DisconnectFromCelluloRobot();
        }

    }

	/// <summary>
	/// Reset all connected cellulos
	/// </summary>
    public void ResetAll()
    {
        List<int> list = new List<int>(CelluloManager._connectedCellulos);
        foreach (int id in list)
        {
            CelluloManager._celluloList[id].GetComponent<CelluloAgent>().ResetRobot();
        }

    }

	/// <summary>
	/// Shutdown all connected cellulos
	/// </summary>
    public void ShutdownAll()
    {
        List<int> list = new List<int>(CelluloManager._connectedCellulos);
        foreach (int id in list)
        {
            CelluloManager._celluloList[id].GetComponent<CelluloAgent>().Shutdown();
        }

    }

	/// <summary>
	/// Add a new found robot to the list
	/// </summary>
	/// <param name="macAddr">
	/// The MAC address of the found robot
	/// </param>
    private void AddRobot(string macAddr)
    {
        if (!_celluloScannedList.ContainsKey(macAddr))
        {
            GameObject newScannerListElementPanel = Instantiate(_scannerListElementPanel, Vector3.zero, Quaternion.identity) as GameObject;
            newScannerListElementPanel.transform.SetParent(_scannerListPanel.transform, false);
            Toggle _toggle = newScannerListElementPanel.GetComponentInChildren<Toggle>();
            _toggle.group = _scannerListPanel.GetComponent<ToggleGroup>();

            _celluloScannedList.Add(macAddr, newScannerListElementPanel);
            ScannerListElement scannerListElement = newScannerListElementPanel.GetComponent<ScannerListElement>();
            scannerListElement.SetMacAddr(macAddr);
            scannerListElement.SetConnected(ConnectionStatus.ConnectionStatusDisconnected);

            SortListPanel();
        }
    }

    private void AddRobot(int nb){
        if(CelluloManager._celluloMacAddresses.ContainsKey(nb))
            AddRobot(CelluloManager._celluloMacAddresses[nb]);
    }

    public void AddCustomRobot(){
        AddRobot(int.Parse(_inputCelluloField.text));
    }

	/// <summary>
	/// Remove a robot from the list
	/// </summary>
	/// <param name="macAddr">
	/// The MAC address of the robot to remove
	/// </param>
    public static void RemoveRobot(string macAddr)
    {
        // Destroy the cellulo robot panel and remove it from the list
        Destroy(_celluloScannedList[macAddr]);
        _celluloScannedList.Remove(macAddr);
    }

	/// <summary>
	/// Sort the cellulo robots in the list by their number
	/// </summary>
    private void SortListPanel()
    {
        if (_celluloScannedList.Count > 1)
        {
            List<GameObject> sorted = _celluloScannedList.OrderBy(x => CelluloManager._celluloNumbers[x.Key]).Select(x => x.Value).ToList();
            for (int i = 0; i < sorted.Count(); i++)
            {
                sorted.ElementAt(i).transform.SetSiblingIndex(i);
            }
        }
    }

	/// <summary>
	/// Called on destroy
	/// </summary>
    void OnDestroy()
    {
        _scanner = null;
    }

	/// <summary>
	/// Called on application quit
	/// </summary>
    void OnApplicationQuit()
    {
        _scanner = null;
    }
}
