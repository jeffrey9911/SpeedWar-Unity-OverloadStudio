using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static string AudioVolume { get { return "AudioVolume"; } }
    public static string SelectedKart { get { return "SelectedKart"; } }
    public static string SelectedMode { get { return "SelectedMode"; } }
    public static string SelectedName { get { return "SelectedName"; } }

    public List<string> DataTypes;

    public SceneData()
    {
        DataTypes = new List<string>
        {
            AudioVolume,
            SelectedKart,
            SelectedMode,
            SelectedName
        };
    }
}
