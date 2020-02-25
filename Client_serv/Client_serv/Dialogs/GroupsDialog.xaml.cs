using Client_serv.Pages;
using System;
using System.Collections.Generic;
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

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для GroupsDialog.xaml
    /// </summary>
    public partial class GroupsDialog : Window
    {
        GroupsPage CurPage;
        mode CurMode;
        int ID;
        public GroupsDialog()
        {
            InitializeComponent();
        }

        public GroupsDialog(mode dialogMode, GroupsPage page, int fieldID = -1) : this()
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
            if (TbGroups.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return false;
            }
            return true;
        }

        bool Save()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MainWindow.connectionString ))
                {
                    connection.Open();
                    string SqlQuery = "";
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addPost;

                        case mode.Copy:
                        addPost:
                            // В переменную SqlQuery передаётся запрос на добавление записи в таблицу
                            SqlQuery = $"INSERT INTO GROUPS (GroupName) VALUES ('{TbGroups.Text}')";
                            break;

                        case mode.Update:
                            // В переменную SqlQuery передаётся запрос на изменение записи в таблице
                            SqlQuery = $"UPDATE GROUPS SET GroupName='{TbGroups.Text}' where GroupId={ID}";
                            break;
                    }
                    SqlCommand command = new SqlCommand(SqlQuery, connection);
                    // Выполняется запрос без возвращаемого значения
                    command.ExecuteNonQuery();
                    // В command передаёт новый запрос, который при выполнении вернёт последнюю добавленную запись
                    command = new SqlCommand("SELECT IDENT_CURRENT ('GROUPS')", connection);
                    // Выполняется запрос, который возвращает id последней добавленной записи и преобразуется в стровку, а из строки в int
                    int id =int.Parse(command.ExecuteScalar().ToString());
                    // Обновление таблицы, которая создала это диалоговое окно
                    CurPage.UpdateGrid(id);
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
            using (HOSTELEntities DB = new HOSTELEntities())
            {
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
                        using(SqlConnection connection = new SqlConnection(MainWindow.connectionString))
                        {
                            connection.Open();
                            string SqlQuery = $"Select * from Groups where(GroupID={ID})";
                            SqlCommand command = new SqlCommand(SqlQuery, connection);
                            // Создаётся SqlDataReader для считывания значений из запроса
                            SqlDataReader reader = command.ExecuteReader();
                            // Переходит к следующей записи (с нулевой на 1-ую)
                            reader.Read();
                            // В reader передаём какое поле нужно получить и в какой тип данных преобразовать 
                            TbGroups.Text=reader.GetString(1);
                        }
                        break;
                }
            }
        }
    }
}
