using UnityEngine;
using System.Runtime.InteropServices;
using System;

public enum SpaceList{
    NULL, Arena, Classroom, LabA, LabB, LabC, LabD
}
public enum ObjList{
    NULL, Desk, Whiteboard1, Whiteboard2, Com1, Com2, Com3, Com4
}

public class ReactManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetPlaceCheck (string name);

    public static void Set(GameObject go) {
        string[] words = go.name.Split(' ');
        int space = (int)(SpaceList)Enum.Parse(typeof(SpaceList), words[0]);
        int obj = (int)(ObjList)Enum.Parse(typeof(ObjList), words[1]);
        Debug.Log(space + ", " + obj);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        SetPlaceCheck(go.name);
#endif
    }
}
