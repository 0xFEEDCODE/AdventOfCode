namespace Framework;

public record Pos2D(int X, int Y);

public class GridPos(int r, int c) : IEquatable<GridPos>
{
    public int R = r;
    public int C = c;

    public bool Equals(GridPos? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return R == other.R && C == other.C;
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return obj.GetType() == GetType() && Equals((GridPos)obj);
    }
    
    public override int GetHashCode() => HashCode.Combine(R, C);
    
    public static bool operator ==(GridPos obj1, GridPos obj2) => obj1.Equals(obj2);
    public static bool operator !=(GridPos obj1, GridPos obj2) => !(obj1 == obj2);
}
