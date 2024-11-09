using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private static bool ContainsRussianLetters(string input)
        {
            Regex russianLetterPattern = new Regex("[А-Яа-яЁё]");
            return russianLetterPattern.IsMatch(input);
        }
        public static bool ContainsDigit(string input)
        {
            Regex digitPattern = new Regex(@"\d");
            return digitPattern.IsMatch(input);
        }
        private bool ValidatePassword(string password)
        {
            if (password.Length < 6) return false;
            else if (ContainsRussianLetters(password)) return false;
            else if (!ContainsDigit(password)) return false;

            return true;
        }

        private bool FreeLogin(string login)
        {
            using (var db = new Entities())
            {
                var user = db.User
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == login);

                if (user != null)
                {
                    return false;
                }

            }
            return true;
        }
        public RegPage()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginHint.Visibility = Visibility.Visible;
            if (LoginTextBox.Text.Length > 0)
            {
                LoginHint.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordHint.Visibility = Visibility.Visible;
            if (PasswordPasswordBox.Password.Length > 0)
            {
                PasswordHint.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordConfirmHint.Visibility = Visibility.Visible;
            if (PasswordConfirmPasswordBox.Password.Length > 0)
            {
                PasswordConfirmHint.Visibility = Visibility.Hidden;
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) ||
                string.IsNullOrEmpty(PasswordPasswordBox.Password) ||
                string.IsNullOrEmpty(PasswordConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(FIOTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все необходимые поля");
                return;
            }

            if (!FreeLogin(LoginTextBox.Text))
            {
                MessageBox.Show("Введенный логин уже занят");
                return;
            }

            if (!ValidatePassword(PasswordPasswordBox.Password))
            {
                MessageBox.Show("Ваш пароль слишком легкий");
                return;
            }

            if (PasswordPasswordBox.Password != PasswordConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пожалуйста, убедитесь что пароли совпадают");
                return;
            }

            Entities db = new Entities();
            User userObject = new User
            {
                FIO = FIOTextBox.Text,
                Login = LoginTextBox.Text,
                Password = GetHash(PasswordPasswordBox.Password),
                Role = RoleComboBox.Text
            };
            db.User.Add(userObject);
            db.SaveChanges();

            MessageBox.Show("Ваш аккаунт зарегистрирован");
            NavigationService?.GoBack();
        }
    }
}
