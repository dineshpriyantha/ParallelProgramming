using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    public class BankAccount
    {
        //public object padlock = new object();
        //public int Balance { get; private set; }

        private int balance;

        public int Balance
        {
            get { return balance; }
            private set { balance = value; }
        }

        public void Deposit(int amount)
        {
            // +=
            // op1:temp <- get_Balance() + amount
            // op2: set_Balance(temp)

            //lock (padlock)
            //{
            //    Balance += amount;
            //}

            //Interlocked.Add(ref balance, amount);


            balance += amount;

        }

        public void WithDraw(int amount)
        {
            //lock (padlock)
            //{
            //    Balance -= amount;
            //}

            //Interlocked.Add(ref balance, -amount);

            balance -= amount;
        }

        public void Tansfer(BankAccount where, int amount)
        {
            Balance -= amount;
            where.Balance += amount;
        }
    }
}
