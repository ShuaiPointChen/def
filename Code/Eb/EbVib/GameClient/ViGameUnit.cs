using System;

using ViEntityID = System.UInt64;
using ViEntityTypeID = System.Byte;

public interface ViGameUnit : ViEntity, ViRPCEntity
{
	new ViEntityID ID { get; }
	new ViEntityTypeID Type { get; }
	new string Name { get; }

	new void Enable(ViEntityID ID);
	new void PreStart();
	new void Start();
	new void AftStart();
	new void PreEnd();
	new void End();
	new void AftEnd();
	new void Tick(float fDeltaTime);

	bool IsMatch(ViStateConditionStruct condition);
}