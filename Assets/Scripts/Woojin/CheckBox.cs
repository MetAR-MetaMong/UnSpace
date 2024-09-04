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
    MaterialPropertyBlock _block;
    public eState state;
    public int currentTimeEpoch; // seconds since Unix epoch
    public int startTimeEpoch, endTimeEpoch;

    void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _block = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_block);
        state = eState.Vacant;
        _block.SetColor("_Emission", ColorLibrary.state2Color[(int)state]);
        _meshRenderer.SetPropertyBlock(_block);
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
        _block.SetColor("_Emission", ColorLibrary.state2Color[(int)state]);
        _meshRenderer.SetPropertyBlock(_block);
    }

    public void UpdateState(int state, int startEpoch, int borrowTimeInSeconds) {
        this.state = (eState)state;
        this.startTimeEpoch = startEpoch;
        this.endTimeEpoch = startEpoch + borrowTimeInSeconds;
        _block.SetColor("_Emission", ColorLibrary.state2Color[(int)state]);
        _meshRenderer.SetPropertyBlock(_block);
    }

    private void FixedUpdate() {
        if (endTimeEpoch <= currentTimeEpoch) {
            // Debug.Log("빌리는 시간 끝났음!!!");
            ReactManager.Instance.UpdatePing(this.gameObject);
        }
    }


}