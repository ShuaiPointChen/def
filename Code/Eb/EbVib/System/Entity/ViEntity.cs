using System;

using ViEntityTypeID = System.Byte;
using ViEntityID = System.UInt64;


public interface ViEntity
{
	ViEntityID ID { get; }
	ViEntityTypeID Type { get; }
	string Name { get; }

	void Enable(ViEntityID ID);
	void PreStart();
	void Start();
	void AftStart();
	void Tick(float fDeltaTime);
	void PreEnd();
	void End();
	void AftEnd();
}

//public static class ViEntitySerialize
//{
//    public static void Append(this ViOStream OS, ViEntity value)
//    {
//        OS.Append(value.ID);
//    }
//    public static void Read(this ViIStream IS, out ViEntity value)
//    {
//        ViEntityID ID;
//        IS.Read(out ID);
//        value = ViEntityManager.GetEntity(ID);
//    }
//}