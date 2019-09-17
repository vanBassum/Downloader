using MasterLibrary.Bindable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Downloader
{
    public delegate void WorkDone<P>(P workArg, object result);

    public class WorkerPool<T, P> where T : AWorker<P>
    {
        public event WorkDone<P> OnWorkerDone;
        private List<T> _Workers;
        private SynchronizationContext ctx;
        private ConcurrentQueue<P> _Work;

        public int WorkCount { get {return _Work.Count; } }

        public WorkerPool(int numberOfWorkers)
        {
            ctx = SynchronizationContext.Current;
            _Work = new ConcurrentQueue<P>();
            _Workers = new List<T>();

            for (int i = 0; i < numberOfWorkers; i++)
            {
                T worker = Activator.CreateInstance<T>();
                worker.OnWorkerDone += Worker_OnWorkerDone;
                _Workers.Add(worker);
            }
        }

        private void Worker_OnWorkerDone(P sender, object result)
        {
            P par;

            OnWorkerDone?.Invoke(sender, result);

            if (_Work.TryDequeue(out par))
            {
                if (!TryStart(par))
                {
                    _Work.Enqueue(par);
                }
            }
        }

        public void AddWork(P par)
        {
            if (!TryStart(par))
            {
                _Work.Enqueue(par);
            }
        }



        bool TryStart(P par)
        {
            int ind = -1;

            ctx?.Send(delegate
            {
                ind = _Workers.FindIndex(w => !w.IsBusy);
            }, null);

            if (ind != -1)
            {
                _Workers[ind].StartWork(par);
                return true;
            }
            return false;
        }

    }

    public abstract class AWorker<P>
    {
        public abstract event WorkDone<P> OnWorkerDone;
        public bool IsBusy { get; private set; } = false;
        protected abstract void StartWorker(P parameter);
        public P Work { get; private set; }
        private SynchronizationContext ctx;

        public AWorker()
        {
            ctx = SynchronizationContext.Current;
            OnWorkerDone += AWorker_OnWorkerDone;
        }

        private void AWorker_OnWorkerDone(P sender, object result)
        {
            ctx.Send(delegate
            {
                IsBusy = false;
            }, null);
        }

        public void StartWork(P parameter)
        {
            Work = parameter;
            ctx.Send(delegate
            {
                IsBusy = true;
            }, null);
            StartWorker(parameter);
        }
    }
}
