using Microsoft.EntityFrameworkCore;
using MODUL_BUL.Context;
using System.Data;

namespace MODUL_BUL
{
    public partial class Form1 : Form
    {
        private MyDbContext dbContext;
        public Form1()
        {
            dbContext = new MyDbContext();
            InitializeComponent();
        }
        DataTable table = new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            string projekod = textBox1.Text;
            string resimno = textBox2.Text;
            table.Columns.Clear();
            table.Rows.Clear();
            table.Clear();
            table.Columns.Add("SATIR NO");
            table.Columns.Add("RESÝM NO");
            table.Columns.Add("MODÜL NO");
            table.Columns.Add("ADET");

            var rsa1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(
                    dbContext.ISEMIRLERI,
                    u => u.upl_isemri,
                    i => i.is_Kod,
                    (u, i) => new { u, i }
                ).Join(dbContext.ISEMIRLERI_USER,
                    ýs => ýs.i.is_Guid,
                    iu => iu.Record_uid,
                    (ýs, iu) => new { ýs, iu })
                .Where(x => x.ýs.u.upl_kodu.Contains(".") && !x.ýs.u.upl_kodu.StartsWith("DIN") &&
                            !string.IsNullOrEmpty(x.ýs.u.upl_urstokkod) && x.ýs.i.is_ProjeKodu == projekod
                            && !string.IsNullOrEmpty(x.ýs.i.is_BagliOlduguIsemri)
                            && x.iu.is_emri_tipi=="KK_IE") // Boþ veya null kontrolü
                .Select(x => x.ýs.i.is_BagliOlduguIsemri).Distinct()
                .ToList(); // Sonucu listeye dönüþtür



            var rsa2 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(
                    dbContext.ISEMIRLERI,
                    u => u.upl_isemri,
                    i => i.is_Kod,
                    (u, i) => new { u, i }
                )
                .Where(x => x.u.upl_kodu.Contains(".") &&
                            !x.u.upl_kodu.StartsWith("DIN") &&
                            rsa1.Contains(x.u.upl_isemri) &&
                            !string.IsNullOrEmpty(x.u.upl_urstokkod) &&
                            x.i.is_ProjeKodu == projekod &&
                            x.u.upl_kodu == resimno)
                .Select(x => new
                {
                    x.i.is_BagliOlduguIsemri,
                    x.u.upl_kodu,
                    x.u.upl_miktar,

                })
                .ToList();



            int sayac = 1;
            foreach (var resim in rsa2)
            {
                        var rsa3 = dbContext.URETIM_MALZEME_PLANLAMA
            .Where(x => x.upl_kodu.Contains(".") &&
                        resim.is_BagliOlduguIsemri == x.upl_isemri &&
                        !string.IsNullOrEmpty(x.upl_urstokkod)) // Boþ veya null kontrolü
            .Select(x => new
            {
                x.upl_kodu

            }).FirstOrDefault
            (); // Sonucu listeye dönüþtür
                        table.Rows.Add(sayac++, resim.upl_kodu, rsa3.upl_kodu, resim.upl_miktar);
                    }
                    advancedDataGridView1.DataSource = table;
                    textBox2.Clear();
        }
    }
    
}
