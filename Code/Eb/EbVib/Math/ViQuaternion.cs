using System;

public struct ViQuaternion
{
	public float x;
	public float y;
	public float z;
	public float w;

	public ViQuaternion(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	static public ViQuaternion FromAxisAngle(ViVector3 axis, float radians)
	{
		axis.Normalize();
		ViVector3 scaledAxis = axis * (float)Math.Sin(radians * 0.5f);
		return new ViQuaternion(scaledAxis.x, scaledAxis.y, scaledAxis.z, (float)Math.Cos(radians * 0.5f));
	}

	static public ViQuaternion FromAxisAngle(float x, float y, float z, float fRadians)
	{
		return ViQuaternion.FromAxisAngle(new ViVector3(x, y, z), fRadians);
	}

	//static public Quaternion FromTransform(Matrix3D xfrm)
	//{
	//    Quaternion quat = new Quaternion();

	//    // Check the sum of the diagonal
	//    float tr = xfrm[0, 0] + xfrm[1, 1] + xfrm[2, 2];
	//    if (tr > 0.0f)
	//    {
	//        // The sum is positive
	//        // 4 muls, 1 div, 6 adds, 1 trig function call
	//        float s = (float)Math.Sqrt(tr + 1.0f);
	//        quat.W = s * 0.5f;
	//        s = 0.5f / s;
	//        quat.X = (xfrm[1, 2] - xfrm[2, 1]) * s;
	//        quat.Y = (xfrm[2, 0] - xfrm[0, 2]) * s;
	//        quat.Z = (xfrm[0, 1] - xfrm[1, 0]) * s;
	//    }
	//    else
	//    {
	//        // The sum is negative
	//        // 4 muls, 1 div, 8 adds, 1 trig function call
	//        int[] nIndex = { 1, 2, 0 };
	//        int i, j, k;
	//        i = 0;
	//        if (xfrm[1, 1] > xfrm[i, i])
	//            i = 1;
	//        if (xfrm[2, 2] > xfrm[i, i])
	//            i = 2;
	//        j = nIndex[i];
	//        k = nIndex[j];

	//        float s = (float)Math.Sqrt((xfrm[i, i] - (xfrm[j, j] + xfrm[k, k])) + 1.0f);
	//        quat[i] = s * 0.5f;
	//        if (s != 0.0)
	//        {
	//            s = 0.5f / s;
	//        }
	//        quat[j] = (xfrm[i, j] + xfrm[j, i]) * s;
	//        quat[k] = (xfrm[i, k] + xfrm[k, i]) * s;
	//        quat[3] = (xfrm[j, k] - xfrm[k, j]) * s;
	//    }
	//    return quat;
	//}

	public float this[int index]
	{
		get
		{
			ViDebuger.AssertError(0 <= index && index <= 3);
			if (index <= 1)
			{
				if (index == 0)
				{
					return this.x;
				}
				return this.y;
			}
			if (index == 2)
			{
				return this.z;
			}
			return this.w;
		}
		set
		{
			ViDebuger.AssertError(0 <= index && index <= 3);
			if (index <= 1)
			{
				if (index == 0)
				{
					this.x = value;
				}
				else
				{
					this.y = value;
				}
			}
			else
			{
				if (index == 2)
				{
					this.z = value;
				}
				else
				{
					this.w = value;
				}
			}
		}
	}

	public void GetAxisAngle(out ViVector3 axis, out float radians)
	{
		radians = (float)Math.Acos(this.w);
		if (radians != 0)
		{
			axis = new ViVector3(this.x, this.y, this.z);
			axis /= (float)Math.Sin(radians);
			axis.Normalize();
			radians *= 2;
		}
		else
		{
			axis = ViVector3.UNIT_X;
		}
	}

	static public bool operator ==(ViQuaternion a, ViQuaternion b)
	{
		return (a.x == b.x) && (a.y == b.y) && (a.z == b.z) && (a.w == b.w);
	}

	static public bool operator !=(ViQuaternion a, ViQuaternion b)
	{
		return (a.x != b.x) || (a.y != b.y) || (a.z != b.z) || (a.w != b.w);
	}

	public override bool Equals(object o)
	{
		if (o is ViQuaternion)
		{
			ViQuaternion q = (ViQuaternion)o;
			return (this.x == q.x) && (this.y == q.y) && (this.z == q.z) && (this.w == q.w);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ ((~this.y.GetHashCode()) ^ (this.z.GetHashCode() ^ (~this.w.GetHashCode())));
	}

	static public ViQuaternion operator +(ViQuaternion a, ViQuaternion b)
	{
		return new ViQuaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	static public ViQuaternion operator -(ViQuaternion a, ViQuaternion b)
	{
		return new ViQuaternion(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	static public ViQuaternion operator *(ViQuaternion a, float f)
	{
		return new ViQuaternion(a.x * f, a.y * f, a.z * f, a.w * f);
	}

	static public ViQuaternion operator /(ViQuaternion a, float f)
	{
		if (f == 0)
		{
			throw new DivideByZeroException("Dividing quaternion by zero");
		}
		return new ViQuaternion(a.x / f, a.y / f, a.z / f, a.w / f);
	}

	static public ViVector3 operator *(ViQuaternion a, ViVector3 pt)
	{
		//ViQuaternion quatResult = a * new ViQuaternion(pt.x, pt.y, pt.z, 0) * a.GetUnitInverse();
		//return new ViVector3(quatResult.x, quatResult.y, quatResult.z);
		ViVector3 uv, uuv;
		ViVector3 qvec = new ViVector3(a.x, a.y, a.z);
		uv = ViVector3.Cross(qvec, pt);
		uuv = ViVector3.Cross(qvec, uv); 
		uv *= (2.0f * a.w);
		uuv *= 2.0f;

		return pt + uv + uuv;
	}

	static public ViQuaternion operator *(ViQuaternion a, ViQuaternion b)
	{
		float E = (a.x + a.z) * (b.x + b.y);
		float F = (a.z - a.x) * (b.x - b.y);
		float G = (a.w + a.y) * (b.w - b.z);
		float H = (a.w - a.y) * (b.w + b.z);
		float A = F - E;
		float B = F + E;
		return new ViQuaternion(
			(a.w - a.x) * (b.y + b.z) + (B + G - H) * 0.5f,
			(a.y + a.z) * (b.w - b.x) + (B - G + H) * 0.5f,
			(a.z - a.y) * (b.y - b.z) + (A + G + H) * 0.5f,
			(a.w + a.x) * (b.w + b.x) + (A - G - H) * 0.5f);
	}

	public ViQuaternion GetConjugate()
	{
		return new ViQuaternion(-this.x, -this.y, -this.z, this.w);
	}

	public ViQuaternion GetInverse()
	{
		return this.GetConjugate() / this.GetMagnitudeSquared();
	}

	public ViQuaternion GetUnitInverse()
	{
		return this.GetConjugate();
	}

	public float GetMagnitude()
	{
		return (float)Math.Sqrt(this.w * this.w + this.x * this.x + this.y * this.y + this.z * this.z);
	}

	public float GetMagnitudeSquared()
	{
		return this.w * this.w + this.x * this.x + this.y * this.y + this.z * this.z;
	}

	public void Normalize()
	{
		float length = this.GetMagnitude();
		if (length == 0)
		{
			throw new DivideByZeroException("Can not normalize quaternion when it's magnitude is zero.");
		}
		this.x /= length;
		this.y /= length;
		this.z /= length;
		this.w /= length;
	}

	static public float DotProduct(ViQuaternion a, ViQuaternion b)
	{
		return a.x * b.x +
				a.y * b.y +
				a.z * b.z +
				a.w * b.w;
	}

	static public ViQuaternion Slerp(ViQuaternion a, ViQuaternion b, float t)
	{
		float fScale0, fScale1;
		double dCos = ViQuaternion.DotProduct(a, b);

		if ((1.0 - Math.Abs(dCos)) > 1e-6f)
		{
			double dTemp = Math.Acos(Math.Abs(dCos));
			double dSin = Math.Sin(dTemp);
			fScale0 = (float)(Math.Sin((1.0 - t) * dTemp) / dSin);
			fScale1 = (float)(Math.Sin(t * dTemp) / dSin);
		}
		else
		{
			fScale0 = 1.0f - t;
			fScale1 = t;
		}
		if (dCos < 0.0)
			fScale1 = -fScale1;

		return (a * fScale0) + (b * fScale1);
	}

	public override string ToString()
	{
		return string.Format("( x={0}, y={1}, z={2}, w={3} )", this.x, this.y, this.z, this.w);
	}


	static public readonly ViQuaternion Zero = new ViQuaternion(0, 0, 0, 0);
	static public readonly ViQuaternion Origin = new ViQuaternion(0, 0, 0, 0);
	static public readonly ViQuaternion XAxis = new ViQuaternion(1, 0, 0, 0);
	static public readonly ViQuaternion YAxis = new ViQuaternion(0, 1, 0, 0);
	static public readonly ViQuaternion ZAxis = new ViQuaternion(0, 0, 1, 0);
	static public readonly ViQuaternion WAxis = new ViQuaternion(0, 0, 0, 1);

}