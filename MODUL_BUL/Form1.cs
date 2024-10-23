using Microsoft.EntityFrameworkCore;
using MODUL_BUL.Context;
using MODUL_BUL.Tables;
using System.Data;
using Zuby.ADGV;

namespace MODUL_BUL
{
    public partial class Form1 : Form
    {
        private MyDbContext dbContext;
        private TContext Tcontext;
        public Form1()
        {
            dbContext = new MyDbContext();
            Tcontext = new TContext();
            InitializeComponent();

            DataGridViewCheckBoxColumn checkBoxColumn1 = new DataGridViewCheckBoxColumn();
            checkBoxColumn1.HeaderText = "T";
            checkBoxColumn1.Name = "checkBoxColumn1";
            advancedDataGridView1.Columns.Insert(0, checkBoxColumn1);
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
            table.Columns.Add("MODÜL2 NO");
            table.Columns.Add("ADET");


            var rsa1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(
                    dbContext.ISEMIRLERI,
                    u => u.upl_isemri,
                    i => i.is_Kod,
                    (u, i) => new { u, i }
                )
                .Join(dbContext.ISEMIRLERI_USER,
                    ýs => ýs.i.is_Guid,
                    iu => iu.Record_uid,
                    (ýs, iu) => new { ýs, iu })
                .Where(x => x.ýs.u.upl_kodu.Contains(".") && !x.ýs.u.upl_kodu.StartsWith("DIN") &&
                            !string.IsNullOrEmpty(x.ýs.u.upl_urstokkod) && x.ýs.i.is_ProjeKodu == projekod
                            && !string.IsNullOrEmpty(x.ýs.i.is_BagliOlduguIsemri)
                            && x.iu.is_emri_tipi == "KK_IE")
                .Select(x => x.ýs.i.is_BagliOlduguIsemri).Distinct()
                .ToList();

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
                            !string.IsNullOrEmpty(x.upl_urstokkod))
                .Select(x => new
                {
                    x.upl_kodu,
                    x.upl_urstokkod,
                }).FirstOrDefault();

                var rsa4 = dbContext.URETIM_MALZEME_PLANLAMA
                    .Where(x => x.upl_kodu.Contains(rsa3.upl_urstokkod.Substring(0, 13)) && x.upl_kodu.Contains(".") &&
                                !string.IsNullOrEmpty(x.upl_urstokkod))
                    .Select(x => new
                    {
                        x.upl_urstokkod
                    }).FirstOrDefault();

                // Modul_Bul tablosunu kontrol et
                bool checkboxValue = Tcontext.Modul_Bul.Any(m => m.resim_no == resim.upl_kodu&&m.proje_no==projekod&&m.modul_no==rsa3.upl_kodu);

                // Satýrý ekle ve checkbox durumunu ayarla
                table.Rows.Add(sayac++, resim.upl_kodu, rsa3.upl_kodu, rsa4.upl_urstokkod, resim.upl_miktar);
            }

            advancedDataGridView1.DataSource = table;

            // Checkbox sütununu güncelle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                bool isChecked = Tcontext.Modul_Bul.Any(m => m.resim_no == row.Cells["RESÝM NO"].Value.ToString());
                row.Cells["checkBoxColumn1"].Value = isChecked; // "T" sütununu güncelle
            }

            textBox2.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (var context = new TContext()) // DbContext sýnýfýnýzý belirtin
            {
                string projeno = textBox1.Text; // Proje numarasýný al

                foreach (DataGridViewRow row in advancedDataGridView1.Rows)
                {
                    // Checkbox'ýn deðerini kontrol et
                    if (Convert.ToBoolean(row.Cells["checkBoxColumn1"].Value) == true)
                    {
                        // Satýrdan gerekli verileri al
                        string resimno = row.Cells["RESÝM NO"].Value?.ToString();
                        string modülno = row.Cells["MODÜL NO"].Value?.ToString();
                        string modulkod = row.Cells["MODÜL2 NO"].Value?.ToString();
                        int adet = Convert.ToInt32(row.Cells["ADET"].Value?.ToString());

                        // Yeni bir Modul_Bul nesnesi oluþtur
                        var newEntry = new Modul_Bul
                        {
                            proje_no = projeno,
                            resim_no = resimno,
                            modul_no = modülno,
                            Modul_kod = modulkod,
                            Adet = adet
                        };

                        // Yeni nesneyi DbSet'e ekle
                        context.Modul_Bul.Add(newEntry);
                    }
                }

                try
                {
                    // Deðiþiklikleri kaydet
                    context.SaveChanges();
                    MessageBox.Show("BAÞARIYLA KAYIT EDÝLDÝ...");
                }
                catch (DbUpdateException ex)
                {
                    // Hata ayrýntýlarýný yazdýr
                    MessageBox.Show($"Hata: {ex.InnerException?.Message}");
                }
            }



        }
    }

}
