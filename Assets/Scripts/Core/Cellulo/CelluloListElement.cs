using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage one UI element of the Cellulo list
/// </summary>
public class CelluloListElement : MonoBehaviour
{

    // Id of the module
    private int _id = -1;
    [Tooltip("Reference to the connection button")]
    public Button _connectionButton;
    [Tooltip("Reference to the update toggle")]
    public Toggle _updateToggle;
    [Tooltip("Reference to the text label of the toggle")]
    public InputField _idInputField;
    [Tooltip("Reference to image showing if the Cellulo is connected or not")]
    public Image _isConnectionImage;


    ///////////////////
    //// UI EVENTS ////
    ///////////////////

    /// <summary>
    /// Updates the panel with the new connection status
    /// </summary>
    /// <param name="status">
    /// The new connection status
    /// </param>
    public void ResetPanel(ConnectionStatus status)
    {
        
        _connectionButton.onClick.RemoveAllListeners();
        _idInputField.text = _id.ToString();
        switch (status)
        {
            case ConnectionStatus.ConnectionStatusConnected:
                _connectionButton.GetComponentInChildren<Text>().text = "DISCONNECT";
                _connectionButton.onClick.AddListener(() => CelluloManager._celluloList[_id].GetComponent<CelluloConnection>().DisconnectFromCelluloRobot());
                _isConnectionImage.color = Color.green;
                _idInputField.text = CelluloManager._celluloNumbers[CelluloManager._celluloList[_id].GetComponent<CelluloConnection>().MacAddr].ToString();
                break;
            case ConnectionStatus.ConnectionStatusConnecting:
                _connectionButton.GetComponentInChildren<Text>().text = "DISCONNECT";
                _connectionButton.onClick.AddListener(() => CelluloManager._celluloList[_id].GetComponent<CelluloConnection>().DisconnectFromCelluloRobot());
                _isConnectionImage.color = Color.yellow;
                break;
            case ConnectionStatus.ConnectionStatusDisconnected:
                _connectionButton.GetComponentInChildren<Text>().text = "CONNECT";
                _connectionButton.onClick.AddListener(() => CelluloManager._celluloList[_id].GetComponent<CelluloConnection>().ConnectToCelluloRobot());
                _isConnectionImage.color = Color.red;
                break;
            default: 
                break;

        }
    }

    /// <summary>
    /// Set Selected Cellulo for Control Panel
    /// </summary>
    public void SetSelectedCelluloControlPanel(bool on)
    {
        Config.controlPanelSelectedCellulo = _id; 
    }


    /// <summary>
    /// Set the visibility of the connection button
    /// </summary>
    /// <param name="isOn">
    /// Whether the button is visible
    /// </param>
    public void SetConnectionButtonVisibility(bool isOn)
    {
        _connectionButton.gameObject.SetActive(isOn);
    }

    ///////////////////////////
    //// Getters / setters ////
    ///////////////////////////

    public void SetId(int id) {_id = id; }
    public int GetId() { return _id; }
}
