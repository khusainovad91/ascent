using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AttackingCell
{
    public Cell cell;
    public FieldHero target;
    public float range;
    public List<PathNode> pathToCell;

    public AttackingCell(Cell cell, FieldHero target, float range, List<PathNode> pathToCell)
    {
        this.cell = cell;
        this.target = target;
        this.range = range;
        this.pathToCell = pathToCell;
    }   
}
