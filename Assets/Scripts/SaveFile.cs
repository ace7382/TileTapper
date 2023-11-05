using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public List<string> Objectives;
    public int          Correct;
    public int          Incorrect;
    public int          Current;
    public int          Best;

    public SaveFile()
    {
        Objectives          = new List<string>();

        Correct             = 0;
        Incorrect           = 0;
        Current             = 0;
        Best                = 0;
    }
}
