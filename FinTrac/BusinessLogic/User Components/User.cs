﻿using BusinessLogic.Category_Components;
using BusinessLogic.Account_Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic.User_Components
{
    public class User
    {
        #region Properties
        public long UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Address { get; set; }
        public List<Category> MyCategories { get; set; }
        public List<Account> MyAccounts { get; set; }

        #endregion

        #region Constructors


        public User()
        {
        }

        public User(string firstName, string lastName, string email, string password, string? address)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Address = address;
            if (ValidateUser())
            {
                MyCategories = new List<Category>();
                MyAccounts = new List<Account>();
            }


        }
        #endregion

        #region Validate User

        private bool ValidateUser()
        {
            bool emailValidated = ValidateEmail(Email);
            bool passwordValidated = ValidatePassword(Password);
            bool firstNameValidated = ValidateFirstName(FirstName);
            bool lastNameValidated = ValidateLastName(LastName);
            return emailValidated && passwordValidated && firstNameValidated && lastNameValidated;
        }

        #endregion

        #region ValidateFirstName

        public bool ValidateFirstName(string possibleFirstName)
        {
            string pattern = "^[A-Za-z ]+$";
            bool hasNullOrSpaceOrEmpty = string.IsNullOrWhiteSpace(possibleFirstName);
            bool hasSpecialChar = !Regex.IsMatch(possibleFirstName, pattern);


            if (hasNullOrSpaceOrEmpty || hasSpecialChar)
            {
                throw new ExceptionValidateUser("ERROR ON FIRSTNAME");
            }

            return true;

        }

        public string RemoveAllUnsenseSpaces(string stringToCorrect)
        {
            return stringToCorrect.Trim();
        }

        #endregion

        #region ValidateLastName

        public bool ValidateLastName(string possibleLastName)
        {
            string pattern = "^[A-Za-z ]+$";
            //Regex checks if a specified regular expression pattern matches the entire input string.
            bool hasSpecialChar = !Regex.IsMatch(possibleLastName, pattern);
            bool hasNullOrEmptyOrSpace = string.IsNullOrWhiteSpace(possibleLastName);


            if (hasNullOrEmptyOrSpace || hasSpecialChar)
            {
                throw new ExceptionValidateUser("ERROR ON LASTNAME");
            }

            return true;
        }


        #endregion

        #region ValidateEmail

        public bool ValidateEmail(string possibleEmail)
        {
            string pattern = @"^[a-zA-Z0-9.]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            bool hasCorrectPattern = Regex.IsMatch(possibleEmail, pattern);

            for (int i = 0; i < possibleEmail.Length; i++)
            {
                if (possibleEmail[i].Equals('.') && possibleEmail[i + 1].Equals('.'))
                {
                    hasCorrectPattern = false;
                }
            }

            if (!hasCorrectPattern)
            {
                throw new ExceptionValidateUser("ERROR ON EMAIL");
            }
            return true;
        }

        #endregion

        #region ValidatePassword
        public bool ValidatePassword(string posiblePassword)
        {
            ValidatePasswordHasCorrectLength(posiblePassword);
            ValidatePasswordUppercase(posiblePassword);

            return true;
        }

        private void ValidatePasswordHasCorrectLength(string posiblePassword)
        {
            const int minLength = 10;
            const int maxLength = 30;



            if (posiblePassword.Length < minLength || posiblePassword.Length > maxLength)
            {
                throw new ExceptionValidateUser("ERROR ON PASSWORD");
            }
        }

        private void ValidatePasswordUppercase(string posiblePassword)
        {
            bool hasUpperCase = false;
            const int minAsciiUperCase = 65;
            const int maxAsciiUperCase = 90;

            for (int i = 0; i < posiblePassword.Length && !hasUpperCase; i++)
            {
                if (posiblePassword[i] <= maxAsciiUperCase && posiblePassword[i] >= minAsciiUperCase)
                {
                    hasUpperCase = true;
                }
            }

            if (!hasUpperCase)
            {
                throw new ExceptionValidateUser("ERROR ON PASSWORD");
            }
        }
        #endregion

        #region Category Management

        #region Add Category

        public void AddCategory(Category categoryToAdd)
        {
            setCategoryId(categoryToAdd);
            ValidateRegisteredCategory(categoryToAdd);
            MyCategories.Add(categoryToAdd);
        }

        private void setCategoryId(Category categoryToAdd)
        {
            categoryToAdd.CategoryId = MyCategories.Count + 1;
        }

        private void ValidateRegisteredCategory(Category categoryToAdd)
        {
            foreach (var category in MyCategories)
            {
                if (category.Name == categoryToAdd.Name)
                {
                    throw new ExceptionCategoryManagement("Category name already registered, impossible to create another Category.");
                }
            }
        }

        #endregion

        #region Get Categories
        public List<Category> GetCategories()
        {
            List<Category> listOfCategories = MyCategories;
            return listOfCategories;
        }
        #endregion

        #region Modify Category

        public void ModifyCategory(Category categoryToUpdate)
        {
            int lengthOfCategoryList = MyCategories.Count;
            bool flag = false;

            for (int i = 0; i < lengthOfCategoryList && !flag; i++)
            {
                if (MyCategories[i].CategoryId == categoryToUpdate.CategoryId)
                {
                    MyCategories[i] = categoryToUpdate;
                    flag = true;
                }
            }
        }

        #endregion

        #region Delete Category

        public void DeleteCategory(Category categoryToDelete)
        {
            MyCategories.Remove(categoryToDelete);
        }

        #endregion

        #endregion

        #region Account Management

        public void addAccount(Account accountToAdd)
        {
            
            foreach(var account in MyAccounts)
            {
                if(account.Name == accountToAdd.Name)
                {
                    throw new ExceptionAccountManagement("Account name already added.");
                }
            }

            MyAccounts.Add(accountToAdd);

        }


        #endregion
    }
}