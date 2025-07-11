using MGRect = Microsoft.Xna.Framework.Rectangle;

namespace InnoEngine.Base;

public struct Rect
{
    public int x, y, width, height;

    public int Left => x;
    public int Right => x + width;
    public int Top => y;
    public int Bottom => y + height;

    public Rect(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    internal MGRect ToXnaRect()
    {
        return new MGRect(x, y, width, height);
    }

    internal static Rect FromXnaRect(MGRect r)
    {
        return new Rect(r.X, r.Y, r.Width, r.Height);
    }
}