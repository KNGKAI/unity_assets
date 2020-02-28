public struct Ticker
{
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
        Act(3);
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