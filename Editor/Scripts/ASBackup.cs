using System;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;

public class ASBackup
{
    private Asset[] assets;
    private byte[][] backup;

    public ASBackup (AssetList assets)
    {
        this.assets = assets.ToArray();
        backup = new byte[this.assets.Length][];
        for (int i = 0; i < this.assets.Length; i++)
        {
            try
            {
                backup[i] = File.ReadAllBytes(this.assets[i].path);
            }
            catch (Exception err)
            {
                backup[i] = null;
                Debug.LogError(err);
            }
        }
    }

    //Doesn't update already present assets.
    public void AddToBackup(Asset asset)
    {
        Asset[] newAssets = new Asset[assets.Length + 1];
        assets.CopyTo(newAssets, 0);
        newAssets[assets.Length] = asset;
        assets = newAssets;

        byte[][] newBytes = new byte[backup.Length + 1][];
        backup.CopyTo(newBytes, 0);
        try
        {
            newBytes[backup.Length] = File.ReadAllBytes(asset.path);
        }
        catch (Exception err)
        {
            newBytes[backup.Length] = null;
            Debug.LogError(err);
        }
        backup = newBytes;
    }

    public bool RestoreAssets()
    {
        for (int i = 0; i < assets.Length; i++)
        {
            try
            {
                if (backup[i] == null)
                {
                    Debug.LogError("[Avatar Scaling] " + assets[i].name + "could not be restored.");
                }
                else
                {
                    File.WriteAllBytes(assets[i].path, backup[i]);
                }
            }
            catch (Exception err)
            {
                Debug.LogError("[Avatar Scaling] " + assets[i].name + "could not be restored.");
                Debug.LogError(err);
                return false;
            }
        }
        return true;
    }
}
