using System;

namespace Bank
{
    public class BankAccountWriter
    {
        private readonly IFileWriter fileWriter;

        public BankAccountWriter(IFileWriter fileWriter)
        {
            this.fileWriter = fileWriter;
        }

        public void WriteAccount(BankAccount account)
        {
            fileWriter.Write(account.Name, String.Format("{0}|{1}", account.Name, account.Balance));
        }
        public BankAccount ReadAccount(string name)
        {
            BankAccount account = null;
            var contents = fileWriter.Read(name);
            var both = contents.Split('|');
            if (both.Length == 2)
            {
                double balance = 0;
                if (double.TryParse(both[1], out balance))
                {
                    account = new BankAccount(both[0], balance);
                }
            }
            return account;
        }
    }
}