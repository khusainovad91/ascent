using UnityEngine;

public class CellWalls
{
    //north wall (северная(
    public bool up;
    //south
    public bool down;
    //west 
    public bool left;
    //east
    public bool right;

    public override string ToString()
    {
        return "up: " + this.up + "\n down: " + this.down + "\n left: " + this.left + "\n right: " + this.right; 
    }
}
