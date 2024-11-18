using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public VirtualMouseInput virtualMouseInput;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"virtualMouseInput.leftButtonAction.reference.ToInputAction().ReadValue<bool>(): {virtualMouseInput.leftButtonAction.reference.ToInputAction().ReadValue<float>()}");
        Debug.Log($"virtualMouseInput.stickAction.reference.ToInputAction().ReadValue<Vector2>(): {virtualMouseInput.stickAction.reference.ToInputAction().ReadValue<Vector2>()}");
    }
}
