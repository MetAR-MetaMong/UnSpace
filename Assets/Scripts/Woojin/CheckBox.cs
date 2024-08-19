using UnityEngine;
using UnityEngine.UI;

public enum eState{
    NULL, Vacant, Selected, Occupied, OccupiedSelected
}

public static class ColorLibrary{
    public static Color[] state2Color = new Color[5]{Color.black, Color.blue, Color.red, Color.green, Color.magenta};
}

[RequireComponent(typeof(MeshRenderer))]
public class CheckBox : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    public eState state;
    public int currentTimeEpoch; // seconds since Unix epoch
    public int startTimeEpoch, endTimeEpoch;

    void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sharedMaterial = new Material(_meshRenderer.sharedMaterial);
        state = eState.Vacant;
        _meshRenderer.sharedMaterial.SetColor("_Emission", ColorLibrary.state2Color[(int)state]);
    }
    public void ToggleColor() {
        switch (state) {
            case eState.Vacant:
                state = eState.Selected;
                break;
            case eState.Selected:
                state = eState.Vacant;
                break;
            case eState.Occupied:
                state = eState.OccupiedSelected;
                break;
            case eState.OccupiedSelected:
                state = eState.Occupied;
                break;
        }
        _meshRenderer.sharedMaterial.SetColor("_Emission", ColorLibrary.state2Color[(int)state]);
    }
}