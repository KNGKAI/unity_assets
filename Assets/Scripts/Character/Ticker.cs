public struct Ticker
{
    public readonly static int DefaultTick = 3;

    private int tick;

    public int Tick
    {
        get
        {
            return (tick);
        }
    }

    public bool Active
    {
        get
        {
            return (tick > 0);
        }
    }

    public void Procces()
    {
        if (tick > 0)
        {
            tick--;
        }
    }

    public void Act()
    {
        Act(DefaultTick);
    }

    public void Act(int a)
    {
        tick = a;
    }

    public void Kill()
    {
        tick = 0;
    }
}