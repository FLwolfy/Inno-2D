namespace InnoBase;

public struct Rect
{
    public int x, y, width, height;

    public int left => x;
    public int right => x + width;
    public int top => y;
    public int bottom => y + height;

    public Rect(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}