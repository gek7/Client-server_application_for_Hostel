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
    /// Логика взаимодействия для StudentsDialog.xaml
    /// </summary>
    public partial class StudentsDialog : Window
    {
        StudentsPage CurPage;
        mode CurMode;
        int ID;
        public StudentsDialog()
        {
            InitializeComponent();
        }

        public StudentsDialog(mode dialogMode, StudentsPage page, int fieldID = -1) : this()
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
                MessageBox.Show("За записью справочника студент должна быть закреплена запись из справочника люди!");
                return false;
            }
            if (dpAd.SelectedDate == null) 
            {
                MessageBox.Show("Дата зачисления обязательно должна быть указана");
                return false;
            }
            if (dpFinished.SelectedDate < dpAd.SelectedDate) 
            {
                MessageBox.Show("Дата зачисления не может быть больше даты выпуска");
                return false;
            }
            if (cbGroups.SelectedIndex == -1)
            {
                MessageBox.Show("Студент должен быть закреплён за какой-нибудь группой! Выберите группу!");
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
                    Students r = new Students();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            FillObject(r);
                            db.Students.Add(r);
                            break;

                        case mode.Update:
                            r = db.Students.Find(ID);
                            FillObject(r);
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGrid(r.StudentID);
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
                db.Groups.Load();
                cbGroups.ItemsSource = db.Groups.Local;
                cbGroups.DisplayMemberPath = "GroupName";
                cbGroups.SelectedValuePath = "GroupID";
                if (cbGroups.Items.Count > 0) cbGroups.SelectedIndex = 0;
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
                        Students r = db.Students.Find(ID);
                        cbPeople.SelectedValue = r.PeopleID;
                        dpAd.SelectedDate = r.Ad;
                        cbGroups.SelectedValue = r.GroupID;
                        dpFinished.SelectedDate = r.Finished;
                        break;
                }
            }
        }

        private void FillObject(Students r)
        {
            r.PeopleID = (int?)cbPeople.SelectedValue;
            r.Ad = dpAd.SelectedDate;
            r.GroupID = (int?)cbGroups.SelectedValue;
            r.Finished = dpFinished.SelectedDate;
        }
    }
}
