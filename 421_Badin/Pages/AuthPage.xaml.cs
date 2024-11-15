﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace _421_Badin.Pages
{
    /// <summary>
    /// Interaction logic for AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
        public AuthPage()
        {
            InitializeComponent();
        }

        private void PasswordPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = PasswordPasswordBox.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginPlaceholder.Visibility = LoginTextBox.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordPasswordBox.Password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }
            using (var db = new Entities())
            {
                var passwordHash = GetHash(PasswordPasswordBox.Password);
                var user = db.User
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == LoginTextBox.Text && u.Password == passwordHash);

                if (user == null)
                {
                    MessageBox.Show("Не удалось найти пользователя в такими данными");
                    return;
                }
                else
                {
                    MessageBox.Show("Добро пожаловать!");

                    switch (user.Role.ToLower())
                    {
                        case "администратор":
                            NavigationService?.Navigate(new AdminPage());
                            break;
                        case "пользователь":
                            NavigationService?.Navigate(new UserPage());
                            break;
                    }
                }

            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }
    }
}
