struct Neighbours
{
    public Chunk left;
    public Chunk right;
    public Chunk bot;
    public Chunk top;
    public Chunk back;
    public Chunk front;

    public Neighbours(Chunk left, Chunk right, Chunk bot, Chunk top, Chunk back, Chunk front)
    {
        this.left = left;
        this.right = right;
        this.bot = bot;
        this.top = top;
        this.back = back;
        this.front = front;
    }
};

public enum Side
{
    LEFT,
    RIGHT,
    BOT,
    TOP,
    BACK,
    FRONT
};