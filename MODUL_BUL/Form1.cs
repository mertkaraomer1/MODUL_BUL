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
            table.Columns.Add("ADET");

            var rsa1 = dbContext.URETIM_MALZEME_PLANLAMA
                .Join(
                    dbContext.ISEMIRLERI,
                    u => u.upl_isemri,
                    i => i.is_Kod,
                    (u, i) => new { u, i }
                ).Join(dbContext.ISEMIRLERI_USER,
                    �s => �s.i.is_Guid,
                    iu => iu.Record_uid,
                    (�s, iu) => new { �s, iu })
                .Where(x => x.�s.u.upl_kodu.Contains(".") && !x.�s.u.upl_kodu.StartsWith("DIN") &&
                            !string.IsNullOrEmpty(x.�s.u.upl_urstokkod) && x.�s.i.is_ProjeKodu == projekod
                            && !string.IsNullOrEmpty(x.�s.i.is_BagliOlduguIsemri)
                            && x.iu.is_emri_tipi == "KK_IE") // Bo� veya null kontrol�
                .Select(x => x.�s.i.is_BagliOlduguIsemri).Distinct()
                .ToList(); // Sonucu listeye d�n��t�r



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
                            !string.IsNullOrEmpty(x.upl_urstokkod)) // Bo� veya null kontrol�
                .Select(x => new
                {
                    x.upl_kodu

                }).FirstOrDefault
                (); // Sonucu listeye d�n��t�r
                table.Rows.Add(sayac++, resim.upl_kodu, rsa3.upl_kodu, resim.upl_miktar);
            }
            advancedDataGridView1.DataSource = table;
            textBox2.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (var context = new TContext()) // DbContext s�n�f�n�z� belirtin
            {
                foreach (DataGridViewRow row in advancedDataGridView1.Rows)
                {
                    string projeno = textBox1.Text;
                    if (Convert.ToBoolean(row.Cells["checkBoxColumn1"].Value))
                    {
                        // Sat�rdan gerekli verileri al
                        string resimno = row.Cells["RES�M NO"].Value.ToString();
                        string mod�lno = row.Cells["MOD�L NO"].Value.ToString();
                        int adet = Convert.ToInt32(row.Cells["ADET"].Value.ToString());


                        // Yeni bir Modul_Bul nesnesi olu�tur
                        var newEntry = new Modul_Bul
                        {
                            proje_no = projeno,
                            resim_no = resimno,
                            modul_no = mod�lno,
                            Adet = adet
                        };

                        // Yeni nesneyi DbSet'e ekle
                        context.Modul_Bul.Add(newEntry);
                    }
                }

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
    }

}
