using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BamziLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Adın yok mu senin?");
                return;
            }

            btnPlay.IsEnabled = false;
            btnPlay.Content = "YÜKLENİYOR...";

            try
            {
                // ================== AYARLAR ==================
                // 1. Forge Klasörünün TAM ADI (Bamzi_Data/versions içindekiyle AYNI olmalı)
                // Klasör adın neyse buraya onu yaz!
                var targetVersion = "1.20.1-forge-47.4.13";

                // 2. Sunucu Bilgileri
                var ip = "84.51.52.126";
                var port = 25565;
                // =============================================

                // PORTABLE AYARI: EXE'nin yanındaki "Bamzi_Data" klasörü
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bamzi_Data");
                var path = new MinecraftPath(basePath);

                var launcher = new MinecraftLauncher(path);

                txtStatus.Text = "Sürüm kontrol ediliyor...";

                // Sürümü buluyoruz (IVersion objesi gelir)
                var versionInfo = await launcher.GetVersionAsync(targetVersion);

                // BAŞLATMA AYARLARI
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = 4096,
                    Session = MSession.CreateOfflineSession(txtUsername.Text),
                    GameLauncherName = "BamziMC",
                    ServerIp = ip,
                    ServerPort = port
                };

                txtStatus.Text = "BamziMC Başlatılıyor...";

                // --- İŞTE DÜZELTTİĞİMİZ YER ---
                // versionInfo (Dosya) yerine versionInfo.Id (İsim) veriyoruz.
                // Bu sayede "string'e dönüştürülemiyor" hatası YOK OLACAK.
                var process = await launcher.CreateProcessAsync(versionInfo.Id, launchOption);

                // B Planı: Sunucuya Zorla Giriş Komutu
                process.StartInfo.Arguments += $" --server {ip} --port {port}";

                this.Hide();
                process.Start();
                process.WaitForExit();
                this.Show();
                txtStatus.Text = "Oyun bitti.";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("HATA:\n" + ex.Message + "\n\nKONTROL ET:\n1. 'Bamzi_Data' klasörü EXE'nin yanında mı?\n2. Klasördeki sürüm adıyla koddaki 'targetVersion' aynı mı?");
            }
            finally
            {
                btnPlay.IsEnabled = true;
                btnPlay.Content = "OYNA";
                txtStatus.Text = "Hazır";
            }
        }
    }
}