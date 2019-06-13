using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule - Look at the Mouse", menuName = "Camera Rules/Sub : Look at the Mouse")]
public class CameraSubRuleLookMouse : CameraSubRule
{
    public override void BeginSubRule()
    {

    }

    public override void ApplySubRule()
    {
        CameraController.Instance.SetLookPosition(CameraController.WorldMousePosition);
    }

    public override void StopSubRule()
    {
        CameraController.Instance.SetLookOffset(Vector2.zero);
    }
}
