using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    public struct DVector3
    {
        private static readonly DVector3 _zero = new DVector3(0, 0, 0);

        public DVector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);
        public static DVector3 Zero => _zero;

        public void Normalize()
        {
            var magnitude = Magnitude;

            X /= magnitude;
            Y /= magnitude;
            Z /= magnitude;
        }

        public static double Distance(DVector3 a, DVector3 b)
        {
            return (a - b).Magnitude;
        }

        public static DVector3 operator+(DVector3 a, Vector3 b)
        {
            return new DVector3(a.X + b.x, a.Y + b.y, a.Z + b.z);
        }
        
        public static DVector3 operator+(DVector3 a, DVector3 b)
        {
            return new DVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static DVector3 operator-(Vector3 a, DVector3 b)
        {
            return new DVector3(a.x - b.X, a.y - b.Y, a.z - b.Z);
        }

        public static DVector3 operator-(DVector3 a, Vector3 b)
        {
            return new DVector3(a.X - b.x, a.Y - b.y, a.Z - b.z);
        }

        public static DVector3 operator-(DVector3 a, DVector3 b)
        {
            return new DVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static DVector3 operator*(DVector3 a, double b)
        {
            return new DVector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static DVector3 operator/(DVector3 a, double b)
        {
            return new DVector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static explicit operator Vector3(DVector3 a)
        {
            return new Vector3((float)a.X, (float)a.Y, (float)a.Z);
        }

        public static implicit operator DVector3(Vector3 a)
        {
            return new DVector3(a.x, a.y, a.z);
        }
    }
}
