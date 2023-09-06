using UnityEngine.Splines;

public class RailStageObject : StageObject
{
    public SplineContainer splineContainer;

    void Reset()
    {
        splineContainer = GetComponentInChildren<SplineContainer>();
    }
}
