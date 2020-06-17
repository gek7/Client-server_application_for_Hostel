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
    /// Логика взаимодействия для ContractsDialog.xaml
    /// </summary>
    public partial class ContractsDialog : Window
    {
        ContractsPage CurPage;
        mode CurMode;
        int ID;
        public ContractsDialog()
        {
            InitializeComponent();
        }

        public ContractsDialog(mode dialogMode, ContractsPage page, int fieldID = -1) : this()
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

           
            if (cbPeople.SelectedIndex == -1)
            {
                MessageBox.Show("Договор должен быть связан с человеком");
                return false;
            }
            if (cbRooms.SelectedIndex == -1)
            {
                MessageBox.Show("Договор должен быть связан с комнатой");
                return false;
            }
            if (cbRooms.SelectedIndex == -1)
            {
                MessageBox.Show("Договор должен быть связан с комнатой");
                return false;
            }
            if (dpDocDate.SelectedDate > dpAppDate.SelectedDate) 
            {
                MessageBox.Show("Дата заключения не может быть больше даты расторжения");
                return false;
            }
            if (dpPlanBegDate.SelectedDate > dpPlanEndDate.SelectedDate)
            {
                MessageBox.Show("Дата въезда не может быть больше даты выезда");
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
                    Contracts r = new Contracts();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            FillObject(r);
                            db.Contracts.Add(r);
                            break;

                        case mode.Update:
                            r = db.Contracts.Find(ID);
                            FillObject(r);
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGrid(r.ContractID);
                }
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233087)
                {
                    MessageBox.Show($"Такая комната в этом корпусе уже существует");
                }
                else
                {
                    MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                }
                return false;
            }
            return true;

        }

        private void FillFields()
        {
            using (HOSTELEntities db = new HOSTELEntities())
            {
                // Заполнение значениями комбобокс, в котором отображаются корпуса
                db.People.Load();
                cbPeople.ItemsSource = db.People.Local;
                cbPeople.DisplayMemberPath = "Name";
                cbPeople.SelectedValuePath = "PeopleID";
                if (cbPeople.Items.Count > 0) cbPeople.SelectedIndex = 0;

                // Заполнение значениями комбобокс, в котором отображаются типы комнат
                db.Rooms.Load();
                cbRooms.ItemsSource = db.Rooms.Local;
                cbRooms.DisplayMemberPath = "Num";
                cbRooms.SelectedValuePath = "RoomID";
                if (cbRooms.Items.Count > 0) cbRooms.SelectedIndex = 0;
                switch (CurMode)
                {
                    case mode.Add:
                        Title = "Добавление";
                        break;

                    case mode.Copy:
                        Title = "Добавление на основе существующего";
                        goto fill;

                    case mode.Update:
                        Title = "Изменение";
                    fill:
                        Contracts r = db.Contracts.Find(ID);
                        cbPeople.SelectedValue = r.PeopleID;
                        dpDocDate.SelectedDate = r.DocDate;
                        dpPlanBegDate.SelectedDate = r.PlanBegDate;
                        dpPlanEndDate.SelectedDate = r.PlanEndDate;
                        tbPriority.Text = r.Priority;
                        cbox.IsChecked = r.Signed ?? false;
                        dpActualEndDate.SelectedDate = r.ActualEndDate;
                        cbRooms.SelectedValue = r.RoomID;
                        dpAppDate.SelectedDate = r.AppDate;
                        break;
                }
            }
        }

        private void FillObject(Contracts r)
        {
            r.PeopleID = (int?)cbPeople.SelectedValue;
            r.DocDate = dpDocDate.SelectedDate;
            r.PlanBegDate = dpPlanBegDate.SelectedDate;
            r.PlanEndDate = dpPlanEndDate.SelectedDate;
            r.Priority = tbPriority.Text;
            r.Signed = cbox.IsChecked;
            r.ActualEndDate = dpActualEndDate.SelectedDate;
            r.RoomID =(int?) cbRooms.SelectedValue;
        }
    }
}
