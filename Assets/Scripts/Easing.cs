using UnityEngine;
using System;

public enum EaseType
{
    Lerp,
    QuadIn,
    QuadOut,
    QuadInOut,
    QuadOutIn,
    CubicIn,
    CubicOut,
    CubicInOut,
    CubicOutIn,
    QuartIn,
    QuartOut,
    QuartInOut,
    QuartOutIn,
    QuintIn,
    QuintOut,
    QuintInOut,
    QuintOutIn,
    SineIn,
    SineOut,
    SineInOut,
    SineOutIn,
    ExpoIn,
    ExpoOut,
    ExpoInOut,
    ExpoOutIn,
    CircIn,
    CircOut,
    CircInOut,
    CircOutIn,
    ElasticIn,
    ElasticOut,
    ElasticInOut,
    ElasticOutIn,
    BackIn,
    BackOut,
    BackInOut,
    BackOutIn,
    BounceIn,
    BounceOut,
    BounceInOut,
    BounceOutIn
}

public static class Easing
{
    #region Public Interface (Enum)

    public static double Ease(EaseType type, double t)
    {
        switch (type)
        {
            case EaseType.Lerp:
                return Lerp(t);
            case EaseType.QuadIn:
                return QuadIn(t);
            case EaseType.QuadOut:
                return QuadOut(t);
            case EaseType.QuadInOut:
                return QuadInOut(t);
            case EaseType.QuadOutIn:
                return QuadOutIn(t);
            case EaseType.CubicIn:
                return CubicIn(t);
            case EaseType.CubicOut:
                return CubicOut(t);
            case EaseType.CubicInOut:
                return CubicInOut(t);
            case EaseType.CubicOutIn:
                return CubicOutIn(t);
            case EaseType.QuartIn:
                return QuartIn(t);
            case EaseType.QuartOut:
                return QuartOut(t);
            case EaseType.QuartInOut:
                return QuartInOut(t);
            case EaseType.QuartOutIn:
                return QuartOutIn(t);
            case EaseType.QuintIn:
                return QuintIn(t);
            case EaseType.QuintOut:
                return QuintOut(t);
            case EaseType.QuintInOut:
                return QuintInOut(t);
            case EaseType.QuintOutIn:
                return QuintOutIn(t);
            case EaseType.SineIn:
                return SineIn(t);
            case EaseType.SineOut:
                return SineOut(t);
            case EaseType.SineInOut:
                return SineInOut(t);
            case EaseType.SineOutIn:
                return SineOutIn(t);
            case EaseType.ExpoIn:
                return ExpoIn(t);
            case EaseType.ExpoOut:
                return ExpoOut(t);
            case EaseType.ExpoInOut:
                return ExpoInOut(t);
            case EaseType.ExpoOutIn:
                return ExpoOutIn(t);
            case EaseType.CircIn:
                return CircIn(t);
            case EaseType.CircOut:
                return CircOut(t);
            case EaseType.CircInOut:
                return CircInOut(t);
            case EaseType.CircOutIn:
                return CircOutIn(t);
            case EaseType.ElasticIn:
                return ElasticIn(t);
            case EaseType.ElasticOut:
                return ElasticOut(t);
            case EaseType.ElasticInOut:
                return ElasticInOut(t);
            case EaseType.ElasticOutIn:
                return ElasticOutIn(t);
            case EaseType.BackIn:
                return BackIn(t);
            case EaseType.BackOut:
                return BackOut(t);
            case EaseType.BackInOut:
                return BackInOut(t);
            case EaseType.BackOutIn:
                return BackOutIn(t);
            case EaseType.BounceIn:
                return BounceIn(t);
            case EaseType.BounceOut:
                return BounceOut(t);
            case EaseType.BounceInOut:
                return BounceInOut(t);
            case EaseType.BounceOutIn:
                return BounceOutIn(t);
        }

        return 0.0;
    }

    #endregion

    #region Public Interface (Functions)

    public static double Lerp(double t)
    {
        return In(Linear, t, 0, 1);
    }

    public static double QuadIn(double t)
    {
        return In(Quad, t, 0, 1);
    }

    public static double QuadOut(double t)
    {
        return Out(Quad, t, 0, 1);
    }

    public static double QuadInOut(double t)
    {
        return InOut(Quad, t, 0, 1);
    }

    public static double QuadOutIn(double t)
    {
        return OutIn(Quad, t, 0, 1);
    }

    public static double CubicIn(double t)
    {
        return In(Cubic, t, 0, 1);
    }

    public static double CubicOut(double t)
    {
        return Out(Cubic, t, 0, 1);
    }

    public static double CubicInOut(double t)
    {
        return InOut(Cubic, t, 0, 1);
    }

    public static double CubicOutIn(double t)
    {
        return OutIn(Cubic, t, 0, 1);
    }

    public static double QuartIn(double t)
    {
        return In(Quart, t, 0, 1);
    }

    public static double QuartOut(double t)
    {
        return Out(Quart, t, 0, 1);
    }

    public static double QuartInOut(double t)
    {
        return InOut(Quart, t, 0, 1);
    }

    public static double QuartOutIn(double t)
    {
        return OutIn(Quart, t, 0, 1);
    }

    public static double QuintIn(double t)
    {
        return In(Quint, t, 0, 1);
    }

    public static double QuintOut(double t)
    {
        return Out(Quint, t, 0, 1);
    }

