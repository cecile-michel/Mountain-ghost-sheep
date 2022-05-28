//  using System;
//  using UnityEngine;
//  using UnityEditor;
//     public static List<string> layers;
//     public static List<int> layerNumbers;
//     public static string[] layerNames;
//     public static long lastUpdateTick;
     
//      /** Displays a LayerMask field.
//       * \param showSpecial Use the Nothing and Everything selections
//       * \param selected Current LayerMask
//       * \version Unity 3.5 and up will use the EditorGUILayout.MaskField instead of a custom written one.
//       */
//     public static LayerMask LayerMaskField (string label, LayerMask selected, bool showSpecial) {
         
//          //Unity 3.5 and up
         
//          if (layers == null || (System.DateTime.Now.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout)) {
//              lastUpdateTick = System.DateTime.Now.Ticks;
//              if (layers == null) {
//                  layers = new List<string>();
//                  layerNumbers = new List<int>();
//                  layerNames = new string[4];
//              } else {
//                  layers.Clear ();
//                  layerNumbers.Clear ();
//              }
             
//              int emptyLayers = 0;
//              for (int i=0;i<32;i++) {
//                  string layerName = LayerMask.LayerToName (i);
                 
//                  if (layerName != "") {
                     
//                      for (;emptyLayers>0;emptyLayers--) layers.Add ("Layer "+(i-emptyLayers));
//                      layerNumbers.Add (i);
//                      layers.Add (layerName);
//                  } else {
//                      emptyLayers++;
//                  }
//              }
             
//              if (layerNames.Length != layers.Count) {
//                  layerNames = new string[layers.Count];
//              }
//              for (int i=0;i<layerNames.Length;i++) layerNames[i] = layers[i];
//          }
         
//          selected.value =  EditorGUILayout.MaskField (label,selected.value,layerNames);
         
//          return selected;
//      }