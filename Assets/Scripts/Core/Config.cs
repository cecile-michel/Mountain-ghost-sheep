using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// Manages the configuration and constants of the project. 
/// </summary>
public static class Config
{
    //Global variables
    public static int controlPanelSelectedCellulo = 0; 

    // Scanner
    public const float REFRESH_TIMER = 1f; // seconds after which it will stop and start scanning to refresh

    ///////////////////
    //// CONSTANTS ////
    ///////////////////

    // Cellulo constant
    public const int CELLULO_KEYS = 6;
    public const float DEFAULT_CONTROL_PERIOD = 0.1f;

    ///////////////////
    //// SCALING ////
    ///////////////////

    /* Scaling between the real map and the virtual one; 
    The default is that 10 units in unity = min(x dimension of real map, y dimension of real map) */
    public static int REAL_MAP_DIMENSION_X = 420 ; // in mm 
    public static int REAL_MAP_DIMENSION_Y = 297; // in mm 

    public static float CELLULO_SCALE = 1000.0f/ Mathf.Min(REAL_MAP_DIMENSION_X,REAL_MAP_DIMENSION_Y);

    public static float UNITY_MAP_DIMENSION_X = REAL_MAP_DIMENSION_X<=REAL_MAP_DIMENSION_Y?10:10* REAL_MAP_DIMENSION_X/ REAL_MAP_DIMENSION_Y; 
    public static float UNITY_MAP_DIMENSION_Y = REAL_MAP_DIMENSION_Y<= REAL_MAP_DIMENSION_X ? 10 : 10 * REAL_MAP_DIMENSION_Y / REAL_MAP_DIMENSION_X;

    public static float UnityToRealScaleInX(float x){
        return x*REAL_MAP_DIMENSION_X/UNITY_MAP_DIMENSION_X;
    }
    public static float UnityToRealScaleInY(float y){
        return -y*REAL_MAP_DIMENSION_Y/UNITY_MAP_DIMENSION_Y;
    }

    public static float RealToUnityScaleInX(float x){
        return x/REAL_MAP_DIMENSION_X*UNITY_MAP_DIMENSION_X;
    }
    public static float RealToUnityScaleInY(float y){
        return -y/REAL_MAP_DIMENSION_Y*UNITY_MAP_DIMENSION_Y;
    }

}
