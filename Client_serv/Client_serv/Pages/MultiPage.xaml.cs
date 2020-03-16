using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для MultiPage.xaml
    /// </summary>
    public partial class MultiPage : Page,Ipage
    {
        MainWindow OwnerPage;
        public MultiPage()
        {
            InitializeComponent();
        }

        public MultiPage(MainWindow _OwnerPage) : this()
        {
            UpdateGrid();
            OwnerPage = _OwnerPage;
        }

        public void UpdateGrid(int buildID,int roomID)
        {
            UpdateGridBuildings(buildID);
            UpdateGridRooms(roomID);
        }

        public void UpdateGrid()
        {
            UpdateGrid(-1,-1);
        }

        public void UpdateGridBuildings(int selectID)
        {
            // Обновление таблицы данных 'Корпуса'
            DataGrid dg = dgBuildings;
            HelperClass.SaveSortDataGrid(dg);
            
            // В using происходит обращение к БД и выборка значений из таблицы 'Корпуса'
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
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
            else if (dg.SelectedIndex == -1 && dg.Items.Count > 0)
            {
                dg.SelectedIndex = 0;
            }
            HelperClass.LoadSortDataGrid(dg);
        }

        public void UpdateGridRooms(int selectID)
        {
            DataGrid dg = dgRooms;
            // Обновление таблицы данных 'Комнаты'
            dg = dgRooms;
            HelperClass.SaveSortDataGrid(dg);
            // Обновление таблицы 'комнаты'
            int? BuildID = (int?)dgBuildings?.SelectedValue;
            if (selectID == -1) selectID = (int)(dg?.SelectedValue ?? -1);
            using (HOSTELEntities h = new HOSTELEntities())
            {
                h.Rooms.Load();
                h.Buildings.Load();
                h.RoomTypes.Load();
                var RoomsDB = from i in h.Rooms.Local
                              where (i.BuildingID == BuildID )
                              select new
                              {
                                id = i.RoomID,
                                Building = i.Buildings?.Name,
                                Num = i.Num,
                                PlacesCount = i.PlacesCount,
                                RoomType = i.RoomTypes?.RoomType
                              };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = RoomsDB;
            }
            dg.SelectedValue = selectID;
            if (dg.SelectedIndex > -1)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
            else if (dg.SelectedIndex == -1 && dg.Items.Count > 0)
            {
                dg.SelectedIndex = 0;
            }
            HelperClass.LoadSortDataGrid(dg);
        }

        #region Обработчики событий для кнопок (Корпуса)

        private void BtnAddBuilding_Click(object sender, RoutedEventArgs e)
        {
            MultiBuildingsDialog b = new MultiBuildingsDialog(mode.Add, this);
            b.Show();
        }

        private void BtnCopyBuilding_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedIndex > -1)
            {
                MultiBuildingsDialog b = new MultiBuildingsDialog(mode.Copy, this, (int)dgBuildings.SelectedValue);
                b.Show();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnEditBuilding_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedIndex > -1)
            {
                MultiBuildingsDialog b = new MultiBuildingsDialog(mode.Update, this,(int)dgBuildings.SelectedValue);
                b.Show();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnDeleteBuilding_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedIndex > -1)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        try
                        {
                            DB.Buildings.Remove(DB.Buildings.Find(dgBuildings.SelectedValue));
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

        #endregion

        #region Обработчики событий для кнопок (Комнаты)

        private void BtnAddRooms_Click(object sender, RoutedEventArgs e)
        {
            MultiRoomsDialog dialog = new MultiRoomsDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void BtnCopyRooms_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedIndex > -1)
            {
                MultiRoomsDialog dialog = new MultiRoomsDialog(mode.Copy, this, (int)dgRooms.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnEditRooms_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedIndex > -1)
            {
                MultiRoomsDialog dialog = new MultiRoomsDialog(mode.Update, this, (int)dgRooms.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnDeleteRooms_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedIndex > -1)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        try
                        {
                            DB.Rooms.Remove(DB.Rooms.Find(dgRooms.SelectedValue));
                            DB.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show($"Ошибка при удалении \n Возможно в других таблицах есть ссылки на эту запись");
                            return;
                        }
                    }
                    UpdateGridRooms(-1);
                }
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        #endregion

        private void dgBuildings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGridRooms(0);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);
        }
    }
}
