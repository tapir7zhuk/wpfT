using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAppT.Data;

namespace WpfAppT.Views.Pages
{
    public partial class FrontPage : Page
    {
        private readonly AppDbContext _db;
        private const string PhotoSettingPath = "shop_photo.txt";

        public FrontPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadData();
            LoadShopPhoto();
        }

        private void LoadData()
        {
            TodayDate.Text = DateTime.Now.ToString("dd MMM yyyy");
            SpecialistsCount.Text = _db.Specialists.Count().ToString();
            CarsCount.Text = _db.Records.Count(r => !r.IsCompleted).ToString();
        }

        private void LoadShopPhoto()
        {
            if (File.Exists(PhotoSettingPath))
            {
                var path = File.ReadAllText(PhotoSettingPath).Trim();
                if (File.Exists(path))
                {
                    ShopPhoto.Source = new BitmapImage(new Uri(path));
                    PhotoPlaceholder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select shop photo"
            };

            if (dialog.ShowDialog() == true)
            {
                ShopPhoto.Source = new BitmapImage(new Uri(dialog.FileName));
                PhotoPlaceholder.Visibility = Visibility.Collapsed;
                File.WriteAllText(PhotoSettingPath, dialog.FileName);
            }
        }
    }
}