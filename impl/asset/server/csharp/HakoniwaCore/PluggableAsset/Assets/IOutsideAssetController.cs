using System;
using System.Collections.Generic;
using System.Text;

namespace Hakoniwa.PluggableAsset.Assets
{
    public interface IOutsideAssetController : IAssetController
    {
        long GetSimTime(); //usec
        void PutHakoniwaTime(long time); //usec
        bool IsConnected();
        void RecvPdu();
        void SendPdu();
    }
}
