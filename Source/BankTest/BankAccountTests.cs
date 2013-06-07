using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank;
using NUnit.Framework;

namespace BankTest
{
    [TestFixture]
    public class BankAccountTests
    {
        [Test]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            account.Debit(debitAmount);

            // assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        }


        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Debit_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRange()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = -100.00;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            account.Debit(debitAmount);

            // assert is handled by ExpectedException
        }

        [Test]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Debit_WhenAmountIsGreaterThanBalance_ShouldThrowArgumentOutOfRange()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = 20.0;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            try
            {
                account.Debit(debitAmount);
            }
            catch (ArgumentOutOfRangeException e)
            {
                // assert
                StringAssert.StartsWith(BankAccount.DebitAmountExceedsBalanceMessage, e.Message);
                return;
            }
            Assert.Fail("No exception was thrown");
        }
        [Test]
        public void Credit_Simple_Amount_UpdatesBalance()
        {
            // arrange
            double beginningBalance = 11.99;
            double creditAmount = 4.55;
            double expected = 16.53;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            account.Credit(creditAmount);

            // assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.01, "Account not credited correctly");
        }

        [Test]
        public void Debit_Multi_Threaded()
        {
            // arrange
            double beginningBalance = 10;
            double debitAmount = -1;
            double expected = 0;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => account.Debit(debitAmount)));
            }

            Task.WaitAll(tasks.ToArray());

            // assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.01, "Account not debited correctly");
            
        }

        [Test]
        public void Debit_And_Credit_Multi_Threaded()
        {
            // arrange
            double beginningBalance = 10;
            double debitAmount = -1;
            double creditAmount = 1;
            double expected = 10;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // act
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => account.Debit(debitAmount)));
                tasks.Add(Task.Run(() => account.Credit(creditAmount)));
            }

            Task.WaitAll(tasks.ToArray());

            // assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.01, "Account balance is not correct");

        }

        [Test]
        public void DebitAccount_WriteToFile_ThenRead()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
            account.Debit(debitAmount);

            // act
            IFileWriter writer = new FileWriter();
            BankAccountWriter baw = new BankAccountWriter(writer);
            baw.WriteAccount(account);            

            // assert
            var readAccount = baw.ReadAccount(account.Name);
            Assert.AreEqual(readAccount.Balance, account.Balance);
            Assert.AreEqual(readAccount.Name, account.Name);
        }

        [Test]
        public void DebitAccount_WriteToFile_ThenRead_Mocked()
        {
            // arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);
            account.Debit(debitAmount);

            // act
            var writer = new Moq.Mock<IFileWriter>();
            writer.Setup(w => w.Write(Moq.It.IsAny<string>(), Moq.It.IsAny<string>()));
            writer.Setup(w => w.Read(Moq.It.IsAny<string>())).Returns(String.Format("{0}|{1}", account.Name, account.Balance));

            BankAccountWriter baw = new BankAccountWriter(writer.Object);
            baw.WriteAccount(account);

            // assert
            var readAccount = baw.ReadAccount(account.Name);
            Assert.AreEqual(readAccount.Balance, account.Balance);
            Assert.AreEqual(readAccount.Name, account.Name);
        }
    }
}