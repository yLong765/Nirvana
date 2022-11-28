using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public static class CurveUtils
    {
        private static float GetCross(Vector3 p1, Vector3 p2, Vector3 p)
        {
            return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
        }
        
        public static bool IsPosInCurve(Vector3 pos, Vector3 leftCenter, Vector3 rightCenter, float height)
        {
            var offset = new Vector3(0, height * 0.5f, 0);
            var p1 = leftCenter - offset;
            var p2 = leftCenter + offset;
            var p3 = rightCenter - offset;
            var p4 = rightCenter + offset;
            return GetCross(p2, p1, pos) * GetCross(p3, p4, pos) >= 0 &&
                   GetCross(p1, p3, pos) * GetCross(p4, p2, pos) >= 0;
        }
    }
}