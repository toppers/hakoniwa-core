using System;
using System.Collections.Generic;
using System.Text;

namespace HakoniwaService
{
    class Asset
    {
        static private int asset_id = 0;
        private int id;
        private string name;

        public Asset(string asset_name)
        {
            this.id = Asset.asset_id++;
            this.name = asset_name;
        }

        public int GetId()
        {
            return id;
        }
        public string GetName()
        {
            return name;
        }

    }
}
