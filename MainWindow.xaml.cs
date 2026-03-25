using System.Windows;
using System.Windows.Controls;
using WpfAppT.Data;
using WpfAppT.Views.Pages;

namespace WpfAppT
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _db;

        public MainWindow(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            Navigate(new FrontPage(_db), BtnDashboard);
        }

        private void Navigate(Page page, Button active)
        {
            MainFrame.Navigate(page);
            // Скидаємо всі кнопки
            BtnDashboard.Style = (Style)Resources["RibbonBtn"];
            BtnRecords.Style = (Style)Resources["RibbonBtn"];
            BtnSpecialists.Style = (Style)Resources["RibbonBtn"];
            BtnCustomers.Style = (Style)Resources["RibbonBtn"];
            // Активна кнопка
            active.Style = (Style)Resources["RibbonBtnActive"];
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
            => Navigate(new FrontPage(_db), BtnDashboard);

        private void BtnRecords_Click(object sender, RoutedEventArgs e)
            => Navigate(new RecordsPage(_db), BtnRecords);

        private void BtnSpecialists_Click(object sender, RoutedEventArgs e)
            => Navigate(new SpecialistsPage(_db), BtnSpecialists);

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
            => Navigate(new CustomersPage(_db), BtnCustomers);
    }
}