    public static double QuintInOut(double t)
    {
        return InOut(Quint, t, 0, 1);
    }

    public static double QuintOutIn(double t)
    {
        return OutIn(Quint, t, 0, 1);
    }

    public static double SineIn(double t)
    {
        return In(Sine, t, 0, 1);
    }

    public static double SineOut(double t)
    {
        return Out(Sine, t, 0, 1);
    }

    public static double SineInOut(double t)
    {
        return InOut(Sine, t, 0, 1);
    }

    public static double SineOutIn(double t)
    {
        return OutIn(Sine, t, 0, 1);
    }

    public static double ExpoIn(double t)
    {
        return In(Expo, t, 0, 1);
    }

    public static double ExpoOut(double t)
    {
        return Out(Expo, t, 0, 1);
    }

    public static double ExpoInOut(double t)
    {
        return InOut(Expo, t, 0, 1);
    }

    public static double ExpoOutIn(double t)
    {
        return OutIn(Expo, t, 0, 1);
    }

    public static double CircIn(double t)
    {
        return In(Circ, t, 0, 1);
    }

    public static double CircOut(double t)
    {
        return Out(Circ, t, 0, 1);
    }

    public static double CircInOut(double t)
    {
        return InOut(Circ, t, 0, 1);
    }

    public static double CircOutIn(double t)
    {
        return OutIn(Circ, t, 0, 1);
    }

    public static double ElasticIn(double t)
    {
        return In(Elastic, t, 0, 1);
    }

    public static double ElasticOut(double t)
    {
        return Out(Elastic, t, 0, 1);
    }

    public static double ElasticInOut(double t)
    {
        return InOut(Elastic, t, 0, 1);
    }

    public static double ElasticOutIn(double t)
    {
        return OutIn(Elastic, t, 0, 1);
    }

    public static double BackIn(double t)
    {
        return In(Back, t, 0, 1);
    }

    public static double BackOut(double t)
    {
        return Out(Back, t, 0, 1);
    }

    public static double BackInOut(double t)
    {
        return InOut(Back, t, 0, 1);
    }

    public static double BackOutIn(double t)
    {
        return OutIn(Back, t, 0, 1);
    }

    public static double BounceIn(double t)
    {
        return In(Bounce, t, 0, 1);
    }

    public static double BounceOut(double t)
    {
        return Out(Bounce, t, 0, 1);
    }

    public static double BounceInOut(double t)
    {
        return InOut(Bounce, t, 0, 1);
    }

    public static double BounceOutIn(double t)
    {
        return OutIn(Bounce, t, 0, 1);
    }

    #endregion

    #region Ease Types

    private static double In(Func<double, double, double> ease_f, double t, double b, double c, double d = 1)
    {
        if (t >= d)
            return b + c;
        if (t <= 0)
            return b;

        return c * ease_f(t, d) + b;
    }

    private static double Out(Func<double, double, double> ease_f, double t, double b, double c, double d = 1)
    {
        if (t >= d)
            return b + c;
        if (t <= 0)
            return b;

        return (b + c) - c * ease_f(d - t, d);
    }

    private static double InOut(Func<double, double, double> ease_f, double t, double b, double c, double d = 1)
    {
        if (t >= d)
            return b + c;
        if (t <= 0)
            return b;

        if (t < d / 2)
            return In(ease_f, t * 2, b, c / 2, d);

        return Out(ease_f, (t * 2) - d, b + c / 2, c / 2, d);
    }

    private static double OutIn(Func<double, double, double> ease_f, double t, double b, double c, double d = 1)
    {
        if (t >= d)
            return b + c;
        if (t <= 0)
            return b;

        if (t < d / 2)
            return Out(ease_f, t * 2, b, c / 2, d);

        return In(ease_f, (t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Equations

    private static double Linear(double t, double d = 1)
    {
        return t / d;
    }

    private static double Quad(double t, double d = 1)
    {
        return (t /= d) * t;
    }

    private static double Cubic(double t, double d = 1)
    {
        return (t /= d) * t * t;
    }

    private static double Quart(double t, double d = 1)
    {
        return (t /= d) * t * t * t;
    }

    private static double Quint(double t, double d = 1)
    {
        return (t /= d) * t * t * t * t;
    }

    private static double Sine(double t, double d = 1)
    {
        return 1 - Math.Cos(t / d * (Math.PI / 2));
    }

    private static double Expo(double t, double d = 1)
    {
        return Math.Pow(2, 10 * (t / d - 1));
    }

    private static double Circ(double t, double d = 1)
    {
        return -(Math.Sqrt(1 - (t /= d) * t) - 1);
    }

    private static double Elastic(double t, double d = 1)
    {
        t /= d;
        var p = d * .3;
        var s = p / 4;
        return -(Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Mathf.PI) / p));
    }

    private static double Back(double t, double d = 1)
    {
        return (t /= d) * t * ((1.70158 + 1) * t - 1.70158);
    }

    private static double Bounce(double t, double d = 1)
    {
        t = d - t;
        if ((t /= d) < (1 / 2.75))
            return 1 - (7.5625 * t * t);
        else if (t < (2 / 2.75))
            return 1 - (7.5625 * (t -= (1.5 / 2.75)) * t + .75);
        else if (t < (2.5 / 2.75))
            return 1 - (7.5625 * (t -= (2.25 / 2.75)) * t + .9375);
        else
            return 1 - (7.5625 * (t -= (2.625 / 2.75)) * t + .984375);
    }

    #endregion
}