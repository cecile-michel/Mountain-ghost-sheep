using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary> 
/// The main cellulo manager 
/// </summary> 
[Serializable]
public class MapConfiguration : MonoBehaviour
{
    public PaperSize paper;
    public int RealMapDimensionInX; 
    public int RealMapDimensionInY; 
    public MapOriginPosition RealMapOrigin;  
    public bool ShowRealRobotPosition;
    public float xRobot;
    public float yRobot;
    public float idRobot;

    public GameObject map;
    public GameObject spawedCellulos;

    public void Awake(){
        UpdateParameters();
    }
    public void UpdateParameters(){
        UpdatePaperSize();
        UpdateCelluloUnityScale();
        UpdatePaperOrigin();
    }
    public void FlipXAndY(){
        int buffer = Config.REAL_MAP_DIMENSION_X;
        Config.REAL_MAP_DIMENSION_X = Config.REAL_MAP_DIMENSION_Y;
        Config.REAL_MAP_DIMENSION_Y = buffer;
        RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X;
        RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y;
    }
    public void UpdatePaperOrigin(){
        Config.ORIGIN = RealMapOrigin;
    }
    public void UpdatePaperSize(){
        switch(paper){
            case PaperSize.A0: 
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 841; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 1189; 
                break; 
            case PaperSize.A1: 
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 594 ; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 841; 
                break;
            case PaperSize.A2: 
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 420 ; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 594; 
                break;
            case PaperSize.A3: 
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 297 ; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 420; 
                break;
            case PaperSize.A4: 
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 210 ; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 297; 
                break;
            case PaperSize.Custom: 
                Config.REAL_MAP_DIMENSION_X  = RealMapDimensionInX;
                Config.REAL_MAP_DIMENSION_Y  = RealMapDimensionInY;
                break;
            default:  
                RealMapDimensionInX = Config.REAL_MAP_DIMENSION_X = 297 ; 
                RealMapDimensionInY = Config.REAL_MAP_DIMENSION_Y = 420;  
                break;
        }
    }    

    public void UpdateCelluloUnityScale(){
        CelluloAgent [] agents = Resources.FindObjectsOfTypeAll<CelluloAgent>();
        CelluloAgent ConnectedAgent = null;
        foreach( CelluloAgent agent in agents){
            agent.SetRobotScale();
            if(agent.isConnected)
                ConnectedAgent = agent;
        }
        if(ConnectedAgent!=null){

            xRobot = agents[0]._celluloRobot.pose.x;
            yRobot = agents[0]._celluloRobot.pose.y;
            idRobot = CelluloManager._celluloNumbers[agents[0]._celluloRobot.MacAddr];
        }

        map.transform.localPosition = -new Vector3(0,(Mathf.Ceil(10*Config.GetCelluloScale()*0.448f/2))/10.0f,0);
        if(map.GetComponent<BoxCollider>()!=null)
        {
            Vector3 size = map.GetComponent<BoxCollider>().size;
            size.z = 0;
            map.GetComponent<BoxCollider>().size=size; 
        }
        spawedCellulos.transform.localPosition = Vector3.zero;
    }
}

public enum PaperSize{
    A0, A1, A2, A3, A4, Custom
}
