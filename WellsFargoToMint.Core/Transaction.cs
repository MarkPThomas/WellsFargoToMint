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

            // Sample CSV (Mine)
            //"09/22/2017","-69.42","*","","CHASE CREDIT CRD AUTOPAY 170921 000000000055583 THOMAS MARK P"
            //"09/22/2017","-3129.86","*","","CARDMEMBER SERV WEB PYMT 170921 ***********6416 THOMAS,MARK P 04"
            //"09/19/2017","-85.08","*","","LIBERTY MUTUAL PAYMENT 170919 AOS26845535340 THOMAS MARK P"
            //"09/19/2017","-14.75","*","","LIBERTY MUTUAL PAYMENT 170919 H4726191981140 THOMAS MARK P"
            //"09/15/2017","-25.00","*","","RECURRING TRANSFER TO THOMAS M SAVINGS REF #OP03RH9VM5 XXXXXX4765"
            //"09/15/2017","5302.50","*","","Computers And St WF PAYROLL 170915 1802 Thomas, Mark"
            //"09/11/2017","-2255.10","*","","CARDMEMBER SERV WEB PYMT 170911 ***********6416 THOMAS,MARK P 03"
            //"09/11/2017","-17.32","*","","PURCHASE AUTHORIZED ON 09/11 ARCO AMPM 82914 MONTECA CA P00000000339806359 CARD 1849"
            //"09/07/2017","-4000.00","*","","ONLINE TRANSFER TO THOMAS M REF #IB03QSQVPB CUSTOM MANAGEMENT(RM) 3RD TAX PAYMENT"
            //"09/06/2017","-592.74","*","","Regence BCBS UT Auto Debit 170905 000000861091674 MARK P THOMAS"
            //"08/22/2017","-42.72","*","","CHASE CREDIT CRD AUTOPAY 170821 000000000055140 THOMAS MARK P"
            //"08/18/2017","6510.00","*","","Computers And St WF PAYROLL 170818 1802 Thomas, Mark"
            //"08/17/2017","-85.08","*","","LIBERTY MUTUAL PAYMENT 170817 AOS26845535340 THOMAS MARK P"
            //"08/17/2017","-14.75","*","","LIBERTY MUTUAL PAYMENT 170817 H4726191981140 THOMAS MARK P"
            //"08/15/2017","-25.00","*","","RECURRING TRANSFER TO THOMAS M SAVINGS REF #OP03NSHKN4 XXXXXX4765"
            //"07/28/2017","1650.60","*","","MOBILE DEPOSIT : REF NUMBER :013280280156"
            //"07/28/2017","277.42","*","","MOBILE DEPOSIT : REF NUMBER :913280279804"
            //"07/24/2017","-836.58","*","","CHASE CREDIT CRD AUTOPAY 170721 000000000107103 THOMAS MARK P"
            //"07/21/2017","4383.75","*","","Computers And St WF PAYROLL 170721 1802 Thomas, Mark"
            //"07/19/2017","-85.08","*","","LIBERTY MUTUAL PAYMENT 170719 AOS26845535340 THOMAS MARK P"
            //"07/19/2017","-14.75","*","","LIBERTY MUTUAL PAYMENT 170719 H4726191981140 THOMAS MARK P"
            //"07/17/2017","-25.00","*","","RECURRING TRANSFER TO THOMAS M SAVINGS REF #OP03LCDH68 XXXXXX4765"
            //"07/13/2017","-100.00","*","","ATM WITHDRAWAL AUTHORIZED ON 07/13 3885 WASATCH BLVD SALT LAKE CTY UT 0004697 ATM ID 4665S CARD 1849"
            //"07/11/2017","-1490.98","*","","CARDMEMBER SERV WEB PYMT 170711 ***********6416 THOMAS,MARK P 01"
            //"07/10/2017","-2856.59","*","","CARDMEMBER SERV WEB PYMT 170707 ***********6416 THOMAS,MARK P 02"
            //"06/26/2017","-5.00","*","","NON-WELLS FARGO ATM TRANSACTION FEE"
            //"06/26/2017","-103.36","*","","NON-WF ATM WITHDRAWAL AUTHORIZED ON 06/24 GRKB MAIENFELD-RAST. Maienfeld CHE 00387175430180108 ATM ID 774004 CARD 1849"

            string amount = "-5.00";
            string category = "Financial";
            string date = "06/26/2017";
            string merchant = "NON-WELLS FARGO ATM TRANSACTION FEE";

            //Transaction transaction = new Transaction(amount, category, date, merchant);
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
