using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct  StaticCommonParams
{
    // InputSystem
    public static string JUMPINPUT = "Jump";
    public static string MOVEINPUT = "Move";
    public static string DASHINPUT = "Fire";
    public static string LOCKONTOGGLEINPUT = "LockonToggle";
    public static string ATTACKINPUT = "BounceBack";

    // Tag
    public static string AttackTag = "Attack";
    public static string WallTag = "Wall";
    public static string GroundTag = "Ground";
    public static string EnemyTag = "Enemy";
    public static string PlayerTag = "Player";
    public static string Obstacle = "Obstacle";
    public static string Untagged = "Untagged";

    //Layer
    public static LayerMask GroundLayer =  1 << 6 ;
    public static LayerMask WallLayer = 1 << 9 ;
    public static LayerMask TargetableLayer = 1 << 7;
    public static LayerMask BulletLayer = 1 << 8;

    //Animaiotn
    public static AnimationCurve NomalizeLiner = AnimationCurve.Linear(0 , 0 ,1 , 1);
    public static AnimationCurve NomalizeConstant = AnimationCurve.Constant(0, 1, 1);
    public static AnimationCurve NomalizeEaseInOut = AnimationCurve.EaseInOut(0 , 0 ,1 ,1);


    //waite
    public static class Yielders
    {

        static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

        static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { return _endOfFrame; }
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { return _fixedUpdate; }
        }

        public static WaitForSeconds Get(float seconds)
        {
            if (!_timeInterval.ContainsKey(seconds))
                _timeInterval.Add(seconds, new WaitForSeconds(seconds));
            return _timeInterval[seconds];
        }
    }
}


