namespace Hakoniwa.PluggableAsset.Communication.Pdu
{
    public interface IPduCommTypedData : IPduCommData
    {
        string GetDataName();
        string GetDataTypeName();
    }
}