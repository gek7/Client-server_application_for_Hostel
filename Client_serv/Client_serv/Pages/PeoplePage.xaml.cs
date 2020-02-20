using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для PeoplePage.xaml
    /// </summary>
    public partial class PeoplePage : Page,Ipage
    {
        public DataGrid PageDataGrid => dg;
        MainWindow OwnerPage;
        public PeoplePage()
        {
            InitializeComponent();
        }
        public PeoplePage(MainWindow _OwnerPage) : this()
        {
            updateGrid();
            OwnerPage = _OwnerPage;
        }


        public void updateGrid()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                db.People.Load();
                var PeopleDB = from p in db.People.Local
                               select new
                               {
                                   id = p.PeopleID,
                                   p.Name,
                                   p.BirthDate,
                                   p.PspNum,
                                   p.PhoneNum,
                                   p.Email
                               };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = PeopleDB;
            }
            UpdateImage();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            PeopleDialog dialog = new PeopleDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                PeopleDialog dialog = new PeopleDialog(mode.Copy, this, (int)dg.SelectedValue);
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
                PeopleDialog dialog = new PeopleDialog(mode.Update, this, (int)dg.SelectedValue);
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
                            DB.People.Remove(DB.People.Find(dg.SelectedValue));
                            DB.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка при удалении \n Возможно в других таблицах есть ссылки на эту запись");
                            return;
                        }
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

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateImage();
        }

        private void UpdateImage()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                byte[] imgData = db.People?.Find(dg.SelectedValue)?.ImageData;
                if (imgData != null)
                {
                    MemoryStream stream = new MemoryStream(imgData);
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = stream;
                    img.EndInit();
                    pict.Source = img;
                }
                else
                {
                    if (pict.Source.ToString() != "pack://application:,,,/Images/unknownImage.png")
                    {
                        BitmapImage unknwn = new BitmapImage(new Uri("/Images/unknownImage.png", UriKind.Relative));
                        pict.Source = unknwn;
                    }
                }
            }
        }
    }
}
