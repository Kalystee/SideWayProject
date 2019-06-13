public class CameraRuleFollowObject : CameraRule
{
    protected override void BeginRule()
    {

    }

    protected override void ApplyRule()
    {
        CameraController.SetFollowTransform(this.transform);
    }

    protected override void StopRule()
    {

    }
}
