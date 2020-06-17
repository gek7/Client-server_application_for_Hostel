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
    /// Логика взаимодействия для WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : Page
    {
        MainWindow OwnerPage;

        public WorkersPage()
        {
            InitializeComponent();
            dg.MouseDoubleClick += Dg_MouseDoubleClick;
        }

        private void Dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WorkersDialog dialog = new WorkersDialog(mode.Update, this, (int)dg.SelectedValue);
            dialog.ShowDialog();
        }

        public WorkersPage(MainWindow _OwnerPage) : this()
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
                // Подгрузка таблиц ( После этого h.Таблица.Local возвращает значения из таблицы на момент вызова Load() )
                h.Workers.Load();
                h.Posts.Load();
                h.People.Load();
                // var - неявный "тип", т.е неизвестный тип данных. Тип становится известным только после присвоения любого значения. 
                // Присваивать значения нужно во время объявления переменной, иначе выдаст ошибку
                var WorkersDB = from i in h.Workers.Local
                                 select new
                                 {
                                     id = i.WorkerID,
                                     Surname = i.People.Name,
                                     Ad = i.Ad,
                                     Post = i.Posts.Post,
                                     Finished = i.Finished
                                 };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = WorkersDB;
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

        public void UpdateGrid()
        {
            UpdateGrid(-1);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            WorkersDialog dialog = new WorkersDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                WorkersDialog dialog = new WorkersDialog(mode.Copy, this, (int)dg.SelectedValue);
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
                WorkersDialog dialog = new WorkersDialog(mode.Update, this, (int)dg.SelectedValue);
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
                            DB.Workers.Remove(DB.Workers.Find(dg.SelectedValue));
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
