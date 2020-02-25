﻿using Client_serv.Pages;
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
                            SqlQuery = $"INSERT INTO GROUPS (GroupName) VALUES ('{TbGroups.Text}')";
                            break;

                        case mode.Update:
                            SqlQuery = $"UPDATE GROUPS SET GroupName='{TbGroups.Text}' where GroupId={ID}";
                            break;
                    }
                    SqlCommand command = new SqlCommand(SqlQuery, connection);
                    command.ExecuteNonQuery();
                    command = new SqlCommand("SELECT IDENT_CURRENT ('GROUPS')", connection);
                    int id =int.Parse(command.ExecuteScalar().ToString());
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
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            TbGroups.Text=reader.GetString(1);
                        }
                        break;
                }
            }
        }
    }
}
