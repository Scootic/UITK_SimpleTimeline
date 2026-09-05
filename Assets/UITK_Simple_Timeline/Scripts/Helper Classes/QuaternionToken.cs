using System;
using UnityEngine;

namespace UITK_SimpleTimeline
{
    [Serializable]
    public struct QuaternionToken
    {
        public float x, y, z, w;

        public QuaternionToken(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }
        public QuaternionToken(float X, float Y, float Z, float W) 
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        public static Quaternion FromToken(QuaternionToken qt) 
        {
            Quaternion toReturn = new(qt.x, qt.y, qt.z, qt.w);
            return toReturn;
        }

        public static implicit operator Quaternion(QuaternionToken qt)
        {
            return FromToken(qt);
        }
    }
}
