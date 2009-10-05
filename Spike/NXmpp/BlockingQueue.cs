#region License

/*
 * Copyright (c) 2007-2008, openmetaverse.org
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the openmetaverse.org nor the names
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

#endregion

using System.Collections.Generic;
using System.Threading;

namespace System.Collection.Generic {
	/// <summary>
	/// Same as Queue except Dequeue function blocks until there is an object to return.
	/// Note: This class does not need to be synchronized
	/// </summary>
	internal class BlockingQueue<T> : Queue<T> {
		private readonly object syncRoot;
		private bool open;

		/// <summary>
		/// Create new BlockingQueue.
		/// </summary>
		/// <param name="col">The System.Collections.ICollection to copy elements from</param>
		internal BlockingQueue(IEnumerable<T> col)
			: base(col) {
			syncRoot = new object();
			open = true;
		}

		/// <summary>
		/// Create new BlockingQueue.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the queue can contain</param>
		internal BlockingQueue(int capacity)
			: base(capacity) {
			syncRoot = new object();
			open = true;
		}

		/// <summary>
		/// Create new BlockingQueue.
		/// </summary>
		internal BlockingQueue() {
			syncRoot = new object();
			open = true;
		}

		/// <summary>
		/// Gets flag indicating if queue has been closed.
		/// </summary>
		internal bool Closed {
			get { return !open; }
		}

		/// <summary>
		/// BlockingQueue Destructor (Close queue, resume any waiting thread).
		/// </summary>
		~BlockingQueue() {
			Close();
		}

		/// <summary>
		/// Remove all objects from the Queue.
		/// </summary>
		internal new void Clear() {
			lock (syncRoot) {
				base.Clear();
			}
		}

		/// <summary>
		/// Remove all objects from the Queue, resume all dequeue threads.
		/// </summary>
		internal void Close() {
			lock (syncRoot) {
				open = false;
				base.Clear();
				Monitor.PulseAll(syncRoot); // resume any waiting threads
			}
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Queue.
		/// </summary>
		/// <returns>Object in queue.</returns>
		internal new T Dequeue() {
			return Dequeue(Timeout.Infinite);
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Queue.
		/// </summary>
		/// <param name="timeout">time to wait before returning</param>
		/// <returns>Object in queue.</returns>
		internal T Dequeue(TimeSpan timeout) {
			return Dequeue(timeout.Milliseconds);
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Queue.
		/// </summary>
		/// <param name="timeout">time to wait before returning (in milliseconds)</param>
		/// <returns>Object in queue.</returns>
		internal T Dequeue(int timeout) {
			lock (syncRoot) {
				while (open && (base.Count == 0)) {
					if (!Monitor.Wait(syncRoot, timeout))
						throw new InvalidOperationException("Timeout");
				}
				if (open) {
					return base.Dequeue();
				}
				throw new InvalidOperationException("Queue Closed");
			}
		}

		internal bool Dequeue(int timeout, ref T obj) {
			lock (syncRoot) {
				while (open && (base.Count == 0)) {
					if (!Monitor.Wait(syncRoot, timeout))
						return false;
				}
				if (open) {
					obj = base.Dequeue();
					return true;
				}
				else {
					obj = default(T);
					return false;
				}
			}
		}

		/// <summary>
		/// Adds an object to the end of the Queue
		/// </summary>
		/// <param name="obj">Object to put in queue</param>
		internal new void Enqueue(T obj) {
			lock (syncRoot) {
				base.Enqueue(obj);
				Monitor.Pulse(syncRoot);
			}
		}

		/// <summary>
		/// Open Queue.
		/// </summary>
		internal void Open() {
			lock (syncRoot) {
				open = true;
			}
		}
	}
}