using Client_serv.Pages;
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
using System.Windows.Shapes;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для MultiRoomsDialog.xaml
    /// </summary>
    public partial class MultiRoomsDialog : Window
    {
        MultiPage CurPage;
        mode CurMode;
        int ID;
        public MultiRoomsDialog()
        {
            InitializeComponent();
        }

        public MultiRoomsDialog(mode dialogMode, MultiPage page, int fieldID = -1) : this()
        {
            CurMode = dialogMode;
            ID = fieldID;
            CurPage = page;
            FillFields();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (Check() && Save())
            {
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool Check()
        {
            if (cbBuilding.SelectedIndex == -1)
            {
                MessageBox.Show("Комната не может быть без корпуса! Выберите корпус");
                return false;
            }
            if (cbRoomType.SelectedIndex == -1)
            {
                MessageBox.Show("Комната должна быть какого-то типа! Выберите тип комнаты");
                return false;
            }
            if (TbNum.Text.Trim() == "")
            {
                MessageBox.Show("Комната не может быть без номера! Заполните поле 'Номер комнаты'");
                return false;
            }
            try
            {
                int.Parse(TbRoomPlaces.Text);
            }
            catch
            {
                MessageBox.Show("Поле кол-во комнат должно быть числом!");
                return false;
            }
            return true;
        }

        bool Save()
        {
            try
            {
                using (HOSTELEntities db = new HOSTELEntities())
                {
                    Rooms r = new Rooms();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            FillObject(r);
                            db.Rooms.Add(r);
                            break;

                        case mode.Update:
                            r = db.Rooms.Find(ID);
                            FillObject(r);
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGridRooms(r.RoomID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                return false;
            }
            return true;
        }

        private void FillFields()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                // Заполнение значениями комбобокс, в котором отображаются корпуса
                db.Buildings.Load();
                cbBuilding.ItemsSource = db.Buildings.Local;
                cbBuilding.DisplayMemberPath = "Name";
                cbBuilding.SelectedValuePath = "BuildingID";
                if (cbBuilding.Items.Count > 0) cbBuilding.SelectedIndex = 0;

                // Заполнение значениями комбобокс, в котором отображаются типы комнат
                db.RoomTypes.Load();
                cbRoomType.ItemsSource = db.RoomTypes.Local;
                cbRoomType.DisplayMemberPath = "RoomType";
                cbRoomType.SelectedValuePath = "RtyID";
                if (cbRoomType.Items.Count > 0) cbRoomType.SelectedIndex = 0;
                switch (CurMode)
                {
                    case mode.Add:
                        Title = "Добавление";
                        cbBuilding.SelectedValue = (int?)CurPage?.dgBuildings?.SelectedValue ?? -1;
                        break;

                    case mode.Copy:
                        Title = "Добавление на основе существующего";
                        goto fill;

                    case mode.Update:
                        Title = "Изменение";
                    fill:
                        Rooms r = db.Rooms.Find(ID);
                        cbBuilding.SelectedValue = r.BuildingID;
                        TbNum.Text = r.Num;
                        TbRoomPlaces.Text = r.PlacesCount?.ToString() ?? "0";
                        cbRoomType.SelectedValue = r.RtyID;
                        break;
                }
            }
        }

        private void FillObject(Rooms r)
        {
            r.BuildingID = (int?)cbBuilding.SelectedValue;
            r.Num = TbNum.Text;
            r.PlacesCount = Convert.ToInt32(TbRoomPlaces.Text);
            r.RtyID = (int?)cbRoomType.SelectedValue;
        }
    }
}
