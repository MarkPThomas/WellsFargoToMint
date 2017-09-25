
namespace MPT.WellsFargoToMint.Core
{
    public interface ITransaction
    {
        string Date{ get; }
        string Merchant { get; }
        string Category { get; }
        string Amount { get; }
        TransactionType Type { get; }
        void Fill(string amount, string category, string date, string merchant);
    }
}
