using System;
using System.Collections.Generic;

namespace Assignment3
{
    // a. Record for financial data
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b. Interface for transaction processors
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
           Console.WriteLine($"[Bank Transfer] Completed {transaction.Amount:C} towards {transaction.Category} ");

        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
           Console.WriteLine($"[Mobile Money] You sent {transaction.Amount:C} towards {transaction.Category} ");

        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
           Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} towards {transaction.Category} ");

        }
    }

    // d. Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New Balance: {Balance:C}");
        }
    }

    // e. Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction successful. Updated Balance: {Balance:C}");
            }
        }
    }

    // f. FinanceApp
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Savings account
            var savingsAccount = new SavingsAccount("ACC123", 1000m);

            // ii. Sample transactions
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 250m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 500m, "Entertainment");

            // iii. Process transactions
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);

            // iv. Apply transactions
            savingsAccount.ApplyTransaction(t1);
            savingsAccount.ApplyTransaction(t2);
            savingsAccount.ApplyTransaction(t3);

            // v. Add to transaction list
            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);

            // Summary
            Console.WriteLine("\nTransaction Summary:");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"ID: {tx.Id}, Category: {tx.Category}, Amount: {tx.Amount:C}, Date: {tx.Date.ToShortDateString()}");
            }
        }
    }
}