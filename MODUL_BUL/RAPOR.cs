using MODUL_BUL.Context;
using System.Data;

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
                                 (joined.Uretim.upl_urstokkod.StartsWith("01.") || joined.Uretim.upl_urstokkod.StartsWith("99.")) &&
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
                .Where(joined => joined.Uretim.upl_urstokkod == unitKod && joined.IsEmir.is_ProjeKodu == projeKodu
                && !joined.Uretim.upl_kodu.StartsWith("DIN"))
                .GroupBy(joined => new { joined.Uretim.upl_kodu })
                .Select(group => group.Key.upl_kodu + ".01.014")
                .ToList();

            // ComboBox'a varsayılan öğe ekle
            var modülListesi = new List<string> { "Bir modül seçiniz" };
            modülListesi.AddRange(uplUrStokKodAnd1);

            // ComboBox'a veri kaynağını ata
            comboBox2.DataSource = modülListesi;

        }
        DataTable table = new DataTable();
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
            table.Columns.Add("PARÇA ADI");
            table.Columns.Add("ADET");

            var sonuc = dbContext.URETIM_MALZEME_PLANLAMA
             .Join(dbContext.ISEMIRLERI,
                 ump => ump.upl_isemri,
                 isem => isem.is_Kod,
                 (ump, isem) => new { Ump = ump, Isem = isem })
             .Where(joined => joined.Isem.is_ProjeKodu == projeKodu &&
                              joined.Ump.upl_urstokkod == modülkod)
             .Join(dbContext.STOKLAR,
                 joined => joined.Ump.upl_kodu, // STOKLAR ile birleştirme koşulu
                 stok => stok.sto_kod, // STOKLAR'daki karşılık gelen alan
                 (joined, stok) => new
                 {
                     // İstediğiniz alanları buraya ekleyebilirsiniz
                     joined.Ump.upl_kodu,
                     stok.sto_isim,
                     joined.Ump.upl_miktar,


                 })
             .ToList();

            int sayac = 1;
            foreach (var item in sonuc)
            {
                table.Rows.Add(sayac++, item.upl_kodu, item.sto_isim, item.upl_miktar);
            }
            advancedDataGridView1.DataSource = table;
            // Checkbox sütununu güncelle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                bool isChecked = Tcontext.Modul_Bul.Any(m => m.resim_no == row.Cells["PARÇA NO"].Value.ToString() && m.proje_no == projeKodu && (m.modul_no.Substring(0, 13) == modülkod.Substring(0, 13) || m.modul_no.Substring(0, 13) == modülkod.Substring(0, 13)) && m.Modul_kod.Substring(0, 13) == ünitekod.Substring(0, 13));
                row.Cells["checkBoxColumn1"].Value = isChecked; // "T" sütununu güncelle
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // DataGridView'deki verileri bir DataTable'a kopyalayın
            DataTable dt = new DataTable();

            foreach (DataGridViewColumn column in advancedDataGridView1.Columns)
            {
                // Eğer ValueType null ise, varsayılan bir veri türü kullanabilirsiniz.
                Type columnType = column.ValueType ?? typeof(string);
                dt.Columns.Add(column.HeaderText, columnType);
            }

            // Satırları ekle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                if (!row.IsNewRow) // Yeni satırı atla
                {
                    DataRow dataRow = dt.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dataRow[cell.ColumnIndex] = cell.Value;
                    }
                    dt.Rows.Add(dataRow);
                }
            }

            // Excel uygulamasını başlatın
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;

            // Yeni bir Excel çalışma kitabı oluşturun
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

            // textBox1, comboBox1 ve comboBox2'deki verileri yaz
            worksheet.Cells[1, 1] = textBox1.Text; // 1. satır, 1. sütun
            worksheet.Cells[2, 1] = comboBox1.Text; // 2. satır, 1. sütun
            worksheet.Cells[3, 1] = comboBox2.Text; // 3. satır, 1. sütun

            // DataTable'ı Excel çalışma sayfasına aktarın (tablo başlıklarını da dahil etmek için)
            int rowIndex = 4; // Veriler 4. satırdan başlayacak

            // Başlıkları yaz
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                worksheet.Cells[rowIndex, j + 1] = dt.Columns[j].ColumnName;
                worksheet.Cells[rowIndex, j + 1].Font.Bold = true;
            }

            // Verileri yaz
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                rowIndex++;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[rowIndex, j + 1] = dt.Rows[i][j].ToString();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string projeKodu = textBox1.Text;
            string ünitekod = comboBox1.Text;

            table.Columns.Clear();
            table.Rows.Clear();
            table.Clear();
            table.Columns.Add("SATIR NO");
            table.Columns.Add("PARÇA NO");
            table.Columns.Add("PARÇA ADI");
            table.Columns.Add("ADET");

            var sonuc = dbContext.URETIM_MALZEME_PLANLAMA
             .Join(dbContext.ISEMIRLERI,
                 ump => ump.upl_isemri,
                 isem => isem.is_Kod,
                 (ump, isem) => new { Ump = ump, Isem = isem })
             .Where(joined => joined.Isem.is_ProjeKodu == projeKodu &&
                              joined.Ump.upl_urstokkod == ünitekod&&
                              !joined.Ump.upl_kodu.StartsWith("01."))
             .Join(dbContext.STOKLAR,
                 joined => joined.Ump.upl_kodu, // STOKLAR ile birleştirme koşulu
                 stok => stok.sto_kod, // STOKLAR'daki karşılık gelen alan
                 (joined, stok) => new
                 {
                     joined.Ump.upl_kodu,
                     stok.sto_isim,
                     joined.Ump.upl_miktar,
                 })
             .ToList();

            int sayac = 1;
            foreach (var item in sonuc)
            {
                table.Rows.Add(sayac++, item.upl_kodu, item.sto_isim, item.upl_miktar);
            }
            advancedDataGridView1.DataSource = table;
            // Checkbox sütununu güncelle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                bool isChecked = Tcontext.Modul_Bul.Any(m => m.resim_no == row.Cells["PARÇA NO"].Value.ToString() && m.proje_no == projeKodu && (m.modul_no.Substring(0, 13) == ünitekod.Substring(0, 13) || m.modul_no.Substring(0, 13) == ünitekod.Substring(0, 13)) && m.Modul_kod.Substring(0, 13) == ünitekod.Substring(0, 13));
                row.Cells["checkBoxColumn1"].Value = isChecked; // "T" sütununu güncelle
            }
        }
    }
}
