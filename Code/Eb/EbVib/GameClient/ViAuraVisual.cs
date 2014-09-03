using System;


public class ViAuraVisual<TAvatar>
{
	public void Start(ViVisualAuraStruct info, TAvatar avatar)
	{
		//foreach (AttachExpress expressInfo in info.express)
		//{

		//}
		//foreach (Int32 durationVisualIdx in info.durationVisual)
		//{
		//    ViAvatarDurationVisualStruct durationVisualInfo = ViSealedDB<ViAvatarDurationVisualStruct>.GetData(durationVisualIdx);
		//    if (durationVisualInfo != null)
		//    {

		//    }
		//}
	}
	public void End(TAvatar avatar)
	{
		_expressList.End();
		_durationVisualList.Clear(avatar);
	}
	public void Deactive(TAvatar avatar)
	{
		_durationVisualList.Deactive(avatar);
	}
	public void Active(TAvatar avatar)
	{
		_durationVisualList.Active(avatar);
	}

	ViExpressOwnList<ViExpressInterface> _expressList = new ViExpressOwnList<ViExpressInterface>();
	ViAvatarDurationVisualOwnList<TAvatar> _durationVisualList = new ViAvatarDurationVisualOwnList<TAvatar>();
}