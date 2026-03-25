using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAppT.Data;
using WpfAppT.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace WpfAppT.Views.Pages
{
    public class RankItem
    {
        public string Rank { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public partial class RecordsPage : Page
    {
        private readonly AppDbContext _db;
        private Record _selected;

        public RecordsPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadData();
        }

        private void LoadData()
        {
            RecordsGrid.ItemsSource = _db.Records
                .Include(r => r.Specialist)
                .Include(r => r.Customer)
                .Include(r => r.Car).ThenInclude(c => c.Brand)
                .Include(r => r.Photo)
                .Where(r => !r.IsCompleted)
                .OrderBy(r => r.DateAdded)
                .ToList();

            var lastMonth = DateTime.Now.AddMonths(-1);
            var top5 = _db.Records
                .Include(r => r.Specialist)
                .Where(r => r.IsCompleted && r.DateCompleted >= lastMonth)
                .AsEnumerable()
                .GroupBy(r => r.Specialist)
                .Select(g => new { Specialist = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            TopSpecialistsList.ItemsSource = top5.Select((x, i) => new RankItem
            {
                Rank = $"{i + 1}.",
                Name = $"{x.Specialist.FirstName} {x.Specialist.LastName}",
                Count = x.Count
            }).ToList();

            var lastYear = DateTime.Now.AddYears(-1);
            var topBrands = _db.Records
                .Include(r => r.Car).ThenInclude(c => c.Brand)
                .Where(r => r.DateAdded >= lastYear)
                .AsEnumerable()
                .GroupBy(r => r.Car.Brand.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            TopBrandsList.ItemsSource = topBrands.Select((x, i) => new RankItem
            {
                Rank = $"{i + 1}.",
                Name = x.Name,
                Count = x.Count
            }).ToList();

            ShowDetail();
        }

        private void RecordsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = RecordsGrid.SelectedItem as Record;
            BtnDelete.IsEnabled = _selected != null;
            BtnEdit.IsEnabled = _selected != null;
            BtnComplete.IsEnabled = _selected != null && !_selected.IsCompleted;

            if (_selected == null) { ClearDetail(); return; }

            ShowDetail();
            DetailPlaceholder.Visibility = Visibility.Collapsed;
            DetailCar.Visibility = Visibility.Visible;
            DetailPlate.Visibility = Visibility.Visible;
            CarPhotoContainer.Visibility = Visibility.Visible;
            DetailSep.Visibility = Visibility.Visible;
            DetailInfo.Visibility = Visibility.Visible;

            DetailCar.Text = $"{_selected.Car?.Brand?.Name} {_selected.Car?.Model}";
            DetailPlate.Text = _selected.LicensePlate;
            DetailSpecialist.Text = $"{_selected.Specialist?.FirstName} {_selected.Specialist?.LastName}";
            DetailCustomer.Text = $"{_selected.Customer?.FirstName} {_selected.Customer?.LastName}";
            DetailDate.Text = _selected.DateAdded.ToString("dd MMM yyyy");
            DetailReason.Text = _selected.Reason;
            DetailDescription.Text = _selected.MasterDescription;

            LoadCarPhoto();
        }

        private void BtnBrowsePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select car photo"
            };
            if (dialog.ShowDialog() == true)
                TxtPhotoUrl.Text = dialog.FileName;
        }

        private void LoadCarPhoto()
        {
            if (_selected.Photo != null && !string.IsNullOrEmpty(_selected.Photo.FilePath))
            {
                try
                {
                    var bitmap = new BitmapImage(
                        new Uri(_selected.Photo.FilePath, UriKind.Absolute));
                    CarPhoto.Source = bitmap;
                }
                catch { CarPhoto.Source = null; }
            }
            else
            {
                CarPhoto.Source = null;
            }
        }

        private async Task LoadPhotoAsync(string url)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var bytes = await client.GetByteArrayAsync(url);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new System.IO.MemoryStream(bytes);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                CarPhoto.Source = bitmap;
            }
            catch
            {
                CarPhoto.Source = null;
            }
        }

        private void RecordsGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selected == null) return;
            ShowForm("Edit Record", _selected);
        }

        private void ShowDetail()
        {
            DetailPanel.Visibility = Visibility.Visible;
            FormPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearDetail()
        {
            DetailPlaceholder.Visibility = Visibility.Visible;
            DetailCar.Visibility = Visibility.Collapsed;
            DetailPlate.Visibility = Visibility.Collapsed;
            CarPhotoContainer.Visibility = Visibility.Collapsed;
            DetailSep.Visibility = Visibility.Collapsed;
            DetailInfo.Visibility = Visibility.Collapsed;
        }

        private void ShowForm(string title, Record r = null)
        {
            FormTitle.Text = title;

            var specialists = _db.Specialists.ToList();
            CmbSpecialist.ItemsSource = specialists;
            CmbSpecialist.DisplayMemberPath = "LastName";

            var customers = _db.Customers
                .Include(c => c.Cars).ThenInclude(car => car.Brand)
                .ToList();
            CmbCustomer.ItemsSource = customers;
            CmbCustomer.DisplayMemberPath = "LastName";

            if (r != null)
            {
                CmbSpecialist.SelectedItem = specialists
                    .FirstOrDefault(s => s.Id == r.SpecialistId);
                var selectedCustomer = customers
                    .FirstOrDefault(c => c.Id == r.CustomerId);
                CmbCustomer.SelectedItem = selectedCustomer;

                if (selectedCustomer != null)
                {
                    var cars = _db.Cars
                        .Include(c => c.Brand)
                        .Where(c => c.CustomerId == selectedCustomer.Id)
                        .ToList();
                    CmbCar.ItemsSource = cars;
                    CmbCar.DisplayMemberPath = "LicensePlate";
                    CmbCar.SelectedItem = cars
                        .FirstOrDefault(c => c.LicensePlate == r.LicensePlate);
                }

                TxtReason.Text = r.Reason;
                TxtDescription.Text = r.MasterDescription;
                TxtPhotoUrl.Text = r.Photo?.FilePath ?? "";
            }
            else
            {
                CmbSpecialist.SelectedIndex = -1;
                CmbCustomer.SelectedIndex = -1;
                CmbCar.ItemsSource = null;
                TxtReason.Text = "";
                TxtDescription.Text = "";
                TxtPhotoUrl.Text = "";
            }

            DetailPanel.Visibility = Visibility.Collapsed;
            FormPanel.Visibility = Visibility.Visible;
        }

        private void CmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCustomer.SelectedItem is Customer customer)
            {
                var cars = _db.Cars
                    .Include(c => c.Brand)
                    .Where(c => c.CustomerId == customer.Id)
                    .ToList();
                CmbCar.ItemsSource = cars;
                CmbCar.DisplayMemberPath = "LicensePlate";
                CmbCar.SelectedIndex = -1;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
            => ShowForm("Add Record");

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            ShowForm("Edit Record", _selected);
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            _selected.IsCompleted = true;
            _selected.DateCompleted = DateTime.Now;
            _db.SaveChanges();
            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var result = MessageBox.Show("Delete this record?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _db.Records.Remove(_selected);
                _db.SaveChanges();
                LoadData();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbSpecialist.SelectedItem is not Specialist specialist ||
                CmbCar.SelectedItem is not Car car)
            {
                MessageBox.Show("Please select specialist and car.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtReason.Text))
            {
                MessageBox.Show("Reason is required.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customer = CmbCustomer.SelectedItem as Customer;

            Photo photo = null;
            if (!string.IsNullOrEmpty(TxtPhotoUrl.Text))
            {
                photo = new Photo { FilePath = TxtPhotoUrl.Text.Trim() };
                _db.Photos.Add(photo);
                _db.SaveChanges();
            }

            if (_selected != null && FormTitle.Text == "Edit Record")
            {
                _selected.SpecialistId = specialist.Id;
                _selected.LicensePlate = car.LicensePlate;
                _selected.CustomerId = customer!.Id;
                _selected.Reason = TxtReason.Text.Trim();
                _selected.MasterDescription = TxtDescription.Text.Trim();
                if (photo != null) _selected.PhotoId = photo.Id;
            }
            else
            {
                _db.Records.Add(new Record
                {
                    SpecialistId = specialist.Id,
                    LicensePlate = car.LicensePlate,
                    CustomerId = customer!.Id,
                    Reason = TxtReason.Text.Trim(),
                    MasterDescription = TxtDescription.Text.Trim(),
                    DateAdded = DateTime.Now,
                    IsCompleted = false,
                    PhotoId = photo?.Id
                });
            }

            _db.SaveChanges();
            LoadData();
            ShowDetail();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => ShowDetail();
    }
}