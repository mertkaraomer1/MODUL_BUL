using MODUL_BUL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MODUL_BUL.Tables
{
    public partial class RAPOR : Form
    {
        private MyDbContext dbContext;
        private TContext Tcontext;
        public RAPOR()
        {
            dbContext = new MyDbContext();
            Tcontext = new TContext();
            InitializeComponent();

            DataGridViewCheckBoxColumn checkBoxColumn1 = new DataGridViewCheckBoxColumn();
            checkBoxColumn1.HeaderText = "T";
            checkBoxColumn1.Name = "checkBoxColumn1";
            advancedDataGridView1.Columns.Insert(0, checkBoxColumn1);
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string projeKodu = textBox1.Text;

            // Filtrelenmiş projeleri al
            var filtrelenmisProjeler = dbContext.ISEMIRLERI
                .Where(isEmir => isEmir.is_ProjeKodu == projeKodu)
                .Select(isEmir => isEmir.is_Kod)
                .ToList();

            // Uretim malzeme planlamadan ve is emirlerinden verileri birleştir
            var uplUrStokKodAnd1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(dbContext.ISEMIRLERI,
                    uretim => uretim.upl_isemri,
                    isEmir => isEmir.is_BagliOlduguIsemri,
                    (uretim, isEmir) => new { Uretim = uretim, IsEmir = isEmir })
                .Where(joined => filtrelenmisProjeler.Contains(joined.Uretim.upl_isemri) &&
                                 joined.Uretim.upl_urstokkod.StartsWith("01.") &&
                                 joined.Uretim.upl_urstokkod.EndsWith(".014")) // .EndsWith kontrolü
                .GroupBy(joined => new { joined.Uretim.upl_urstokkod, joined.IsEmir.is_EmriDurumu })
                .Select(group => new
                {
                    Ustokkod = group.Key.upl_urstokkod,
                    // Burada istediğiniz formatı belirleyebilirsiniz
                    FormattedUstokkod = $"{group.Key.upl_urstokkod}"
                })
                .Distinct()
                .ToList();

            // ComboBox'a veri kaynağını ata
            comboBox1.DataSource = uplUrStokKodAnd1;
            comboBox1.DisplayMember = "FormattedUstokkod"; // Gösterilecek alan
            comboBox1.ValueMember = "Ustokkod"; // Değer alanı
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string projeKodu = textBox1.Text;
            string unitKod = comboBox1.Text;

            // Uretim malzeme planlamadan ve is emirlerinden verileri birleştir
            var uplUrStokKodAnd1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(dbContext.ISEMIRLERI,
                    uretim => uretim.upl_isemri,
                    isEmir => isEmir.is_BagliOlduguIsemri,
                    (uretim, isEmir) => new { Uretim = uretim, IsEmir = isEmir })
                .Where(joined => joined.Uretim.upl_urstokkod == unitKod && joined.IsEmir.is_ProjeKodu == projeKodu)
                .GroupBy(joined => new { joined.Uretim.upl_kodu })
                .Select(group => group.Key.upl_kodu + ".01.014")
                .ToList();

            // ComboBox'a varsayılan öğe ekle
            var modülListesi = new List<string> { "Bir modül seçiniz" };
            modülListesi.AddRange(uplUrStokKodAnd1);

            // ComboBox'a veri kaynağını ata
            comboBox2.DataSource = modülListesi;

        }
        DataTable table= new DataTable();
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string projeKodu = textBox1.Text;
            string modülkod = comboBox2.Text;
            string ünitekod = comboBox1.Text;

            table.Columns.Clear();
            table.Rows.Clear();
            table.Clear();
            table.Columns.Add("SATIR NO");
            table.Columns.Add("PARÇA NO");

            var sonuc = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(dbContext.ISEMIRLERI,
                    ump => ump.upl_isemri,
                    isem => isem.is_Kod,
                    (ump, isem) => new { Ump = ump, Isem = isem })
                .Where(joined => joined.Isem.is_ProjeKodu == projeKodu &&
                                 joined.Ump.upl_urstokkod == modülkod)
                .Select(joined => new
                {
                    // İstediğiniz alanları buraya ekleyebilirsiniz
                    joined.Ump.upl_kodu
                })
                .ToList();
            int sayac = 1;
            foreach (var item in sonuc)
            { 
                table.Rows.Add(sayac++,item.upl_kodu);
            }
            advancedDataGridView1.DataSource = table;
            // Checkbox sütununu güncelle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                bool isChecked = Tcontext.Modul_Bul.Any(m => m.resim_no == row.Cells["PARÇA NO"].Value.ToString() && m.proje_no == projeKodu && (m.modul_no == modülkod||m.modul_no==modülkod )&& m.Modul_kod == ünitekod);
                row.Cells["checkBoxColumn1"].Value = isChecked; // "T" sütununu güncelle
            }

        }
    }
}
