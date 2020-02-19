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
    /// Логика взаимодействия для RoomTypesPage.xaml
    /// </summary>
    public partial class RoomTypesPage : Page,Ipage
    {
        public DataGrid PageDataGrid => dg;
        MainWindow OwnerPage;
        public RoomTypesPage()
        {
            InitializeComponent();
        }
        public RoomTypesPage(MainWindow owner) : this()
        {
            updateGrid();
            OwnerPage = owner;
        }


        public void updateGrid()
        {
            using (HOSTELEntities h = new HOSTELEntities())
            {
                h.RoomTypes.Load();
                var postDB = from i in h.RoomTypes.Local
                             select new
                             {
                                 id = i.RtyID,
                                 i.RoomType
                             };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = postDB;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            RoomTypesDialog dialog = new RoomTypesDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                RoomTypesDialog dialog = new RoomTypesDialog(mode.Copy, this, (int)dg.SelectedValue);
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
                RoomTypesDialog dialog = new RoomTypesDialog(mode.Update, this, (int)dg.SelectedValue);
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
                        DB.RoomTypes.Remove(DB.RoomTypes.Find(dg.SelectedValue));
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
            OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);
        }
    }
}
