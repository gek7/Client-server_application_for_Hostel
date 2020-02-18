using Client_serv.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client_serv
{
    public enum mode
    {
        Add,
        Copy,
        Update,
    }

    public partial class MainWindow : Window
    {
        DispatcherTimer timeTimer = new DispatcherTimer();
        public TabControl MainTabControl { get; }
        public static string connectionString = "Data Source=41-03;Initial Catalog=HOSTEL;Integrated Security=True";
        public MainWindow()
        {
            InitializeComponent();
            timeTimer.Tick += TimeTimer_Tick;
            timeTimer.Interval = TimeSpan.FromSeconds(0.5);
            timeTimer.Start();
            CurTime.Text = DateTime.Now.ToString();
        }

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            CurTime.Text = DateTime.Now.ToString();
        }
        #region События

        private void table_rooms_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Комнаты";
            Frame f = new Frame();
            t.Content = f;
            RoomsPage p = new RoomsPage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            pages.Items.Add(t);
        }

        private void table_students_Click(object sender, RoutedEventArgs e)
        {

        }
        private void close_app_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Table_posts_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Должности";
            Frame f = new Frame();
            t.Content = f;
            PostsPage p = new PostsPage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            pages.Items.Add(t);
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Группы";
            Frame f = new Frame();
            t.Content = f;
            GroupsPage p = new GroupsPage(this);
            p.Margin = new Thickness(0, 10, 0, 0);
            f.Navigate(p);
            pages.Items.Add(t);
        }

        private void Table_building_Click(object sender, RoutedEventArgs e)
        {
            TabItem t = new TabItem();
            t.Header = "Корпуса";
            Frame f = new Frame();
            BuildingsPage b = new BuildingsPage(this);
            
            f.Navigate(b);
            t.Content = f;
            pages.Items.Add(t);
        }
        #endregion
    }
}
