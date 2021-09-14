using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
    public class Utils
    {
        /// <summary>
        /// 计算图像的方法，值越高清晰度越高
        /// </summary>
        /// <param name="ho_image"></param>
        /// <returns></returns>
        public static double CalIntensity(HObject ho_image, double zoom = 1)//wells0179
        {

            HOperatorSet.GenEmptyObj(out HObject calHob);
            if (!ho_image.IsInitialized()) return 0;
            if (zoom < 1)
            {
                HOperatorSet.ZoomImageFactor(ho_image, out calHob, 0.5, 0.5, "bilinear");
            }
            else calHob = ho_image.Clone();
            #region ***** 计算图片方差 *****
            double Deviation = 0;
            try
            {
                HOperatorSet.SobelAmp(calHob, out HObject imgAmp, "sum_abs", 3);
                HOperatorSet.Intensity(imgAmp, imgAmp, out HTuple mean, out HTuple deviation);
                //ho_image.Dispose();
                imgAmp.Dispose();
                calHob.Dispose();
                //ho_image.Dispose();
                Deviation = deviation.D;
            }
            catch (System.Exception ex)
            {

            }
            return Deviation;
            #endregion
        }
        public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
        {
            /// <summary>Whether the current thread is processing work items.</summary> 
            [ThreadStatic]
            private static bool _currentThreadIsProcessingItems;
            /// <summary>The list of tasks to be executed.</summary> 
            private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks) 
            /// <summary>The maximum concurrency level allowed by this scheduler.</summary> 
            private readonly int _maxDegreeOfParallelism;
            /// <summary>Whether the scheduler is currently processing work items.</summary> 
            private int _delegatesQueuedOrRunning = 0; // protected by lock(_tasks) 

            /// <summary> 
            /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the 
            /// specified degree of parallelism. 
            /// </summary> 
            /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param> 
            public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
            {
                if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
                _maxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            /// <summary>
            /// current executing number;
            /// </summary>
            public int CurrentCount { get; set; }

            /// <summary>Queues a task to the scheduler.</summary> 
            /// <param name="task">The task to be queued.</param> 
            protected sealed override void QueueTask(Task task)
            {
                // Add the task to the list of tasks to be processed. If there aren't enough 
                // delegates currently queued or running to process tasks, schedule another. 
                lock (_tasks)
                {
                    //Console.WriteLine("Task Count : {0} ", _tasks.Count);
                    _tasks.AddLast(task);
                    if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                    {
                        ++_delegatesQueuedOrRunning;
                        NotifyThreadPoolOfPendingWork();
                    }
                }
            }
            int executingCount = 0;
            private static object executeLock = new object();
            /// <summary> 
            /// Informs the ThreadPool that there's work to be executed for this scheduler. 
            /// </summary> 
            private void NotifyThreadPoolOfPendingWork()
            {
                ThreadPool.UnsafeQueueUserWorkItem(_ =>
                {
                    // Note that the current thread is now processing work items. 
                    // This is necessary to enable inlining of tasks into this thread. 
                    _currentThreadIsProcessingItems = true;
                    try
                    {
                        // Process all available items in the queue. 
                        while (true)
                        {
                            Task item;
                            lock (_tasks)
                            {
                                // When there are no more items to be processed, 
                                // note that we're done processing, and get out. 
                                if (_tasks.Count == 0)
                                {
                                    --_delegatesQueuedOrRunning;

                                    break;
                                }

                                // Get the next item from the queue 
                                item = _tasks.First.Value;
                                _tasks.RemoveFirst();
                            }


                            // Execute the task we pulled out of the queue 
                            base.TryExecuteTask(item);
                        }
                    }
                    // We're done processing items on the current thread 
                    finally { _currentThreadIsProcessingItems = false; }
                }, null);
            }

            /// <summary>Attempts to execute the specified task on the current thread.</summary> 
            /// <param name="task">The task to be executed.</param> 
            /// <param name="taskWasPreviouslyQueued"></param> 
            /// <returns>Whether the task could be executed on the current thread.</returns> 
            protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {

                // If this thread isn't already processing a task, we don't support inlining 
                if (!_currentThreadIsProcessingItems) return false;

                // If the task was previously queued, remove it from the queue 
                if (taskWasPreviouslyQueued) TryDequeue(task);

                // Try to run the task. 
                return base.TryExecuteTask(task);
            }

            /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary> 
            /// <param name="task">The task to be removed.</param> 
            /// <returns>Whether the task could be found and removed.</returns> 
            protected sealed override bool TryDequeue(Task task)
            {
                lock (_tasks) return _tasks.Remove(task);
            }

            /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary> 
            public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

            /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary> 
            /// <returns>An enumerable of the tasks currently scheduled.</returns> 
            protected sealed override IEnumerable<Task> GetScheduledTasks()
            {
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(_tasks, ref lockTaken);
                    if (lockTaken) return _tasks.ToArray();
                    else throw new NotSupportedException();
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_tasks);
                }
            }
        }

        /// <summary>
        /// 任意三点计算圆心
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static Point FindCenter(Point startPoint, Point secondPoint, Point endPoint)
        {
            double tempA1, tempA2, tempB1, tempB2, tempC1, tempC2, temp, x, y;

            tempA1 = startPoint.X - secondPoint.X;

            tempB1 = startPoint.Y - secondPoint.Y;
            tempC1 = (Math.Pow(startPoint.X, 2) - Math.Pow(secondPoint.X, 2) + Math.Pow(startPoint.Y, 2) - Math.Pow(secondPoint.Y, 2)) / 2;

            tempA2 = endPoint.X - secondPoint.X;
            tempB2 = endPoint.Y - secondPoint.Y;
            tempC2 = (Math.Pow(endPoint.X, 2) - Math.Pow(secondPoint.X, 2) + Math.Pow(endPoint.Y, 2) - Math.Pow(secondPoint.Y, 2)) / 2;

            temp = tempA1 * tempB2 - tempA2 * tempB1;
            if (temp == 0)
            {
                x = startPoint.X;
                y = startPoint.Y;
            }
            else
            {
                x = (tempC1 * tempB2 - tempC2 * tempB1) / temp;
                y = (tempA1 * tempC2 - tempA2 * tempC1) / temp;
            }
            return new Point(Convert.ToInt32(x), Convert.ToInt32(y)); //x  y为点的坐标
        }
    }
}
