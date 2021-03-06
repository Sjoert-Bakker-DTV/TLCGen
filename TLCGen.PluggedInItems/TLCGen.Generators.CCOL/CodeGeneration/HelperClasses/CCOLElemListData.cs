﻿using System.Collections.Generic;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLElemListData
    {
        public List<CCOLElement> Elements { get; set; }

        public string CCOLCode { get; set; }
        public string CCOLSetting { get; set; }
        public string CCOLTType { get; set; }

        public int CCOLCodeWidth { get { return CCOLCode == null ? 0 : CCOLCode.Length; } }
        public int CCOLSettingWidth { get { return CCOLSetting == null ? 0 : CCOLSetting.Length; } }
        public int CCOLTTypeWidth { get { return CCOLTType == null ? 0 : CCOLTType.Length; } }

        public int TTypeMaxWidth { get; set; }
        public int DefineMaxWidth { get; set; }
        public int NameMaxWidth { get; set; }
        public int SettingMaxWidth { get; set; }
        public int CommentsMaxWidth { get; set; }

        public void SetMax()
        {
            foreach (CCOLElement elem in this.Elements)
            {
                if (elem.Define?.Length > this.DefineMaxWidth) this.DefineMaxWidth = elem.Define.Length;
                if (elem.Naam?.Length > this.NameMaxWidth) this.NameMaxWidth = elem.Naam.Length;
                if (elem.Instelling?.ToString().Length > this.SettingMaxWidth) this.SettingMaxWidth = elem.Instelling.ToString().Length;
                if (elem.Commentaar?.ToString().Length > this.CommentsMaxWidth) this.CommentsMaxWidth = elem.Commentaar.ToString().Length;
                if (elem.TType.ToString().Length > this.TTypeMaxWidth) this.TTypeMaxWidth = elem.TType.ToString().Length;
            }
        }

        public CCOLElemListData()
        {
            DefineMaxWidth = 0;
            NameMaxWidth = 0;
            SettingMaxWidth = 0;

            Elements = new List<CCOLElement>();
        }
    }
}
