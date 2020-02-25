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
        MainWindow OwnerPage;
        public BuildingsPage()
        {
            InitializeComponent();
        }
        public BuildingsPage(MainWindow _OwnerPage) : this()
        {
            UpdateGrid();
            OwnerPage = _OwnerPage;
        }


        public void UpdateGrid(int selectID)
        {
            
            HelperClass.SaveSortDataGrid(dg);
            if (selectID == -1) selectID = (int)(dg?.SelectedValue ?? -1);
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
            dg.SelectedValue = selectID;
            if (dg.SelectedIndex > -1)
            dg.ScrollIntoView(dg.SelectedItem);
            HelperClass.LoadSortDataGrid(dg);
        }

        public void UpdateGrid()
        {
            UpdateGrid(-1);
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
                        try
                        {
                            DB.Buildings.Remove(DB.Buildings.Find(dg.SelectedValue));
                            DB.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка при удалении \n Возможно в других таблицах есть ссылки на эту запись");
                            return;
                        }
                    }
                    UpdateGrid();
                }
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnClosePage_Click(object sender, RoutedEventArgs e)
        {
                OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);
        }
    }
}
