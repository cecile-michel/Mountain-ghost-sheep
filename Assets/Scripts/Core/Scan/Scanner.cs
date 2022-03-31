using System;
using System.Runtime.InteropServices;

/// <summary> 
/// This class represents a scanner for cellulo robots
/// </summary>
/// <remarks>
/// It is dependent on an external library "cellulolib" to work correctly
/// </remarks>
public class Scanner {

    // Scanner id for the library
    private long id;
    public bool isScanning { get; private set; }

    /// <summary>
    /// The scanner constructor
    /// </summary>
    public Scanner()
    {
        id = newScanner();
        isScanning = false;
    }

    /// <summary>
    /// Destroys the scanner
    /// </summary>
    ~Scanner()
    {
        StopScanning();
        destroyScanner(id);
    }

    // Library methods
    [DllImport("cellulolib")]
    private static extern long newScanner();
    [DllImport("cellulolib")]
    private static extern void destroyScanner(long id);
    [DllImport("cellulolib")]
    private static extern void startScanning(long id);
    /// <summary>
    /// Start scanning for devices
    /// </summary>
    public void StartScanning()
    {
        if (!isScanning)
        {
            startScanning(id);
            isScanning = true;
        }
    }
    [DllImport("cellulolib")]
    private static extern void stopScanning(long id);
    /// <summary>
    /// Stop scanning for devices
    /// </summary>
    public void StopScanning()
    {
        if (isScanning)
        {
            stopScanning(id);
            isScanning = false;
        }
    }
    [DllImport("cellulolib")]
    private static extern int getNumberOfRobotsFound(long id);
    /// <summary>
    /// Getter for the number of robots found by the scanner
    /// </summary>
    /// <returns>
    /// The number of robots found
    /// </returns>
    public int GetNumberOfRobotsFound() {
        return getNumberOfRobotsFound(id);
    }
    [DllImport("cellulolib", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getMacAddrOfRobot(long id, int index);
    /// <summary>
    /// Getter for the MAC address of the robot at the given index
    /// </summary>
    /// <param name="index">
    /// The index of the robot
    /// </param>
    /// <returns>
    /// The MAC address of the robot as a string
    /// </returns>
    public string GetMacAddrOfRobot(int index) {
        IntPtr ptr = getMacAddrOfRobot(id, index);
        return Marshal.PtrToStringAnsi(ptr, 17);
    }
    [DllImport("cellulolib")]
    private static extern void clearRobotsFound(long id);
    /// <summary>
    /// Clears the list of robots found
    /// </summary>
    public void ClearRobotsFound() {
        clearRobotsFound(id);
    }
}
