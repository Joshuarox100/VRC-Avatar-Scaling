using System;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class ASBackup
{
    private readonly Asset[] assets;
    private readonly byte[][] backup;

    public ASBackup (AssetList assets)
    {
        this.assets = assets.ToArray();
        backup = new byte[this.assets.Length][];
        for (int i = 0; i < this.assets.Length; i++)
        {
            backup[i] = File.ReadAllBytes(this.assets[i].path);
        }
    }

    public bool RestoreAssets()
    {
        for (int i = 0; i < assets.Length; i++)
        {
            try
            {
                
                File.WriteAllBytes(assets[i].path, backup[i]);
            }
            catch (Exception err)
            {
                Debug.LogError(err);
                return false;
            }
        }
        return true;
    }
}
