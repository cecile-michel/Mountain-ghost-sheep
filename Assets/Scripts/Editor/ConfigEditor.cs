using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MapConfiguration)), CanEditMultipleObjects]
public class ConfigEditor : Editor {

    private SerializedProperty _paper;
    private SerializedProperty _dim_x; 
    private SerializedProperty _dim_y; 
    private SerializedProperty _map;
    private SerializedProperty _origin;
    private SerializedProperty _showRealRobotPosition;
    private SerializedProperty _spawedCellulos;

    private SerializedProperty _xrobot;
    private SerializedProperty _yrobot;
    private SerializedProperty _idrobot;
     
    private void OnEnable()
    {
        _paper = serializedObject.FindProperty("paper");    
        _dim_x = serializedObject.FindProperty("RealMapDimensionInX");
        _dim_y = serializedObject.FindProperty("RealMapDimensionInY");
        _map = serializedObject.FindProperty("map");
        _origin = serializedObject.FindProperty("RealMapOrigin");
        _showRealRobotPosition = serializedObject.FindProperty("ShowRealRobotPosition");
        _spawedCellulos = serializedObject.FindProperty("spawedCellulos");
        _xrobot = serializedObject.FindProperty("xRobot");
        _yrobot = serializedObject.FindProperty("yRobot");
        _idrobot = serializedObject.FindProperty("idRobot");
        
    }
    public override void OnInspectorGUI()
    {
        MapConfiguration script = (MapConfiguration) target; 
        serializedObject.Update();
        EditorGUILayout.PropertyField(_map);
        EditorGUILayout.PropertyField(_spawedCellulos);
        EditorGUILayout.PropertyField(_paper);
        
        if(_paper.enumValueIndex == (int) PaperSize.Custom)
        {
            EditorGUILayout.PropertyField(_dim_x);
            EditorGUILayout.PropertyField(_dim_y);
        }
        EditorGUILayout.PropertyField(_origin);
        EditorGUILayout.PropertyField(_showRealRobotPosition);
        if(_showRealRobotPosition.boolValue){
            EditorGUILayout.PropertyField(_idrobot);
            EditorGUILayout.PropertyField(_xrobot);
            EditorGUILayout.PropertyField(_yrobot);
        }

        if(GUILayout.Button("Update Parameters"))
        {     
            script.UpdateParameters();
        }
        
        if(GUILayout.Button("Flip X and Y dimesions"))
        {     
            script.FlipXAndY();
        }
        serializedObject.ApplyModifiedProperties();
    }
}