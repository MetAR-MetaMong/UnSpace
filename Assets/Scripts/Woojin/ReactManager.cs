using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public enum SpaceList{
    NULL, Arena, Classroom, LabA, LabB, LabC, LabD
}
public enum ObjList{
    NULL, Desk, Whiteboard, Computer
}

public class ReactManager : Singleton<ReactManager>
{
    public List<CheckBox> checkboxes;

    [DllImport("__Internal")] private static extern void SetPlaceCheck (string name);
    [DllImport("__Internal")] private static extern string UpdatePlace (string name);

    public static void SetClick(GameObject go) {
        string[] words = go.name.Split(' ');
        int space = (int)(SpaceList)Enum.Parse(typeof(SpaceList), words[0]);
        int obj = (int)(ObjList)Enum.Parse(typeof(ObjList), words[1]);
        int number = 0;
        if (words.Length > 2) {
            number = int.Parse(words[2]);
        }
        Debug.Log(space + ", " + obj + ", " + number);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        SetPlaceCheck(go.name);
#endif
    }

    public void UpdatePing(GameObject go) {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        string str = UpdatePlace(go.name);
#endif
    }

    public void Fetch(string spaceName, int state, int startEpoch, int borrowTimeInSeconds) {
        for (int i = 0; i < checkboxes.Count; i++) {
            if (checkboxes[i].gameObject.name != spaceName) continue;
            checkboxes[i].UpdateState(state, startEpoch, borrowTimeInSeconds);
        }
        
    }
}
