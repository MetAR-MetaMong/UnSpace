using UnityEngine;
using System.Runtime.InteropServices;

public enum PlaceList{
    Desk, Whiteboard1, Whiteboard2, Com1, Com2, Com3, Com4
}

public class ReactManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetPlaceCheck (int id);

    public static void Set(GameObject go) {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        string[] words = go.name.Split(' ');
        SetPlaceCheck ((int)(MyEnum)Enum.Parse(typeof(PlaceList), words[1]));
#endif
    }
}
