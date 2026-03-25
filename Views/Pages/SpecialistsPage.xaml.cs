using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAppT.Data;
using WpfAppT.Models;

namespace WpfAppT.Views.Pages
{
    public partial class SpecialistsPage : Page
    {
        private readonly AppDbContext _db;
        private Specialist _selected;
        private bool _isEdit;

        public SpecialistsPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadData();
        }

        private void LoadData()
        {
            SpecialistsGrid.ItemsSource = _db.Specialists
                .Include(s => s.Photo)
                .ToList();
            ShowDetail();
        }

        private void SpecialistsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = SpecialistsGrid.SelectedItem as Specialist;
            BtnEdit.IsEnabled = _selected != null;
            BtnDelete.IsEnabled = _selected != null;

            if (_selected == null) { ClearDetail(); return; }

            ShowDetail();
            DetailPlaceholder.Visibility = Visibility.Collapsed;
            DetailName.Text = $"{_selected.FirstName} {_selected.LastName}";
            DetailSpecialization.Text = _selected.Specialization;
            DetailPhone.Text = _selected.PhoneNumber;
            DetailRecords.Text = _db.Records
                .Count(r => r.SpecialistId == _selected.Id && !r.IsCompleted)
                .ToString();

            if (_selected.Photo != null)
            {
                try
                {
                    SpecialistPhoto.Source = new BitmapImage(
                        new Uri(_selected.Photo.FilePath, UriKind.RelativeOrAbsolute));
                }
                catch { SpecialistPhoto.Source = null; }
            }
            else SpecialistPhoto.Source = null;
        }

        private void SpecialistsGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selected == null) return;
            _isEdit = true;
            ShowForm("Edit Specialist", _selected);
        }

        private void ShowDetail()
        {
            DetailPanel.Visibility = Visibility.Visible;
            FormPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowForm(string title, Specialist s = null)
        {
            FormTitle.Text = title;
            TxtFirstName.Text = s?.FirstName ?? "";
            TxtLastName.Text = s?.LastName ?? "";
            TxtPhone.Text = s?.PhoneNumber ?? "";
            TxtSpecialization.Text = s?.Specialization ?? "";
            TxtPhotoPath.Text = s?.Photo?.FilePath ?? "";
            DetailPanel.Visibility = Visibility.Collapsed;
            FormPanel.Visibility = Visibility.Visible;
        }

        private void ClearDetail()
        {
            DetailPlaceholder.Visibility = Visibility.Visible;
            DetailName.Text = "";
            DetailSpecialization.Text = "";
            DetailPhone.Text = "";
            DetailRecords.Text = "";
            SpecialistPhoto.Source = null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _isEdit = false;
            _selected = null;
            ShowForm("Add Specialist");
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            _isEdit = true;
            ShowForm("Edit Specialist", _selected);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var result = MessageBox.Show(
                $"Delete {_selected.FirstName} {_selected.LastName}?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _db.Specialists.Remove(_selected);
                _db.SaveChanges();
                LoadData();
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select photo"
            };
            if (dialog.ShowDialog() == true)
                TxtPhotoPath.Text = dialog.FileName;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                MessageBox.Show("First name and last name are required.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(TxtPhone.Text) &&
                !Regex.IsMatch(TxtPhone.Text, @"^\+380\d{9}$"))
            {
                MessageBox.Show("Phone format: +380XXXXXXXXX",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit && _selected != null)
            {
                _selected.FirstName = TxtFirstName.Text.Trim();
                _selected.LastName = TxtLastName.Text.Trim();
                _selected.PhoneNumber = TxtPhone.Text.Trim();
                _selected.Specialization = TxtSpecialization.Text.Trim();

                if (!string.IsNullOrEmpty(TxtPhotoPath.Text))
                {
                    if (_selected.Photo == null)
                        _selected.Photo = new Photo();
                    _selected.Photo.FilePath = TxtPhotoPath.Text;
                }
            }
            else
            {
                var specialist = new Specialist
                {
                    FirstName = TxtFirstName.Text.Trim(),
                    LastName = TxtLastName.Text.Trim(),
                    PhoneNumber = TxtPhone.Text.Trim(),
                    Specialization = TxtSpecialization.Text.Trim()
                };

                if (!string.IsNullOrEmpty(TxtPhotoPath.Text))
                    specialist.Photo = new Photo { FilePath = TxtPhotoPath.Text };

                _db.Specialists.Add(specialist);
            }

            _db.SaveChanges();
            LoadData();
            ShowDetail();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => ShowDetail();
    }
}