using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfAppT.Data;
using WpfAppT.Models;

namespace WpfAppT.Views.Pages
{
    public partial class CustomersPage : Page
    {
        private readonly AppDbContext _db;
        private Customer _selected;
        private bool _isEdit;

        public CustomersPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadData();
        }

        private void LoadData()
        {
            CustomersGrid.ItemsSource = _db.Customers
                .Include(c => c.Cars)
                    .ThenInclude(car => car.Brand)
                .Include(c => c.Records)
                .ToList();
            ShowDetail();
        }

        private void CustomersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = CustomersGrid.SelectedItem as Customer;
            BtnDelete.IsEnabled = _selected != null;

            if (_selected == null) { ClearDetail(); return; }

            ShowDetail();
            DetailPlaceholder.Visibility = Visibility.Collapsed;
            DetailName.Text = $"{_selected.FirstName} {_selected.LastName}";
            DetailPhone.Text = _selected.PhoneNumber;
            CarsList.ItemsSource = _selected.Cars;
            DetailVisits.Text = _selected.Records?.Count.ToString() ?? "0";
        }

        private void CustomersGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selected == null) return;
            _isEdit = true;
            ShowForm("Edit Customer", _selected);
        }

        private void ShowDetail()
        {
            DetailPanel.Visibility = Visibility.Visible;
            FormPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowForm(string title, Customer c = null)
        {
            FormTitle.Text = title;
            TxtFirstName.Text = c?.FirstName ?? "";
            TxtLastName.Text = c?.LastName ?? "";
            TxtPhone.Text = c?.PhoneNumber ?? "";
            DetailPanel.Visibility = Visibility.Collapsed;
            FormPanel.Visibility = Visibility.Visible;
        }

        private void ClearDetail()
        {
            DetailPlaceholder.Visibility = Visibility.Visible;
            DetailName.Text = "";
            DetailPhone.Text = "";
            CarsList.ItemsSource = null;
            DetailVisits.Text = "";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _isEdit = false;
            ShowForm("Add Customer");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var result = MessageBox.Show(
                $"Delete {_selected.FirstName} {_selected.LastName}?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _db.Customers.Remove(_selected);
                _db.SaveChanges();
                LoadData();
            }
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
            }
            else
            {
                _db.Customers.Add(new Customer
                {
                    FirstName = TxtFirstName.Text.Trim(),
                    LastName = TxtLastName.Text.Trim(),
                    PhoneNumber = TxtPhone.Text.Trim()
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