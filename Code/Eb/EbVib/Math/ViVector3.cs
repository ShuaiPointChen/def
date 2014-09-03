using System;
using System.Collections.Generic;
using ViArrayIdx = System.Int32;

public struct ViVector3
{
    public const float kEpsilon = 1E-05f;
    public float x;
    public float y;
    public float z;
    public float Length { get { return ViMathDefine.Sqrt((x * x) + (y * y) + (z * z)); } }
    public float Length2 { get { return ((x * x) + (y * y) + (z * z)); } }

    public ViVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public ViVector3(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = 0f;
    }

    public void Scale(ViVector3 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
        this.z *= scale.z;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ ((~this.y.GetHashCode()) ^ (this.z.GetHashCode()));
    }

    public override bool Equals(object other)
    {
        if (!(other is ViVector3))
        {
            return false;
        }
        ViVector3 vector = (ViVector3)other;
        return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
    }

    public void Normalize()
    {
        float num = Magnitude(this);
        if (num > 1E-05f)
        {
            this = (ViVector3)(this / num);
        }
        else
        {
            this = ZERO;
        }
    }

    public static ViVector3 Normalize(ViVector3 value)
    {
        float num = Magnitude(value);
        if (num > 1E-05f)
        {
            return (ViVector3)(value / num);
        }
        return ZERO;
    }

    public ViVector3 normalized
    {
        get
        {
            return Normalize(this);
        }
    }

