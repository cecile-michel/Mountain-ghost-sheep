using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages one element of the Scanner cellulo list
/// </summary>
public class ScannerListElement : MonoBehaviour
{

    // Mac address
    private string _macAddr;
    public bool _isConnected { get; private set; }

    [Tooltip("Reference to the selected toggle")]
    public Toggle _updateToggle;
    [Tooltip("Reference to the text label of the toggle")]
    public Text _nameText;
    [Tooltip("Reference to image showing if the Cellulo is connected or not")]
    public Image _isConnectedImage;
    public Button _deleteButton; 

    /// <summary>
    /// Update the connection status and display it accordingly
    /// </summary>
    /// <param name="state">
    /// The current connection state
    /// </param>
    public void SetConnected(ConnectionStatus state)
    {
        switch (state)
        {
            case ConnectionStatus.ConnectionStatusDisconnected:
                _isConnectedImage.color = Color.red;
                _isConnected = false;
                break;
            case ConnectionStatus.ConnectionStatusConnecting:
                _isConnectedImage.color = Color.yellow;
                _updateToggle.isOn = false;
                _isConnected = true;
                break;
            case ConnectionStatus.ConnectionStatusConnected:
                _isConnectedImage.color = Color.green;
                _updateToggle.isOn = false;
                _isConnected = true;
                break;
        }

        // Find and display corresponding id for the mac address
        string number = CelluloManager._celluloNumbers[_macAddr].ToString();
        _nameText.text = "Cellulo " + number;
    }


    /// <summary>
    /// Set the current selected cellulo
    /// </summary>
    /// <param name="on">
    /// Whether this element is selected
    /// </param>
    public void SetSelectedCellulo(bool on)
    {
        if (_isConnected)
        {
            _updateToggle.isOn = false;
            CelluloManager._selectedCelluloToConnectTo = null;
        }
        else if (on)
        {
            CelluloManager._selectedCelluloToConnectTo = _macAddr;
        }
        else
        {
            CelluloManager._selectedCelluloToConnectTo = null;
        }
    }

    public void Remove()
    {
        ScanPanel.RemoveRobot(_macAddr);
    }
    ///////////////////////////
    //// Getters / setters ////
    ///////////////////////////

    public void SetMacAddr(string macAddr) { _macAddr = macAddr; }
    public string GetMacAddr() { return _macAddr; }
}
