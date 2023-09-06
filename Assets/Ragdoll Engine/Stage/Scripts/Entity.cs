using System;

public class Entity : StageObject
{
    public event EventHandler Destroyed;

    public virtual void OnDestroy()
    {
        Destroyed?.Invoke(this, EventArgs.Empty);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
