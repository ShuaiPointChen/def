using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ViSpaceExpress : ViExpressInterface
{
	public ViProvider<ViVector3> PosProvider { get { return _posProvider; } }

	protected ViProvider<ViVector3> _posProvider;
}