# WellsFargoToMint
Wells Fargo sucks. Currently, while it technically works with Mint, all that Mint is able to retrieve is a total balance for each account. As I have a number of large transactions and all income occurring within my Wells Fargo account, I want to be able to get transaction data from there for proper budgeting.

Codeplex had a [project](https://www.codeproject.com/Articles/1183561/Importing-transactions-into-Mint-com-using-Csharp "project") to do this via command line, but the download link was broken. Fortunately, they displayed the bulk of the difficult code for implementing the procedures. I am basing my project off of this, using Selenium Web Driver to push *.csv bank data to the Mint website.

# Current State
I am currently refactoring the code to be more general and extendable. It is also being changed to work via a GUI in WPF in order to make it easier to use.

# Future Development
I also don't see any reason why I can't automate the process of downloading the Wells Fargo accounts as *.csv files, so that I can directly update Mint with very little effort. I will investigate doing this using similar methods.