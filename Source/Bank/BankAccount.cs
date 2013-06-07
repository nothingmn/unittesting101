using System;
using System.Threading;

namespace Bank
{
    public class BankAccount
    {
        // class under test
        public const string DebitAmountExceedsBalanceMessage = "Debit amount exceeds balance";
        public const string DebitAmountLessThanZeroMessage = "Debit amount will result in a negative balance";

        public BankAccount(string name, double startingBalance)
        {
            Name = name;
            Balance = startingBalance;

        }

        public string Name { get; set; }
        public double Balance { get; set; }

        private readonly object _balanceLock = new object();
        public void Debit(double amount)
        {
            lock (_balanceLock)
            {
                double newBalance = Balance - Math.Abs(amount);
                if (amount > Balance)
                {
                    throw new ArgumentOutOfRangeException("amount", amount, DebitAmountExceedsBalanceMessage);
                }
                if (newBalance < 0)
                {
                    throw new ArgumentOutOfRangeException("amount", amount, DebitAmountLessThanZeroMessage);
                }
                System.Threading.Thread.Sleep(100);
                Balance = newBalance;
            }
        }

        public void Credit(double amount)
        {
            lock (_balanceLock)
            {
                System.Threading.Thread.Sleep(10);
                Balance += Math.Abs(amount);
            }
        }

    }
}