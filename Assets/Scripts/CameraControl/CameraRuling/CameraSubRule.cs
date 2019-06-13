using System;
using UnityEngine;

[Serializable]
public abstract class CameraSubRule : ScriptableObject
{
    protected CameraController CameraController { get { return CameraController.Instance; } }

    public abstract void BeginSubRule();
    public abstract void ApplySubRule();
    public abstract void StopSubRule();
}
