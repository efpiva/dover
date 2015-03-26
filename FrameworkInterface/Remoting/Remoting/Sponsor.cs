/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace Dover.Framework.Remoting
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
