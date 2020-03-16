using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_serv.Pages
{
    /// <summary>
    /// Логика взаимодействия для GroupsPage.xaml
    /// </summary>
    public partial class GroupsPage : Page,Ipage 
    {
        MainWindow OwnerPage;
        public GroupsPage()
        {
            InitializeComponent();
        }
        public GroupsPage(MainWindow _OwnerPage) : this()
        {
            UpdateGrid();
            OwnerPage = _OwnerPage;
        }


        public void UpdateGrid(int selectID)
        {
            // Сохранение сортировки в статические поля класса HelperClass
            HelperClass.SaveSortDataGrid(dg);
            // Если переданный в метод ID == -1, то оставить выбранный элемент или поставить в первый элемент
            
            if (selectID == -1) selectID = (int)(dg?.SelectedValue ?? -1);
            //Создание соединения с БД
            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                // Открытие соединения
                connection.Open();
                // Переменной присваивается запрос, который в будущем будет выполнен
                string SqlQuery = "select * from Groups";
                // Создаётся экземпляр SqlCommand для выполнения запроса. В кач-ве параметров передаётся запрос и соединение к БД
                SqlCommand command = new SqlCommand(SqlQuery, connection);
                // SqlDataAdapter - это посредник между БД и приложением
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                // ds - источник для таблицы на форме
                DataTable ds = new DataTable();
                // Адаптер заполняет таблицу ds значениями из запроса
                adapter.Fill(ds);
                // Источник для таблицы на форме - ds
                dg.ItemsSource = ds.DefaultView;
                // Каждая строка таблицы будет хранить значение поля GroupID
                dg.SelectedValuePath = "GroupID";
            }
            // Выбранная строка = строка с GroupID равным selectID
            dg.SelectedValue = selectID;
            // Если выбрана какая-то строка
            if (dg.SelectedIndex > -1)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
            else if (dg.SelectedIndex == -1 && dg.Items.Count > 0)
            {
                dg.SelectedIndex = 0;
            }
            // Вернуть сортировку (Во время обновления данных сортировка сбрасывается, для этого мы её сохраняли)
            HelperClass.LoadSortDataGrid(dg);
        }

        public void UpdateGrid()
        {
            UpdateGrid(-1);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            GroupsDialog dialog = new GroupsDialog(mode.Add, this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex>-1)
            {
                GroupsDialog dialog = new GroupsDialog(mode.Copy, this, (int)dg.SelectedValue);
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
                GroupsDialog dialog = new GroupsDialog(mode.Update, this, (int)dg.SelectedValue);
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
                    using(SqlConnection connection = new SqlConnection(MainWindow.connectionString))
                    {
                        connection.Open();
                        try
                        {
                            string SqlQuery = $"Delete from Groups where(GroupID={dg.SelectedValue})";
                            SqlCommand command = new SqlCommand(SqlQuery, connection);
                            // Выполнение запроса, которая ничего не возвращает
                            command.ExecuteNonQuery();
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
