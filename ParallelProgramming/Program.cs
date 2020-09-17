using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgramming
{
    public interface IAsyncInit
    {
        Task InitTask { get; }
    }

    public class MyClass : IAsyncInit
    {
        public MyClass()
        {
            InitTask = InitAsync();
        }
        public Task InitTask { get; }

        private async Task InitAsync()
        {
            await Task.Delay(1000);
        }
    }

    public class MyOtherClass : IAsyncInit
    {
        private readonly MyClass myClass;
        public MyOtherClass(MyClass myClass)
        {
            this.myClass = myClass;
            InitTask = InitAsync();
        }
        public Task InitTask { get; }

        private async Task InitAsync()
        {
            //if(myClass is IAsyncInit ai)

            await Task.Delay(1000);
        }
    }

    public class stuff
    {
        private static int value;

        private readonly Lazy<Task<int>> AutoIncValue =
            new Lazy<Task<int>>(async () =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                return value++;
            });

        private readonly Lazy<Task<int>> AutoIncValue2 =
            new Lazy<Task<int>>(() => Task.Run(async () =>
            {
                await Task.Delay(1000);
                return value++;
            }));

        //public AsyncLazy<int> AutoIncValue3 =
        //    new AsyncLazy<int>(async () =>
        //   {
        //       await Task.Delay(1000);
        //       return value++;
        //   });

        public async void UseValue()
        {
            int value = await AutoIncValue.Value;
        }
    }

    public class Program
    {
        //static ReaderWriterLockSlim padlockReaderWriter = new ReaderWriterLockSlim();

        // for multiple padlockReaderWriter.EnterReadLock();, and padlockReaderWriter.ExitReadLock();
        static ReaderWriterLockSlim padlockReaderWriter = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        static Random random = new Random();

        static SpinLock sl = new SpinLock(true);

        // What is ConcurrentDictionary C#? 
        // ConcurrentDictionary is thread-safe collection class to store key/value pairs.
        // It internally uses locking to provide you a thread-safe class. 
        // It provides different methods as compared to Dictionary class. 
        // We can use TryAdd, TryUpdate, TryRemove, and TryGetValue to do CRUD operations on ConcurrentDictionary.

        // ConcurrentDictionary is primary for use 
        // in an environment where you'd updating the dictionary from multiple threads (or async tasks). 
        // You can use a standard Dictionary from as much code as you like if it's from a single thread ;) ... 
        //When this thread calls Add , it'll throw an exception.
        private static ConcurrentDictionary<string, string> capitals =
            new ConcurrentDictionary<string, string>();

        static BlockingCollection<int> messages =
            new BlockingCollection<int>(new ConcurrentBag<int>(), 10);

        static CancellationTokenSource cts = new CancellationTokenSource();

        static Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"phase {b.CurrentPhaseNumber} is finished");
        });

        static Barrier barrier2 = new Barrier(participantCount: 5);

        private static int taskCount = 5;

        static CountdownEvent cte = new CountdownEvent(taskCount);

        private static Random random1 = new Random();

        private static ParallelLoopResult result;
        public static void Water()
        {
            Console.WriteLine("Putting the kettle on (taks a bit longer)");
            Thread.Sleep(2000);
            barrier.SignalAndWait();
            Console.WriteLine("Putting into the cup");
            barrier.SignalAndWait();
            Console.WriteLine("Putting the kettle away");
        }

        public static void Cup()
        {
            Console.WriteLine("Finding the nicest cup of tea (fast)");
            barrier.SignalAndWait();
            Console.WriteLine("Adding tea.");
            barrier.SignalAndWait();
            Console.WriteLine("Adding suger");
        }

        static void Main(string[] args)
        {
            #region Creating Task
            ///////////////////////////////////// Creating a Task //////////////////////////////////////

            //Task.Factory.StartNew(() => CreatingandStartingTask.Write('.'));

            //var t = new Task(() => CreatingandStartingTask.Write('?'));
            //t.Start();
            //CreatingandStartingTask.Write('-');


            //Task t = new Task(CreatingandStartingTask.Write, "hello");
            //t.Start();

            //Task.Factory.StartNew(CreatingandStartingTask.Write, 123);


            // use generic type of task
            //string text1 = "testing1", text2 = "testing2";
            //var task1 = new Task<int>(CreatingandStartingTask.TextLength, text1);
            //task1.Start();
            //Task<int> task2 = Task.Factory.StartNew(CreatingandStartingTask.TextLength, text2);

            //Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            //Console.WriteLine($"Length of '{text2}' is {task2.Result}");


            ///////////////////////////////////// Creating a Task //////////////////////////////////////

            #endregion

            #region Cancellation task

            //var cts = new CancellationTokenSource(); //using System.Threading;
            //var token = cts.Token;

            //token.Register(() =>
            //{
            //    Console.WriteLine($"Concelation has been requested");
            //});

            //var t = new Task(() =>
            //{
            //    int i = 0;
            //    while (true)
            //    {
            //        //if (token.IsCancellationRequested)
            //        //{
            //        //    throw new OperationCanceledException();
            //        //}
            //        //else

            //        token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //    }

            //}, token);
            //t.Start();

            //Task.Factory.StartNew(() =>
            //{
            //    token.WaitHandle.WaitOne();
            //    Console.WriteLine("Wait handle relesed, canceltion was requested");
            //});


            //Console.ReadKey();
            //cts.Cancel();



            //var planned = new CancellationTokenSource();
            //var preventative = new CancellationTokenSource();
            //var emergency = new CancellationTokenSource();

            //var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
            //    planned.Token, preventative.Token, emergency.Token);

            //Task.Factory.StartNew(() =>
            //{
            //    int i = 0;
            //    while(true)
            //    {
            //        paranoid.Token.ThrowIfCancellationRequested();
            //        Console.WriteLine($"{i++}\t");
            //        Thread.Sleep(1000);
            //    }
            //}, paranoid.Token);

            //Console.ReadKey();
            //emergency.Cancel();

            //Console.ReadKey();

            #endregion

            #region Wating for Time to Pass

            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            //var t = new Task(() =>
            //{
            //    Console.WriteLine("Press any key to disram: you have d seconds");
            //    bool cencelled = token.WaitHandle.WaitOne(5000);
            //    Console.WriteLine(cencelled ? "Bomb disarmed" : "BOOM!!");



            //}, token);
            //t.Start();


            //Console.ReadKey();
            //cts.Cancel();

            //Console.ReadKey();

            #endregion

            #region Waiting for Tasks

            //var cts = new CancellationTokenSource();
            //var token = cts.Token;

            //var t = new Task(() =>
            //{
            //    Console.WriteLine("I take 5 seconds");

            //    for (int i = 0; i < 5; i++)
            //    {
            //        token.ThrowIfCancellationRequested();
            //        Thread.Sleep(1000);
            //    }

            //    Console.WriteLine("I'm done");
            //}, token);

            //t.Start();

            //Task t2 = Task.Factory.StartNew(() => Thread.Sleep(3000), token);


            ////Console.ReadKey();
            ////cts.Cancel();

            ////Task.WaitAll(t, t2);
            ////Task.WaitAny(new[] { t, t2}, 6000, token);  // wait untill any of task done
            //Task.WaitAll(new[] { t, t2 }, 4000, token);

            //Console.WriteLine($"Task t status {t.Status}");
            //Console.WriteLine($"Task t2 status {t2.Status}");


            //Console.WriteLine("Main Program done");
            //Console.ReadKey();

            #endregion

            #region Exception Handling

            //try
            //{
            //    Test();
            //}
            //catch (AggregateException ex)
            //{
            //    foreach (var e in ex.InnerExceptions)
            //    {
            //        Console.WriteLine($"Handled elsewhere: {ex.GetType()}");
            //    }
            //}


            //Console.WriteLine("Main program done");
            //Console.ReadKey();
            #endregion

            #region Critical area , Interlocked ,Spin Locking
            //////// Data Sharing and Synchronization
            // padlock
            // he lock keyword ensures that one thread does not enter a critical section of code 
            // while another thread is in that critical section. Lock is a keyword shortcut for acquiring a lock 
            // for the piece of code for only one thread.

            // A critical section is a section of code where if more than one thread was to be doing it an the same time, 
            // they could interfere with each other is such a way as to cause an incorrect result or other malfunction.


            // Interlocked
            // spin locking and Lock Recursion

            // SpinLock are typically used when working with interrupts to perform busy waiting inside a loop 
            // till the resource is made available. SpinLock don't cause the thread to be preempted, rather, 
            // it continues to spin till lock on the resource is released.

            // What is the advantage of Spinlocks?
            // Because they avoid overhead from operating system process rescheduling or context switching, 
            // spinlocks are efficient if threads are likely to be blocked for only short periods.For this reason, 
            // operating - system kernels often use spinlocks.

            //var tasks = new List<Task>();
            //var ba = new BankAccount();

            //SpinLock sl = new SpinLock();

            //for (int i = 0; i < 10; i++)
            //{
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            var lockTaken = false;
            //            try
            //            {
            //                sl.Enter(ref lockTaken);
            //                ba.Deposit(100);
            //            }
            //            finally
            //            {
            //                if (lockTaken) sl.Exit();
            //            }                        
            //        }
            //    }));

            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            var lockTaken = false;

            //            try
            //            {
            //                sl.Enter(ref lockTaken);
            //                ba.WithDraw(100);
            //            }
            //            finally
            //            {
            //                if (lockTaken) sl.Exit();
            //            }

            //        }
            //    }));
            //}

            //Task.WaitAll(tasks.ToArray());
            //Console.WriteLine($"Final balance is {ba.Balance}");

            // LockRecursion(5);

            #endregion

            #region Mutex
            // Mutex
            // A mutual exclusion (“Mutex”) is a mechanism that acts as a flag to prevent two threads from performing 
            // one or more actions simultaneously. A Mutex is like a C# lock, but it can work across multiple processes. ... 
            // A Mutex is a synchronization primitive that can also be used for interprocess synchronization.

            //var tasks = new List<Task>();
            //var ba = new BankAccount();
            //var ba2 = new BankAccount();

            //Mutex mutex = new Mutex();
            //Mutex mutex2 = new Mutex();

            //for (int i = 0; i < 10; i++)
            //{
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            bool haveLock = mutex.WaitOne();
            //            try
            //            {
            //                ba.Deposit(1);
            //            }
            //            finally
            //            {
            //                if (haveLock) mutex.ReleaseMutex();
            //            }

            //        }
            //    }));

            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            bool haveLock = mutex2.WaitOne();
            //            try
            //            {
            //                ba2.Deposit(1);
            //            }
            //            finally
            //            {
            //                if (haveLock) mutex2.ReleaseMutex();
            //            }
            //        }
            //    }));

            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            bool haveLock = WaitHandle.WaitAll(new[] { mutex, mutex2 });
            //            try
            //            {
            //                ba.Tansfer(ba2, 1);
            //            }
            //            finally
            //            {
            //                if (haveLock)
            //                {
            //                    mutex.ReleaseMutex();
            //                    mutex2.ReleaseMutex();
            //                }
            //            }
            //        }
            //    }));
            //}

            //Task.WaitAll(tasks.ToArray());
            //Console.WriteLine($"Final balance ba is {ba.Balance}");
            //Console.WriteLine($"Final balance ba2 is {ba2.Balance}");

            #endregion

            #region Mutex real world example

            //const string appName = "MyApp";

            //Mutex mutex;

            //try
            //{
            //    mutex = Mutex.OpenExisting(appName);
            //    Console.WriteLine($"Sorry, {appName} is already running");
            //}
            //catch (WaitHandleCannotBeOpenedException e)
            //{
            //    Console.WriteLine("We can run the program just fine");
            //    mutex = new Mutex(false, appName);
            //}


            //Console.ReadKey();
            //mutex.ReleaseMutex();

            #endregion

            #region Reader - Writer Locks

            // NET Framework provides several threading locking primitives. The ReaderWriterLock is one of them. 
            // The ReaderWriterLock class is used to synchronize access to a resource. At any given time, 
            // it allows concurrent read access to multiple (essentially unlimited) threads, 
            // or it allows write access for a single thread.

            //int x = 0;
            //var tasks = new List<Task>();

            //for (int i = 0; i < 10; i++)
            //{

            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        //padlockReaderWriter.EnterReadLock();

            //        padlockReaderWriter.EnterUpgradeableReadLock();

            //        if(i%2 == 0)
            //        {
            //            padlockReaderWriter.EnterWriteLock();
            //            x = 125;
            //            padlockReaderWriter.EnterWriteLock();
            //        }

            //        Console.WriteLine($"Entered read lock, x = {x}");
            //        Thread.Sleep(5000);

            //        padlockReaderWriter.ExitReadLock();
            //        Console.WriteLine($"Exited read lock, x = {x}");

            //    }));

            //    try
            //    {
            //        Task.WaitAll(tasks.ToArray());
            //    }
            //    catch (AggregateException ex)
            //    {
            //        // AggregateException is used to consolidate multiple failures into a single, 
            //        // throwable exception object. It is used extensively in the Task Parallel Library (TPL) 
            //        // and Parallel LINQ (PLINQ). ... For additional information, see the Aggregating Exceptions entry in the . 
            //        // NET Matters blog.
            //        ex.Handle(e =>
            //        {
            //            Console.WriteLine(ex);
            //            return true;
            //        });
            //    }
            //}

            //while (true)
            //{
            //    Console.ReadKey();
            //    padlockReaderWriter.EnterWriteLock();
            //    Console.Write("Write lock acquired");
            //    int newValue = random.Next(10);
            //    x = newValue;
            //    Console.WriteLine($"Set x = {x}");
            //    padlockReaderWriter.ExitWriteLock();
            //    Console.WriteLine("Write lock released");
            //}

            #endregion

            #region Concurrent Collections

            // ConcurrentDictionary is primary for use 
            // in an environment where you'd updating the dictionary from multiple threads (or async tasks). 
            // You can use a standard Dictionary from as much code as you like if it's from a single thread ;) ... 
            //When this thread calls Add , it'll throw an exception.
            #region Concurrrent Dictionary

            //Task.Factory.StartNew(AddParis).Wait();
            ////Task.Factory.StartNew(AddRose).Wait();
            //AddParis();
            ////AddRose();

            ////capitals["Russia"] = "Leningard";
            //capitals["Russia"] = "Moscow";

            //capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + " ---> Moscow");
            //Console.WriteLine($"The capital of Russia is {capitals["Russia"]}");


            ////capitals["Sweden"] = "Uppsala";
            //var capOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            //Console.WriteLine($"The capital of sweden id {capOfSweden}");


            //const string toRemove = "Russia";
            //string removed;
            //var didRemove = capitals.TryRemove(toRemove, out removed);
            //if (didRemove)
            //{
            //    Console.WriteLine($"We just removed {removed}");
            //}
            //else
            //{
            //    // if capitals["Russia"] is commented, it will not longer available in the dictionary the
            //    // execute Failed to remove the capital
            //    Console.WriteLine($"Failed to remove the capital of {toRemove}");
            //}

            //foreach (var kv in capitals)
            //{
            //    Console.WriteLine($" - {kv.Value} is the capital of {kv.Key}");
            //}

            //Console.ReadKey();

            #endregion

            //C# ConcurrentQueue is a thread-safe collection class. 
            // It is introduced in .NET 4.0 with other concurrent collection classes. 
            // It provides a thread-safe First-In-First-Out (FIFO) data structure. You can read more about Queue here.

            // What is concurrent queue in C#?
            // The concurrent queue is a data structure designed to accept multiple threads reading and writing 
            // to the queue without you needing to explicitly lock the data structure. ... 
            // Because you are using a concurrent collection, you can have multiple threads adding items and 
            // multiple threads removing items.
            #region ConcurrentQueue



            //var q = new ConcurrentQueue<int>();
            //q.Enqueue(1);
            //q.Enqueue(2);
            //q.Enqueue(3);
            //q.Enqueue(4);

            //// 2 1 <- front

            //int result;
            //if(q.TryDequeue(out result))
            //{
            //    Console.WriteLine($"Removed element {result}");
            //}

            //if (q.TryDequeue(out result))
            //{
            //    Console.WriteLine($"Removed element {result}");
            //}


            //if (q.TryPeek(out result))
            //{
            //    Console.WriteLine($"Front element is {result}");
            //}

            #endregion

            // ConcurrentStack is a thread - safe generic collection class introduced in . 
            // NET 4.0 framework.It provides a Last-In-First-Out(LIFO) data structure. ...
            // It internally uses locking to synchronize different threads.
            #region ConcurrentStack

            //var stack = new ConcurrentStack<int>();
            //stack.Push(1);
            //stack.Push(2);
            //stack.Push(377);
            //stack.Push(4);
            //stack.Push(56);

            //int result;
            //if (stack.TryPeek(out result))
            //    Console.WriteLine($"{result} is on top");

            //if (stack.TryPop(out result))
            //    Console.WriteLine($"Popped {result}");

            //var items = new int[5];
            //if(stack.TryPopRange(items, 0, 5) > 0)
            //{
            //    var text = string.Join(", ", items.Select(i => i.ToString()));
            //    Console.WriteLine($"Popped these items: {text}");
            //}


            #endregion

            // C# ConcurrentBag is one of the thread-safe collection classes introduced in . 
            // NET 4.0. ConcurrentBag allows you to store objects in unordered way. Contrary to ConcurrentDictionary class, 
            // it allows you to store duplicate objects.
            // stack LIFO
            // Queue FIFO
            #region ConcurrentBag

            //var bag = new ConcurrentBag<int>();
            //var tasks = new List<Task>();

            //for (int i = 0; i < 10; i++)
            //{
            //    var i1 = i;
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        bag.Add(i1);
            //        Console.WriteLine($"{Task.CurrentId} has added {i1}");
            //        int result;
            //        if (bag.TryPeek(out result))
            //        {
            //            Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
            //        }
            //    }));

            //    Task.WaitAll(tasks.ToArray());


            //    int last;
            //    if(bag.TryTake(out last))
            //    {
            //        Console.WriteLine($"I got {last}");
            //    }
            //}

            #endregion

            // BlockingCollection is a collection class which ensures thread-safety. 
            // Multiple threads can add and remove objects in BlockingCollection concurrently.
            #region Blockingcollection and the Producer-Consumer Pattern

            //Task.Factory.StartNew(ProduceAndConsume, cts.Token);

            //Console.ReadKey();
            //cts.Cancel();

            #endregion

            #endregion

            #region Task Coordination

            // Create continuations using the Task Parallel Library that can run immediately 
            // after the execution of the antecedent is complete or to chain tasks as continuations.
            #region Continuations

            //var task = Task.Factory.StartNew(() =>
            //{
            //    Console.WriteLine("Boil the water");
            //});


            //var task2 = task.ContinueWith(t =>
            //{
            //    Console.WriteLine($"Completed task {t.Id}, pour water into cup");
            //});

            //task2.Wait();

            //var task = Task.Factory.StartNew(() => "Task 1");
            //var task2 = Task.Factory.StartNew(() => "Task 2");
            //var task3 = Task.Factory.ContinueWhenAll(new[] { task, task2 }, tasks =>   // ContinueWhenAny
            //{

            //    Console.WriteLine("Task Completed");
            //    foreach (var t in tasks)
            //        Console.WriteLine(" - " + t.Result);


            //});

            //task3.Wait();

            #endregion

            // A child Task, a.k.a a nested Task, is a Task that's started within the body of another Task. 
            // The containing Task is called the parent. .
            #region Child Tasks

            //var parent = new Task(() =>
            //{
            //    var child = new Task(() =>
            //    {
            //        Console.WriteLine("Child tasks starting.");
            //        Thread.Sleep(3000);
            //        Console.WriteLine("Child task finishing");
            //        throw new Exception(); // used to check excetion maybe occur or not
            //    }, TaskCreationOptions.AttachedToParent);

            //    var completionHandler = child.ContinueWith(t =>
            //    {
            //        Console.WriteLine($"Hooray, task {t.Id}'s state is {t.Status}");
            //    }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);

            //    var failHandler = child.ContinueWith(t =>
            //    {
            //        Console.WriteLine($"Oops, task {t.Id}'s state is {t.Status}");
            //    }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);


            //    child.Start();

            //});

            //parent.Start();

            //try
            //{
            //    parent.Wait();
            //}
            //catch (AggregateException ex)
            //{
            //    ex.Handle(e => true);
            //}

            #endregion

            // Barrier is a synchronization primitive that enables multiple threads
            // (known as participants) to work concurrently on an algorithm in phases.
            #region Barrier

            //var water = Task.Factory.StartNew(Water);
            //var cup = Task.Factory.StartNew(Cup);

            //var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, t =>
            // {
            //     Console.WriteLine("Enoy your cup of tea");
            // });

            //tea.Wait();

            // example 01

            //Task[] tasks = new Task[5];

            //for (int i = 0; i < 5; i++)
            //{
            //    int j = i;
            //    tasks[j] = Task.Factory.StartNew(() =>
            //    {
            //        test(j);
            //    });
            //}

            //Task.WaitAll(tasks);


            #endregion

            // C# CountdownEvent unblocks a waiting thread when its receives signal a certain number of times.
            // Its is used in fork - join scenarios.
            // Its signal count can be increment dynamically.
            // CountdownEvent can be reuse by using Reset method.
            #region CountdownEvent

            //for (int i = 0; i < taskCount; i++)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        Console.WriteLine($"Entering task {Task.CurrentId}");
            //        Thread.Sleep(random1.Next(3000));
            //        cte.Signal();
            //        Console.WriteLine($"Existing task {Task.CurrentId}");
            //    });
            //}

            //var finalTask = Task.Factory.StartNew(() =>
            // {
            //     Console.WriteLine($"Writing for the other tasks to complete in {Task.CurrentId}");
            //     cte.Wait();
            //     Console.WriteLine($"All tasks completed");
            // });

            //finalTask.Wait();

            #endregion

            // ManualResetEventSlim uses busy spinning for a short time while it waits for the event to become signaled. 
            // When wait times are short, spinning can be much less expensive than waiting by using wait handles.
            // AutoResetEvent and ManualResetEvent are used in threading and help us to manage synchronization using signals. 
            // For example, suppose there are 2 threads, Thread1 and Thread2 and 1 main thread created by the Main() method.
            #region MonualRestEventSlim and AutoRestEvent
            //var evt = new ManualResetEventSlim();
            //var avt = new AutoResetEvent(true);

            //Task.Factory.StartNew(() =>
            //{
            //    Console.WriteLine("Boiling water");
            //    //evt.Set();
            //    avt.Set();
            //});

            //var makeTea = Task.Factory.StartNew(() =>
            //{
            //    Console.WriteLine("Waiting for water...");
            //    //evt.Wait();
            //    avt.WaitOne();
            //    Console.WriteLine("Here is your tea");
            //    var ok = avt.WaitOne(1000);
            //    if (ok)
            //    {
            //        Console.WriteLine("Enjoy your tea");
            //    }
            //    else
            //    {
            //        Console.WriteLine("No tea for you.");
            //    }
            //});

            //makeTea.Wait();

            #endregion

            // The SemaphoreSlim class is the recommended semaphore for synchronization within a single app.
            // A lightweight semaphore controls access to a pool of resources that is local to your application.
            //  When you instantiate a semaphore, you can specify the maximum number of threads that can enter the semaphore concurrently.
            #region SamaphoresSlim

            //var semaphore = new SemaphoreSlim(2, 10);
            //for (int i = 0; i < 20; i++)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        Console.WriteLine($"Entering task {Task.CurrentId}");
            //        semaphore.Wait();
            //        Console.WriteLine($"Exixting task {Task.CurrentId}");
            //    });
            //}

            //while (semaphore.CurrentCount <= 2)
            //{
            //    Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
            //    Console.ReadKey();
            //    semaphore.Release(2);
            //}

            #endregion
            #endregion

            #region Parallel Loops

            #region Parallel Invoke/For/Foreach

            //var a = new Action(() => Console.WriteLine($"First : {Task.CurrentId}"));
            //var b = new Action(() => Console.WriteLine($"Second : {Task.CurrentId}"));
            //var c = new Action(() => Console.WriteLine($"Third : {Task.CurrentId}"));

            //Parallel.Invoke(a, b, c);

            //Parallel.For(1, 11, i =>
            //  {
            //      Console.WriteLine($"{i }\t");
            //  });

            //string[] words = {"oh","what", "a", "night" };
            //Parallel.ForEach(words, word =>
            //{
            //    Console.WriteLine($"{word} has length {word.Length} (task {Task.CurrentId}");
            //});

            ////var po = new ParallelOptions();
            ////po.CancellationToken;

            //Parallel.ForEach(Range(1, 20, 3), Console.WriteLine);

            #endregion

            #region Breaking, Cancellations and Exceptions

            //try
            //{
            //    Demo();
            //}
            //catch (AggregateException ex)
            //{
            //    ex.Handle(e =>
            //    {
            //        Console.WriteLine($"Error is : {ex.Message}");
            //        return true;
            //    });               
            //}
            //catch (OperationCanceledException)
            //{

            //}

            #endregion

            #region Thread Local Storage

            //int sum = 0;
            //Parallel.For(1, 1001,
            //    () => 0,
            //    (x, state, tls) =>
            //    {
            //        tls += x;
            //        Console.WriteLine($"Task {Task.CurrentId} has sum {tls}");
            //        return tls;
            //    }, partialSum =>
            //    {
            //        Console.WriteLine($"Partial value of task {Task.CurrentId} is {partialSum}");
            //        Interlocked.Add(ref sum, partialSum);
            //    });

            //Console.WriteLine($"Sum of 1..100 = {sum}");


            #endregion

            #region Partitioning

            //var summary = BenchmarkRunner.Run<Program>();
            //Console.WriteLine(summary);

            #endregion

            #endregion

            #region Parallel LINQ

            #region AsParellel and ParallelQuery

            //const int count = 50;

            //var items = Enumerable.Range(1, count).ToArray();
            //var results = new int[count];

            //items.AsParallel().ForAll(x =>
            //{
            //    int newValue = x * x * x;
            //    Console.Write($"{newValue} ({Task.CurrentId})");
            //    results[x - 1] = newValue;
            //});

            //Console.WriteLine();
            //Console.WriteLine();

            //foreach (var item in results)
            //{
            //    Console.WriteLine($"{item}\t");
            //}
            //Console.WriteLine();

            //var cubes = items.AsParallel().AsOrdered().Select(x => x * x * x);
            //foreach (var item in cubes)
            //{
            //    Console.Write($"{item}\t");
            //}
            //Console.WriteLine();
            #endregion

            #region Cancellation and Exceptions

            //var cts = new CancellationTokenSource();
            //var items = ParallelEnumerable.Range(1, 20);

            //var results = items.WithCancellation(cts.Token).Select(i =>
            //{
            //    double result = Math.Log10(i);

            //    //if (result > 1) throw new InvalidOperationException();

            //    Console.WriteLine($"i = {i}, tid = {Task.CurrentId}");
            //    return result;
            //});

            //try
            //{
            //    foreach (var c in results)
            //    {
            //        if (c > 1)
            //            cts.Cancel();

            //        Console.WriteLine($"result = {c}");
            //    }
            //}
            //catch (AggregateException ae)
            //{
            //    ae.Handle(e =>
            //    {
            //        Console.WriteLine($"{e.GetType().Name} : {e.Message}");
            //        return true;
            //    });
            //}
            //catch (OperationCanceledException e)
            //{
            //    Console.WriteLine($"Cancelled : {e.Message}");
            //}

            #endregion

            #region Merge Options

            //var numbers = Enumerable.Range(1, 20).ToArray();

            //var results = numbers
            //    .AsParallel()
            //    .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
            //    .Select(x =>
            //    {
            //        var result = Math.Log10(x);
            //        Console.WriteLine($"Produced {result}");
            //        return result;
            //    });

            //foreach (var result in results)
            //{
            //    Console.WriteLine($"Consumed {result}");
            //}

            #endregion

            #region Custom Aggregation

            //var sum = Enumerable.Range(1, 1000).Sum();

            //Console.WriteLine($"sum = {sum}");

            //int sum1 = 0;
            //for (int i = 1; i <= 1000; i++)
            //{

            //    sum1 += i;
            //    Console.WriteLine(sum1);
            //}

            //var sum2 = Enumerable.Range(1, 1000)
            //    .Aggregate(0, (i, acc) => i + acc);

            //Console.WriteLine($"sum = {sum}");

            //var sum3 = ParallelEnumerable.Range(1, 1000)
            //    .Aggregate(
            //        0,
            //        (partialSum, i) => partialSum += i,
            //        (total, subtotal) => total += subtotal,
            //        i => i);

            //Console.WriteLine($"sum = {sum3}");

            #endregion

            #endregion


            #region Asynchronous Programming

            #region Async Factory Method

            //var foo = new Foo();
            //await foo.InitAsync();

            //Foo xAsync = await Foo.CreateASYNC();

            #endregion

            #region Asynchronous Initialization Pattern

            var myClass = new MyClass();

            //if (myClass is IAsyncInit ai)
            //    await ai.InitTask();

            //if(myClass is IAsyncInit ai){

            //}
            //await ai.In

            #endregion

            #region Asynchronous Lazy Initialization



            #endregion

            #region Value Task

            #endregion

            #endregion

            Console.ReadKey();

        }

        [Benchmark]
        public void SquareEachValue()
        {
            const int count = 100000;
            var values = Enumerable.Range(0, count);
            var results = new int[count];
            Parallel.ForEach(values, x => { results[x] = (int)Math.Pow(x, 2); });
        }

        public static void Demo()
        {
            var cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;

            result = Parallel.For(0, 20, po, (int i, ParallelLoopState state) =>
            {
                Console.WriteLine($"{i} [{Task.CurrentId}]");

                if (i == 10)
                {
                    //state.Stop();
                    //state.Break();
                    //throw new Exception();
                    cts.Cancel();
                }
            });

            Console.WriteLine();
            Console.WriteLine($"Wa loop completed? {result.IsCompleted}");
            if (result.LowestBreakIteration.HasValue)
                Console.WriteLine($"Lowest break iteration is {result.LowestBreakIteration}");
        }


        public static IEnumerable<int> Range(int start, int end, int step)
        {
            for (int i = start; i < end; i += step)
            {
                yield return i;
            }
        }

        static void test(int i)
        {
            Console.WriteLine($"Get data from server {i}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            barrier2.SignalAndWait();

            Console.WriteLine($"Backup data to db {i} ");
            barrier2.SignalAndWait();
        }

        static void ProduceAndConsume()
        {
            var producer = Task.Factory.StartNew(RunProducer);
            var consumer = Task.Factory.StartNew(RunConsumer);

            try
            {
                Task.WaitAll(new[] { producer, consumer }, cts.Token);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => true);
            }
        }

        private static void RunConsumer()
        {
            foreach (var item in messages.GetConsumingEnumerable())
            {
                cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"-{item}\t");
                Thread.Sleep(random.Next(1000));
            }
        }

        private static void RunProducer()
        {
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                int i = random.Next(100);
                messages.Add(i);
                Console.WriteLine($"+{i}\t");
                Thread.Sleep(random.Next(100));
            }
        }

        public static void AddRose()
        {
            bool success = capitals.TryAdd("Red", "Rose");
            string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "SecondThread";
            Console.WriteLine($"{who} {(success ? "added " : "did not added")} the element");
        }

        public static void AddParis()
        {
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "Main Thread";
            Console.WriteLine($"{who} {(success ? "added " : "did not add")} the element");
        }

        public static void LockRecursion(int x)
        {
            bool lockTaken = false;

            try
            {
                sl.Enter(ref lockTaken);
            }
            catch (LockRecursionException e)
            {

                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    LockRecursion(x - 1);
                    sl.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x = {x}");
                }
            }
        }

        /// <summary>
        /// Exception Handling in Task
        /// </summary>
        private static void Test()
        {
            var t = Task.Factory.StartNew(() => { throw new InvalidOperationException("Can't do this!") { Source = "t" }; });

            var t2 = Task.Factory.StartNew(() => { throw new InvalidOperationException("Can't access this!") { Source = "t2" }; });

            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                //foreach (var e in ae.InnerExceptions)
                //{
                //    Console.WriteLine($"Exception {e.GetType()} from {e.Source}");
                //}

                ae.Handle(e =>
                {
                    if (e is InvalidOperationException)
                    {
                        Console.WriteLine("Invalid op!");
                        return true;
                    }
                    else return false;
                });
            }
        }
    }

    class Foo
    {
        private Foo()
        {
            //
        }

        //public async Task<Foo> InitAsync()
        //{
        //    await Task.Delay(1000);
        //    return this;
        //}

        private async Task<Foo> InitAsync()
        {
            await Task.Delay(1000);
            return this;
        }

        public static Task<Foo> CreateASYNC()
        {
            var result = new Foo();
            return result.InitAsync();
        }
    }
}
