using System;
using System.Globalization;

namespace MPT.WellsFargoToMint.Core
{
    public class Transaction : ITransaction
    {
        #region Properties
        public string Amount { get; private set; }

        public string Category { get; private set; }

        public string Date { get; private set; }

        public string Merchant { get; private set; }

        public TransactionType Type { get; private set; } = TransactionType.Error;

        #endregion

        #region Methods: Public
        public void Fill(string amount, string category, string date, string merchant)
        {
            // Test that arguments contain a valid amount and date
            if (!(isValidAmount(amount) && isValidDate(date))) { return; }

            setAmountAndType(amount);
            Date = date;
            Merchant = merchant;
            setCategory(category);
        }

        public static Transaction FromCsv(string input)
        {
            throw new NotImplementedException();
            //CSV File

            // Downloading credit card transactions is straightforward.
            // Refactoring the CSV to[Date, Merchant, Category, Amount], cleaning up the data, and applying categories, while tedious, is also straightforward.

            //However, keep the following in mind:
            //Some categories will not work, like Transfer and Credit Card Payment. Do not import “Transfer from XXX” items since they will have to be marked as an expense and this will mess up the various Trend graphs.
            //Beware of non - standard single quotes.
            //Ensure that the “negative” transactions start with a minus sign and are not in parenthesizes.
            //Do not have any blank lines at the end of the file.
            
            //A sample input file:

            //Date, Merchant, Category, Amount
            //01 / 01 / 2016,TestA,Financial,$1.00
            //01 / 01 / 2016,TestB,Financial,-$1.00
            //01 / 02 / 2016,TestC,Financial,-$1.00
            //01 / 02 / 2016,TestD,Financial,$1.00
            //01 / 03 / 2016,TestE,Financial,$1.00

            
            string amount = "-5.00";
            string category = "Financial";
            string date = "06/26/2017";
            string merchant = "NON-WELLS FARGO ATM TRANSACTION FEE";
            
            Transaction transaction = new Transaction();
            transaction.Fill(amount, category, date, merchant);
        }
        #endregion

        #region Methods: Private

        private static bool isValidAmount(string amount)
        {
            decimal amountNumber;
            return decimal.TryParse(amount, out amountNumber);
        }

        private static bool isValidDate(string date)
        {
            DateTime dateValue;
            return DateTime.TryParseExact(date, "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateValue);
        }

        private void setAmountAndType(string amount)
        {
            decimal amountNumber;
            decimal.TryParse(amount, out amountNumber);
            Type = amountNumber < 0 ? TransactionType.Expense : TransactionType.Income;
            Amount = amount;
        }

        private void setCategory(string category)
        {
            // TODO: Write logic for some auto categories, such as transfers
            Category = category;
        }
        #endregion
    }
}
