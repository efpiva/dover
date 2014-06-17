using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace AddOne.Framework.Remoting
{
    /// <summary>
    /// Wraps an instance of TInterface. If the instance is a 
    /// MarshalByRefObject, this class acts as a sponsor for its lifetime 
    /// service (until disposed/finalized). Disposing the sponsor implicitly 
    /// disposes the instance.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    [Serializable]
    [SecurityPermission(SecurityAction.Demand, Infrastructure = true)]
    public sealed class Sponsor<TInterface> : ISponsor, IDisposable where TInterface : class
    {

        private TInterface mInstance;

        /// <summary>
        /// Gets the wrapped instance of TInterface.
        /// </summary>
        public TInterface Instance
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Instance");
                else
                    return mInstance;
            }
            private set
            {
                mInstance = value;
            }
        }

        /// <summary>
        /// Gets whether the sponsor has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initialises a new instance of the Sponsor&lt;TInterface&gt; class, 
        /// wrapping the specified object instance.
        /// </summary>
        /// <param name="instance"></param>
        public Sponsor(TInterface instance)
        {
            Instance = instance;

            if (Instance is MarshalByRefObject)
            {
                object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)Instance);
                if (lifetimeService is ILease)
                {
                    ILease lease = (ILease)lifetimeService;
                    lease.Register(this);
                }
            }
        }

        /// <summary>
        /// Finaliser.
        /// </summary>
        ~Sponsor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the sponsor and the instance it wraps.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the sponsor and the instance it wraps.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (Instance is IDisposable) ((IDisposable)Instance).Dispose();

                    if (Instance is MarshalByRefObject)
                    {
                        object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)Instance);
                        if (lifetimeService is ILease)
                        {
                            ILease lease = (ILease)lifetimeService;
                            lease.Unregister(this);
                        }
                    }
                }

                Instance = null;
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Renews the lease on the instance as though it has been called normally.
        /// </summary>
        /// <param name="lease"></param>
        /// <returns></returns>
        TimeSpan ISponsor.Renewal(ILease lease)
        {
            if (IsDisposed)
                return TimeSpan.Zero;
            else
                return LifetimeServices.RenewOnCallTime;
        }
    }
}
