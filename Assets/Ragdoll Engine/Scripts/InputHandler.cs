using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class InputHandler : MonoBehaviour
    {
        [HideInInspector] public Vector2 move;

        [HideInInspector] public Vector2 lookDelta;

        [HideInInspector] public Vector2 look;

        public Button start = new Button();

        public Button select = new Button();

        public Button jump = new Button();

        public Button roll = new Button();

        public Button stomp = new Button();

        public Button boost = new Button();

        public Button cyloop = new Button();

        public Button attack = new Button();

        public Button sidestep = new Button();

        public Button zoomDelta = new Button();

        public Button zoom = new Button();

        public class Button
        {
            public bool hold => Mathf.Abs(value) >= InputSystem.settings.defaultButtonPressPoint;

            public bool pressed => Mathf.RoundToInt(Time.unscaledTime / Time.unscaledDeltaTime) == Mathf.RoundToInt((Time.inFixedTimeStep ? fixedPressed : framePressed) / Time.unscaledDeltaTime) + 1;

            /*public bool pressed
            {
                get
                {
                    bool returning = Mathf.RoundToInt(Time.unscaledTime / Time.unscaledDeltaTime) == Mathf.RoundToInt((Time.inFixedTimeStep ? fixedPressed : framePressed) / Time.unscaledDeltaTime) + 1;

                    if (returning)
                        print((Time.unscaledTime / Time.unscaledDeltaTime).ToString() + "; " + (Mathf.RoundToInt((Time.inFixedTimeStep ? fixedPressed : framePressed) / Time.unscaledDeltaTime) + 1).ToString());

                    return returning;
                }
            }*/

            public bool released => Mathf.RoundToInt(Time.unscaledTime / Time.unscaledDeltaTime) == Mathf.RoundToInt((Time.inFixedTimeStep ? fixedReleased : frameReleased) / Time.unscaledDeltaTime) + 1;

            public float value;

            float framePressed = -2;

            float fixedPressed = -2;

            float frameReleased = -2;

            float fixedReleased = -2;

            public void Set(float value)
            {
                if (this.value == value) return;

                this.value = value;

                if (hold)
                {
                    framePressed = Time.unscaledTime;

                    if (Time.timeScale > 0)
                        fixedPressed = Time.fixedUnscaledTime;
                }
                else
                {
                    frameReleased = Time.unscaledTime;

                    if (Time.timeScale > 0)
                        fixedReleased = Time.fixedUnscaledTime;
                }
            }
        }
    }
}
