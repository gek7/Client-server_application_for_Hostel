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
        public DataGrid PageDataGrid => dg;
        MainWindow OwnerPage;
        public GroupsPage()
        {
            InitializeComponent();
        }
        public GroupsPage(MainWindow _OwnerPage) : this()
        {
            updateGrid();
            OwnerPage = _OwnerPage;
        }


        public void updateGrid()
        {

            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                connection.Open();
                string SqlQuery = "select * from Groups";
                SqlCommand command = new SqlCommand(SqlQuery, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                dg.ItemsSource = ds.DefaultView;
                dg.SelectedValuePath = "GroupID";
            }
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
                            command.ExecuteNonQuery();
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
