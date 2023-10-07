﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Transaction_Components;
using BusinessLogic.Category_Components;
using BusinessLogic.User_Components;
using BusinessLogic.Account_Components;
using BusinessLogic.ExchangeHistory_Components;
using BusinessLogic.Goal_Components;

namespace BusinessLogic.Report_Components
{
    public abstract class Report
    {

        #region Monthly report

        public static List<ResumeOfGoalReport> MonthlyReportPerGoal(User loggedUser)
        {
            decimal[] spendingsPerCategory = CategorySpendings(loggedUser, (MonthsEnum)DateTime.Now.Month, loggedUser.MyAccounts);
            decimal totalSpent = 0;
            List<ResumeOfGoalReport> listOfSpendingsResumes = new List<ResumeOfGoalReport>();
            bool goalAchieved = false;

            foreach (var myGoal in loggedUser.MyGoals)
            {
                totalSpent = 0;
                goalAchieved = true;
                foreach (var category in myGoal.CategoriesOfGoal)
                {
                    totalSpent += spendingsPerCategory[category.CategoryId];
                }
                if (totalSpent > myGoal.MaxAmountToSpend) { goalAchieved = false; }
                ResumeOfGoalReport myResume = new ResumeOfGoalReport(myGoal.MaxAmountToSpend, totalSpent, goalAchieved);
                listOfSpendingsResumes.Add(myResume);
            }
            return listOfSpendingsResumes;
        }
        #endregion

        #region Report of all spendings per cateogry detailed

        public static List<ResumeOfSpendigsReport> GiveAllSpendingsPerCategoryDetailed(User loggedUser, MonthsEnum monthGiven)
        {
            decimal[] spendingsPerCategory = CategorySpendings(loggedUser, (MonthsEnum)monthGiven, loggedUser.MyAccounts);
            decimal totalSpentPerCategory = 0;
            decimal percentajeOfTotal = 0;
            Category categoryRelatedToSpending = new Category();
            List<ResumeOfSpendigsReport> listOfSpendingsResumes = new List<ResumeOfSpendigsReport>();

            foreach (var category in loggedUser.MyCategories)
            {
                totalSpentPerCategory = spendingsPerCategory[category.CategoryId];
                percentajeOfTotal = CalulatePercent(spendingsPerCategory, totalSpentPerCategory);
                categoryRelatedToSpending = category;

                ResumeOfSpendigsReport myCategorySpendingsResume = new ResumeOfSpendigsReport(category, totalSpentPerCategory, percentajeOfTotal);

                listOfSpendingsResumes.Add(myCategorySpendingsResume);
            }
            return listOfSpendingsResumes;
        }

        private static decimal CalulatePercent(decimal[] spendingsPerCategory, decimal totalSpentPerCategory)
        {
            return (totalSpentPerCategory / spendingsPerCategory[spendingsPerCategory.Length - 1]) * 100;
        }

        #endregion

        #region Report of All Outcome Transactions
        public static List<Transaction> GiveAllOutcomeTransactions(User loggedUser)
        {
            List<Transaction> listOfAllOutcomeTransactions = new List<Transaction>();
            foreach (var account in loggedUser.MyAccounts)
            {
                foreach (var transaction in account.MyTransactions)
                {
                    AddToListOfOutcomest(listOfAllOutcomeTransactions, transaction);
                }
            }
            return listOfAllOutcomeTransactions;
        }

        private static void AddToListOfOutcomest(List<Transaction> listOfAllOutcomeTransactions, Transaction transaction)
        {
            if (transaction.Type == TypeEnum.Outcome)
            {
                listOfAllOutcomeTransactions.Add(transaction);
            }
        }

        #endregion

        #region Report Of Spendings For Credit Card

        public static List<Transaction> ReportOfSpendingsPerCard(CreditCardAccount creditCard)
        {
            DateTime dateTimInit = GetDateTimInit(creditCard);
            List<Transaction> listOfAllOutcomeTransactions = new List<Transaction>();
            foreach (var transaction in creditCard.MyTransactions)
            {
                if (transaction.Type == TypeEnum.Outcome)

                    if (IsBetweenBalanceDates(creditCard, dateTimInit, transaction))
                    {
                        listOfAllOutcomeTransactions.Add(transaction);
                    }
            }
            return listOfAllOutcomeTransactions;
        }

