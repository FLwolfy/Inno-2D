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
    
    public bool Overlaps(Rect other)
    {
        if (right <= other.left) return false;
        if (left >= other.right) return false;
        if (bottom <= other.top) return false;
        if (top >= other.bottom) return false;
        return true;
    }

    public bool Contains(Rect other)
    {
        return left <= other.left &&
               top <= other.top &&
               right >= other.right &&
               bottom >= other.bottom;
    }
    
    public override string ToString() => $"({x}, {y}, {width}, {height})";
}