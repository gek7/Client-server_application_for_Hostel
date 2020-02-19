using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для BuildingsPage.xaml
    /// </summary>
    public partial class BuildingsPage : Page,Ipage
    {
        public DataGrid PageDataGrid => dg;
        object OwnerPage;
        public BuildingsPage()
        {
            InitializeComponent();
        }
        public BuildingsPage(MainWindow owner) : this()
        {
            updateGrid();
            OwnerPage = owner;
        }


        public void updateGrid()
        {
            using (HOSTELEntities h = new HOSTELEntities())
            {
                h.Buildings.Load();
                var BuildingDB = from i in h.Buildings.Local
                                 select new
                                 {
                                     id = i.BuildingID,
                                     Name = i.Name,
                                     Address = i.Address
                                 };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = BuildingDB;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            BuildingsDialog dialog = new BuildingsDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                BuildingsDialog dialog = new BuildingsDialog(mode.Copy, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                BuildingsDialog dialog = new BuildingsDialog(mode.Update, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        DB.Buildings.Remove(DB.Buildings.Find(dg.SelectedValue));
                        DB.SaveChanges();
                    }
                    updateGrid();
                }
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnClosePage_Click(object sender, RoutedEventArgs e)
        {
            if(OwnerPage is MainWindow)
            {
                MainWindow owner = OwnerPage as MainWindow;
                owner.pages.Items.Remove(owner.pages.SelectedItem);
            }
        }
    }
}
