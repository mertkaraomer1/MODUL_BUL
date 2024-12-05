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
            table.Columns.Add("RES�M NO");
            table.Columns.Add("MOD�L NO");
            table.Columns.Add("MOD�L2 NO");
            table.Columns.Add("�N�TE ADI");
            table.Columns.Add("ADET");


            var rsa1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(
                    dbContext.ISEMIRLERI,
                    u => u.upl_isemri,
                    i => i.is_Kod,
                    (u, i) => new { u, i }
                )
                .Join(dbContext.ISEMIRLERI_USER,
                    �s => �s.i.is_Guid,
                    iu => iu.Record_uid,
                    (�s, iu) => new { �s, iu })
                .Where(x => x.�s.u.upl_kodu.Contains(".") && !x.�s.u.upl_kodu.StartsWith("DIN") &&
                            !string.IsNullOrEmpty(x.�s.u.upl_urstokkod) && x.�s.i.is_ProjeKodu == projekod
                            && !string.IsNullOrEmpty(x.�s.i.is_BagliOlduguIsemri)
                            && x.iu.is_emri_tipi == "KK_IE")
                .Select(x => x.�s.i.is_BagliOlduguIsemri).Distinct()
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
                }).Distinct()
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
                // STOKLAR tablosundan sto_kod ile e�le�en veriyi bul
                var stokVerisi = dbContext.STOKLAR.FirstOrDefault(stok => stok.sto_kod == rsa4.upl_urstokkod);

                // Sat�r� ekle ve checkbox durumunu ayarla
                table.Rows.Add(sayac++, resim.upl_kodu, rsa3.upl_kodu, rsa4.upl_urstokkod, stokVerisi.sto_isim, resim.upl_miktar);
            }

            advancedDataGridView1.DataSource = table;

            // Checkbox s�tununu g�ncelle
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                bool isChecked = Tcontext.Modul_Bul.Any(m => m.resim_no == row.Cells["RES�M NO"].Value.ToString() && m.proje_no == projekod &&
                m.modul_no == row.Cells["MOD�L NO"].Value.ToString() && m.Sat�r_no == row.Cells["SATIR NO"].Value.ToString());
                row.Cells["checkBoxColumn1"].Value = isChecked; // "T" s�tununu g�ncelle
            }

            textBox2.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (var context = new TContext()) // DbContext s�n�f�n�z� belirtin
            {
                string projeno = textBox1.Text; // Proje numaras�n� al

                // Checkbox'�n de�erini kontrol et
                foreach (DataGridViewRow row in advancedDataGridView1.Rows)
                {
                    string resimno = row.Cells["RES�M NO"].Value?.ToString();
                    string mod�lno = row.Cells["MOD�L NO"].Value?.ToString();
                    string modulkod = row.Cells["MOD�L2 NO"].Value?.ToString();
                    int adet = Convert.ToInt32(row.Cells["ADET"].Value?.ToString());
                    string sat�rno = row.Cells["SATIR NO"].Value?.ToString();

                    // Checkbox'�n de�erini kontrol et
                    if (Convert.ToBoolean(row.Cells["checkBoxColumn1"].Value) == true) // Buray� g�ncelledik
                    {
                        // Veritaban�nda kay�t olup olmad���n� kontrol et
                        var existingEntry = context.Modul_Bul
                            .FirstOrDefault(m => m.proje_no == projeno && m.resim_no == resimno && m.modul_no == mod�lno && m.Sat�r_no == sat�rno);

                        // E�er kay�t yoksa yeni bir Modul_Bul nesnesi olu�tur
                        if (existingEntry == null)
                        {
                            var newEntry = new Modul_Bul
                            {
                                proje_no = projeno,
                                resim_no = resimno,
                                modul_no = mod�lno,
                                Modul_kod = modulkod,
                                Adet = adet,
                                Sat�r_no = sat�rno
                            };

                            // Yeni nesneyi DbSet'e ekle
                            context.Modul_Bul.Add(newEntry);
                        }
                    }
                    else
                    {
                        // Checkbox bo�sa veritaban�nda varsa sil
                        var existingEntry = context.Modul_Bul.FirstOrDefault(m => m.proje_no == projeno && m.resim_no == resimno && m.modul_no == mod�lno && m.Sat�r_no == sat�rno);

                        if (existingEntry != null)
                        {
                            context.Modul_Bul.Remove(existingEntry);
                        }
                    }
                }

                // De�i�iklikleri kaydet
                context.SaveChanges();


                try
                {
                    // De�i�iklikleri kaydet
                    context.SaveChanges();
                    MessageBox.Show("BA�ARIYLA KAYIT ED�LD�...");
                }
                catch (DbUpdateException ex)
                {
                    // Hata ayr�nt�lar�n� yazd�r
                    MessageBox.Show($"Hata: {ex.InnerException?.Message}");
                }
            }



        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RAPOR RP=new RAPOR();
            RP.Show();
        }
    }

}
