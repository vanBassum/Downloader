using System;
using System.IO;
using MasterLibrary.PropertySensitive;

namespace Downloader
{
    public class DLPar : PropertySensitiveExternal
    {
        public Uri Source { get { return GetPar<Uri>(); } set { SetPar(value); } }
        public string Dest { get { return GetPar<string>(); } set { SetPar(value); } }

        public override string ToString()
        {
            return Path.GetFileName(Dest);
        }
    }

}
