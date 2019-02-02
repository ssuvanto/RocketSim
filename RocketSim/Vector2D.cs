using Microsoft.Xna.Framework;
using System;

namespace RocketSim {
    public class Vector2D {

        public double X;
        public double Y;

        public static Vector2D Zero { get{ return new Vector2D(0, 0); } protected set {} }

        public Vector2D(double x, double y) {
            X = x;
            Y = y;
        }

        public Vector2D(Vector2D other) {
            X = other.X;
            Y = other.Y;
        }

        public Vector2D(Vector2 floatvec) {
            X = floatvec.X;
            Y = floatvec.Y;
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2D operator *(Vector2D v, double d) {
            return new Vector2D(v.X * d, v.Y * d);
        }

        public static Vector2D operator *(double d, Vector2D v) {
            return new Vector2D(v.X * d, v.Y * d);
        }

        public static Vector2D operator /(Vector2D v, double d) {
            return new Vector2D(v.X / d, v.Y / d);
        }

        public static Vector2D operator /(double d, Vector2D v) {
            return new Vector2D(v.X / d, v.Y / d);
        }

        public double LengthSquared() {
            return X * X + Y * Y;
        }

        public double Length() {
            return Math.Sqrt(LengthSquared());
        }

        public Vector2D Normalized() {
            double len = Length();
            if (len == 0)
                return Zero;
            return new Vector2D(X / len, Y / len);
        }

        public double Dot(Vector2D v) {
            return X * v.X + Y * v.Y;
        }

        public double Angle(Vector2D v) {
            return Math.Acos( Dot(v) / (Length() * v.Length()) );
        }

        public Vector2D Rotate(double radians) {
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            double x = X * cos - Y * sin;
            double y = X * sin + Y * cos;
            return new Vector2D(x, y);
        }

        public Vector2 ToFloatVec() {
            return new Vector2((float)X, (float)Y);
        }

    }
}