        private static DateTime GetDateTimInit(CreditCardAccount creditCard)
        {
            return new DateTime(creditCard.ClosingDate.Year, creditCard.ClosingDate.Month - 1, creditCard.CreationDate.Day + 1);
        }

        private static bool IsBetweenBalanceDates(CreditCardAccount creditCard, DateTime dateTimInit, Transaction transaction)
        {
            return transaction.CreationDate.CompareTo(dateTimInit) >= 0 && transaction.CreationDate.CompareTo(creditCard.ClosingDate) <= 0;
        }

        #endregion

        #region Report Of Balance For Monetary Account

        public static decimal GiveAccountBalance(MonetaryAccount account)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods used by reports
        public static decimal ConvertDolar(Transaction myTransaction, User loggedUser)
        {
            decimal amountToReturn = myTransaction.Amount;
            if (myTransaction.Currency == CurrencyEnum.USA)
            {
                bool found = false;
                decimal dolarValue = 0;
                DateTime bestDate = DateTime.MinValue;
                foreach (ExchangeHistory exchange in loggedUser.MyExchangesHistory)
                {
                    if (!found && exchange.ValueDate > bestDate && exchange.ValueDate <= myTransaction.CreationDate)
                    {
                        bestDate = exchange.ValueDate;
                        dolarValue = exchange.Value;
                        if (exchange.ValueDate == myTransaction.CreationDate) { found = true; }
                    }
                }
                amountToReturn = myTransaction.Amount * dolarValue;
            }
            return amountToReturn;
        }

        public static decimal[] CategorySpendings(User loggedUser, MonthsEnum monthSelected, List<Account> listOfAccounts)
        {
            decimal[] spendings = new decimal[loggedUser.MyCategories.Count + 2];

            foreach (var account in listOfAccounts)
            {
                foreach (var transaction in account.MyTransactions)
                {
                    if ((MonthsEnum)transaction.CreationDate.Month == monthSelected
                        && transaction.TransactionCategory.Type == TypeEnum.Outcome)
                    {
                        decimal amountToAdd = ConvertDolar(transaction, loggedUser);
                        LoadArray(spendings, transaction, amountToAdd);
                    }
                }
            }
            return spendings;
        }
        private static void LoadArray(decimal[] arrayToLoad, Transaction transaction, decimal amountToAdd)
        {
            LoadPerCategory(arrayToLoad, transaction, amountToAdd);
            LoadTotalsInArray(arrayToLoad, transaction, amountToAdd);

        }

        private static void LoadTotalsInArray(decimal[] arrayToLoad, Transaction transaction, decimal amountToAdd)
        {
            if (transaction.Type == TypeEnum.Income)
            {
                arrayToLoad[arrayToLoad.Length - 2] += amountToAdd;
            }
            else
            {
                arrayToLoad[arrayToLoad.Length - 1] += amountToAdd;
            }
        }

        private static void LoadPerCategory(decimal[] arrayToLoad, Transaction transaction, decimal amountToAdd)
        {
            arrayToLoad[transaction.TransactionCategory.CategoryId] += amountToAdd;
        }
        #endregion


    }


    public class ResumeOfGoalReport
    {
        public decimal AmountDefined { get; set; }
        public decimal TotalSpent { get; set; }
        public bool GoalAchieved { get; set; }

        public ResumeOfGoalReport(decimal amountDefined, decimal totalSpent, bool goalAchieved)
        {
            AmountDefined = amountDefined;
            TotalSpent = totalSpent;
            GoalAchieved = goalAchieved;
        }
    }

    public class ResumeOfSpendigsReport
    {
        public Category CategoryRelated { get; set; }
        public decimal TotalSpentInCategory { get; set; }
        public decimal PercentajeOfTotal { get; set; }

        public ResumeOfSpendigsReport(Category categoryRelated, decimal totalSpent, decimal percentajeOfTotal)
        {
            CategoryRelated = categoryRelated;
            TotalSpentInCategory = totalSpent;
            PercentajeOfTotal = percentajeOfTotal;
        }

    }

}


