﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODUL_BUL.Tables
{
    public class URETIM_MALZEME_PLANLAMA
    {
        public Guid upl_Guid { get; set; }
        public int upl_DBCno { get; set; }
        public string upl_isemri { get; set; }
        public string upl_kodu { get; set; }
        public string upl_urstokkod { get; set; }
        public int upl_satirno { get; set; }
        public double upl_uret_miktar { get; set; }
        public string upl_aciklama { get; set; }
        public double upl_sarfmiktari { get; set; }
        public double upl_miktar { get; set; }
    }
}