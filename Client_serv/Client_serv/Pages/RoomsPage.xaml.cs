using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для RoomsPagexaml.xaml
    /// </summary>
    public partial class RoomsPage : Page,Ipage
    {
        MainWindow OwnerPage;

        public DataGrid PageDataGrid => dg;

        public RoomsPage()
        {
            InitializeComponent();
        }
        public RoomsPage(MainWindow _OwnerPage) : this()
        {
            updateGrid();
            OwnerPage = _OwnerPage;
        }


        public void updateGrid()
        {
            using (HOSTELEntities h = new HOSTELEntities())
            {
                // Подгрузка таблиц ( После этого h.Таблица.Local возвращает значения из таблицы на момент вызова Load() )
                h.Rooms.Load();
                h.Buildings.Load();
                h.RoomTypes.Load();
                // var - неявный "тип", т.е неизвестный тип данных. Тип становится известным только после присвоения любого значения. 
                // Присваивать значения нужно во время объявления переменной, иначе выдаст ошибку
                var RoomsDB = from i in h.Rooms.Local
                                  // inner join Buildings on Rooms.BuildingID = Buildings.BuildingID
                              join j in h.Buildings.Local on i.BuildingID equals j.BuildingID into t
                              from t1 in t.DefaultIfEmpty()
                                  // inner join RoomTypes on RoomTypes.RtyID = Rooms.RtyID
                              join c in h.RoomTypes.Local on i.RtyID equals c.RtyID into t2
                              from t3 in t2?.DefaultIfEmpty()
                              select new
                              {
                                  id = i.RoomID,
                                  Building = t1.Name ?? "",
                                  Num = i.Num,
                                  PlacesCount = i.PlacesCount,
                                  RoomType = t3.RoomType ?? ""
                              };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = RoomsDB;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            RoomsDialog dialog = new RoomsDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                RoomsDialog dialog = new RoomsDialog(mode.Copy, this, (int)dg.SelectedValue);
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
                RoomsDialog dialog = new RoomsDialog(mode.Update, this, (int)dg.SelectedValue);
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
                            DB.Rooms.Remove(DB.Rooms.Find(dg.SelectedValue));
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
    }
}
