using UnityEngine;

namespace RagdollEngine
{
    public class LifePlayerBehaviour : PlayerBehaviour
    {
        bool alive = true;

        public override bool Evaluate()
        {
            return alive;
        }

        public override void Execute()
        {
            if (DeathCheck() && alive)
            {
                alive = false;

                character.Respawn();
            }
        }

        bool DeathCheck()
        {
            foreach (Volume thisVolume in volumes)
                if (thisVolume is DeathVolume)
                    return true;

            return false;
        }
    }
}
