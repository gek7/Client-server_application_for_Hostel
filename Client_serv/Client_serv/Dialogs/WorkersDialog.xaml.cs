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
    /// Логика взаимодействия для WorkersDialog.xaml
    /// </summary>
    public partial class WorkersDialog : Window
    {
        WorkersPage CurPage;
        mode CurMode;
        int ID;
        public WorkersDialog()
        {
            InitializeComponent();
        }

        public WorkersDialog(mode dialogMode, WorkersPage page, int fieldID = -1) : this()
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
            if (dpFinished.SelectedDate < dpAd.SelectedDate)
            {
                MessageBox.Show("Дата зачисления не может быть больше даты выпуска");
                return false;
            }
            if (cbPosts.SelectedIndex == -1)
            {
                MessageBox.Show("Студент должен быть закреплён за какой-нибудь группой! Выберите группу!");
                return false;
            }
            if (dpAd.SelectedDate == null)
            {
                MessageBox.Show("Дата вступления в должность обязательно должна быть указана");
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
                    Workers r = new Workers();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addRoom;

                        case mode.Copy:
                        addRoom:
                            FillObject(r);
                            db.Workers.Add(r);
                            break;

                        case mode.Update:
                            r = db.Workers.Find(ID);
                            FillObject(r);
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGrid(r.WorkerID);
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
                db.Posts.Load();
                cbPosts.ItemsSource = db.Posts.Local;
                cbPosts.DisplayMemberPath = "Post";
                cbPosts.SelectedValuePath = "PostID";
                if (cbPosts.Items.Count > 0) cbPosts.SelectedIndex = 0;
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
                        Workers r = db.Workers.Find(ID);
                        cbPeople.SelectedValue = r.PeopleID;
                        dpAd.SelectedDate = r.Ad;
                        cbPosts.SelectedValue = r.PostID;
                        dpFinished.SelectedDate = r.Finished;
                        break;
                }
            }
        }

        private void FillObject(Workers r)
        {
            r.PeopleID = (int?)cbPeople.SelectedValue;
            r.Ad = dpAd.SelectedDate;
            r.PostID = (int?)cbPosts.SelectedValue;
            r.Finished = dpFinished.SelectedDate;
        }
    }
}
