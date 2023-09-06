using UnityEngine;

namespace RagdollEngine
{
    public class PointPlayerCameraState : PlayerCameraState
    {
        Volume volume;

        Vector3 position;

        bool point;

        public override void Execute()
        {
            point = PointCameraCheck();

            if (!point) return;

            cameraTransform.position = modelTransform.position
                + Vector3.Slerp(oldPosition,
                    position - modelTransform.position,
                    Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.rotation = Quaternion.Slerp(oldRotation,
                Quaternion.LookRotation(modelTransform.position - cameraTransform.position, Vector3.up),
                1 - Mathf.Sin(Mathf.SmoothStep(0, 1, transition / transitionTime) * Mathf.PI / 2));
        }

        public override bool Check()
        {
            return PointCameraCheck();
        }

        bool PointCameraCheck()
        {
            if (point)
                foreach (Volume thisVolume in volumes)
                    if (thisVolume is PointCameraVolume && thisVolume == volume)
                        return true;

            foreach (Volume thisVolume in volumes)
                if (thisVolume is PointCameraVolume)
                {
                    position = ((PointCameraVolume)thisVolume).point;

                    volume = thisVolume;

                    return true;
                }

            return false;
        }
    }
}
