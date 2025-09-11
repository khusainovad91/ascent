using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Constants
{
    public const float LIL_ABOVE_FLOOR = 0.1f;
    public const float LIL_ABOVE_CHARACTER = 2f;
    public const float MID_ABOVE_CHARACTER = 2f;
    public const float CAMERA_STOP_DISTANCE = 8f;
    public const float CLICK_ANIMATION_TIME = 0.2f;
    public static Vector3 VECTOR_1 = new Vector3(1f, 1f, 1f);
    public static Vector3 ATTACK_SMALL_CUBE_SIZE = new Vector3(0.2f, 0.2f, 0.2f);
    public static Vector3 DEF_SMALL_CUBE_SIZE = new Vector3(0.2f, 0.2f, 0.2f);
    public static Vector3 NORMAL_CUBE_SIZE = new Vector3(0.4f, 0.4f, 0.4f);
    public static Vector3 MID_OF_COLLIDER = new Vector3(0, 0.5f, 0f);
    public static Vector3 TOP_OF_COLLIDER = new Vector3(0, 1f, 0f);
    public static Vector3 HEAD_LEVEL = new Vector3(0, 0.75f, 0f);

    public const float ENEMY_SKULL_SUN_TIME = 3f;
    public const float DICE_ROLL_TIME = 2f;
    public const float FATE_DICE_RESSET_TIME = 3f;
    public static LayerMask everyThingMask = ~LayerMask.GetMask("Enemys", "Heroes", "Field Objects");
    public static LayerMask obstacleMask = ~LayerMask.GetMask("Obstacles");

    public static Dictionary<Colorr, Color> ColorMap = new Dictionary<Colorr, Color>
    {
        { Colorr.Yellow, new Color(0.9647059f, 0.8980392f, 0.5529412f, 1) },
        { Colorr.Orange, new Color(1, 0.7450981f, 0.4627451f, 1) },
        { Colorr.SlightlyRed, new Color(1, 0.4745098f, 0.4745098f, 1) },
        { Colorr.SlightlyGreen, new Color(0.7294118f, 0.8627451f, 0.345098f, 1) }
    };
}

public enum Colorr
{
    Yellow,
    Orange,
    SlightlyRed,
    SlightlyGreen,
    SlightlyBlue,
    SlightlyOrange,
    MoreOrange,
    Red,
    Green
}
