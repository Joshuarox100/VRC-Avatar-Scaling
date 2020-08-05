using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class ASBackup
{
    private readonly Object[][] assets;
    private readonly string[] aliases;

    public ASBackup (Asset[] assets, string[] aliases)
    {
        this.assets = new Object[assets.Length][];
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i].isFolder)
            {
                this.assets[i] = GetFolderContents(assets[i]);
            }
            else
            {
                this.assets[i] = new Object[] { assets[i].Load() };
            }
        }
        this.aliases = aliases;
    }

    private Object[] GetFolderContents(Asset folder)
    {
        string[] guids = AssetDatabase.FindAssets("", new string[] { folder.path });
        Object[] output = new Object[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            output[i] = 
        }
        return output;
    }


}