    public override string ToString()
    {
        return string.Format("({0:F2}, {1:F2}, {2:F2})", this.x, this.y, this.z);
    }

    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2})", this.x.ToString(format), this.y.ToString(format), this.z.ToString(format));
    }

    public static float Dot(ViVector3 lhs, ViVector3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static ViVector3 Cross(ViVector3 v1, ViVector3 v2)
    {
        ViVector3 result;
        result.x = (v1.y * v2.z) - (v1.z * v2.y);
        result.y = (v1.z * v2.x) - (v1.x * v2.z);
        result.z = (v1.x * v2.y) - (v1.y * v2.x);
        return result;
    }

    public static ViVector3 Project(ViVector3 vector, ViVector3 onNormal)
    {
        float num = Dot(onNormal, onNormal);
        if (num < float.Epsilon)
        {
            return ZERO;
        }
        return (ViVector3)((onNormal * Dot(vector, onNormal)) / num);
    }

    public static ViVector3 Exclude(ViVector3 excludeThis, ViVector3 fromThat)
    {
        return (fromThat - Project(fromThat, excludeThis));
    }

    public static float Angle(ViVector3 from, ViVector3 to)
    {
        return (ViMathDefine.Acos(ViMathDefine.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)));
    }

    public static float Distance(ViVector3 a, ViVector3 b)
    {
        float deltaX = a.x - b.x;
        float deltaY = a.y - b.y;
        float deltaZ = a.z - b.z;
        return ViMathDefine.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
    }

    public static float Distance2(ViVector3 a, ViVector3 b)
    {
        float deltaX = a.x - b.x;
        float deltaY = a.y - b.y;
        float deltaZ = a.z - b.z;
        return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
    }

    public static ViVector3 Lerp(ViVector3 from, ViVector3 to, float t)
    {
        ViVector3 v = from;
        v += (to - from) * t;
        return v;
    }

    public static ViVector3 ClampMagnitude(ViVector3 vector, float maxLength)
    {
        if (vector.sqrMagnitude > (maxLength * maxLength))
        {
            return (ViVector3)(vector.normalized * maxLength);
        }
        return vector;
    }

    public static float Magnitude(ViVector3 a)
    {
        return ViMathDefine.Sqrt(((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
    }

    public float magnitude
    {
        get
        {
            return ViMathDefine.Sqrt(((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }

    public static float SqrMagnitude(ViVector3 a)
    {
        return (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
    }

    public float sqrMagnitude
    {
        get
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }

    public static ViVector3 Min(ViVector3 lhs, ViVector3 rhs)
    {
        return new ViVector3(ViMathDefine.Min(lhs.x, rhs.x), ViMathDefine.Min(lhs.y, rhs.y), ViMathDefine.Min(lhs.z, rhs.z));
    }

    public static ViVector3 Max(ViVector3 lhs, ViVector3 rhs)
    {
        return new ViVector3(ViMathDefine.Max(lhs.x, rhs.x), ViMathDefine.Max(lhs.y, rhs.y), ViMathDefine.Max(lhs.z, rhs.z));
    }

    public static ViVector3 ZERO
    {
        get
        {
            return new ViVector3(0f, 0f, 0f);
        }
    }

    public static ViVector3 UNIT_X
    {
        get
        {
            return new ViVector3(1f, 0f, 1f);
        }
    }

    public static ViVector3 UNIT_Y
    {
        get
        {
            return new ViVector3(0f, 1f, 0f);
        }
    }

    public static ViVector3 UNIT_Z
    {
        get
        {
            return new ViVector3(0f, 0f, 1f);
        }
    }

    public static ViVector3 UNIT
    {
        get
        {
            return new ViVector3(1f, 1f, 1f);
        }
    }

    public static ViVector3 operator +(ViVector3 a, ViVector3 b)
    {
        return new ViVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static ViVector3 operator -(ViVector3 a, ViVector3 b)
    {
        return new ViVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static ViVector3 operator -(ViVector3 a)
    {
        return new ViVector3(-a.x, -a.y, -a.z);
    }

    public static ViVector3 operator *(ViVector3 a, float d)
    {
        return new ViVector3(a.x * d, a.y * d, a.z * d);
    }

    public static ViVector3 operator *(float d, ViVector3 a)
    {
        return new ViVector3(a.x * d, a.y * d, a.z * d);
    }

    public static ViVector3 operator /(ViVector3 a, float d)
    {
        return new ViVector3(a.x / d, a.y / d, a.z / d);
    }

    public static bool operator ==(ViVector3 lhs, ViVector3 rhs)
    {
        return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
    }

    public static bool operator !=(ViVector3 lhs, ViVector3 rhs)
    {
        return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
    }
}


public static class ViVector3Serialize
{
    public static void Append(this ViOStream OS, ViVector3 value)
    {
        OS.Append(value.x);
        OS.Append(value.y);
        OS.Append(value.z);
    }

    public static void Read(this ViIStream IS, out ViVector3 value)
    {
        IS.Read(out value.x);
        IS.Read(out value.y);
        IS.Read(out value.z);
    }

    public static bool Read(this ViStringIStream IS, out ViVector3 value)
    {
        value = ViVector3.ZERO;
        if (IS.Read(out value.x) == false) { return false; }
        if (IS.Read(out value.y) == false) { return false; }
        if (IS.Read(out value.z) == false) { return false; }
        return true;
    }

    public static void Read(this string strValue, out ViVector3 value)
    {
        string[] values = strValue.Split(new char[] { ' ' });
        value = new ViVector3();
        if (values.Length == 3)
        {
            value.x = Convert.ToSingle(values[0]);
            value.y = Convert.ToSingle(values[1]);
            value.z = Convert.ToSingle(values[2]);
        }
        else
        {
            ViDebuger.Warning("ViVector3Serialize.Read出错");
        }
    }

    public static void Append(this ViOStream OS, List<ViVector3> list)
    {
        ViArrayIdx size = (ViArrayIdx)list.Count;
        OS.Append(size);
        foreach (ViVector3 value in list)
        {
            OS.Append(value);
        }
    }

    public static void Read(this ViIStream IS, out List<ViVector3> list)
    {
        ViArrayIdx size;
        IS.Read(out size);
        list = new List<ViVector3>((int)size);
        for (ViArrayIdx idx = 0; idx < size; ++idx)
        {
            ViVector3 value;
            IS.Read(out value);
            list.Add(value);
        }
    }

    public static bool Read(this ViStringIStream IS, out List<ViVector3> list)
    {
        ViArrayIdx size;
        IS.Read(out size);
        list = new List<ViVector3>((int)size);
        for (ViArrayIdx idx = 0; idx < size; ++idx)
        {
            ViVector3 value;
            if (IS.Read(out value) == false) { return false; }
            list.Add(value);
        }
        return true;
    }

    public static void Read(this string strValue, out List<ViVector3> list)
    {
        ViDebuger.Error("Not Completed");
        list = new List<ViVector3>();
    }

    public static void PrintTo(this ViVector3 value, ref string strValue)
    {
        strValue += "(";
        strValue += value.x;
        strValue += ", ";
        strValue += value.y;
        strValue += ", ";
        strValue += value.z;
        strValue += ")";
    }

    public static void PrintTo(this List<ViVector3> list, ref string strValue)
    {
        strValue += "(";
        foreach (ViVector3 value in list)
        {
            value.PrintTo(ref strValue);
            strValue += ",";
        }
        strValue += ")";
    }
}
