using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class TexturePostProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        importer.anisoLevel = 1;
        importer.filterMode = FilterMode.Bilinear;
        importer.npotScale = TextureImporterNPOTScale.ToLarger;
        importer.textureType = TextureImporterType.Sprite;
        
        // Only post process the dimensions if they are in a 
        // "Maps" folder or a sub folder of it.
        /*string lowerCaseAssetPath = assetPath.ToLower();
        if (lowerCaseAssetPath.IndexOf("/Maps/") == -1)
            return;
        */
        int width, height; 
        GetImageSize(importer,out width,out height);
        importer.mipmapEnabled = false;
        importer.spritePixelsPerUnit =(float) Math.Min(height,width)/10;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
        
    }
    public static bool GetImageSize(TextureImporter importer, out int width, out int height) {
            if (importer != null) {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
     
                width = (int)args[0];
                height = (int)args[1];
     
                return true;
            
        }
     
        height = width = 0;
        return false;
    }
}