#region License

// Copyright 2009 Damian Hickey
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 

#endregion

using System;
using System.Collection.Generic;
using System.Threading;

namespace NXmpp {
	internal class MainLoop : IDisposable {
		private readonly Thread _mainLoopThread;
		private readonly BlockingQueue<Task> _taskQueue = new BlockingQueue<Task>();
		private bool _isDisposed;
		private volatile bool _stopLoop;

		internal MainLoop(string name) {
			_mainLoopThread = new Thread(Loop) {Name = name, IsBackground = true};
			_mainLoopThread.Start();
		}

		#region IDisposable Members

		public void Dispose() {
			if (_isDisposed) return;
			_stopLoop = true;
			if (!_mainLoopThread.Join(2000)) {
				_mainLoopThread.Abort();
			}
			_isDisposed = true;
		}

		#endregion

		private void Loop() {
			Task task = null;
			while (!_stopLoop) {
				if (_taskQueue.Dequeue(250, ref task)) {
					task.Execute();
				}
			}
		}

		internal void Queue(Action action) {
			var task = new Task(action);
			_taskQueue.Enqueue(task);
		}

		internal void QueueWait(Action action) {
			var task = new Task(action);
			_taskQueue.Enqueue(task);
			task.Handle.WaitOne();
		}

		#region Nested type: Task

		private class Task {
			private readonly Action _action;
			private Exception _exception;

			internal Task(Action action) {
				_action = action;
				Handle = new ManualResetEvent(false);
			}

			internal ManualResetEvent Handle { get; private set; }

			internal void Execute() {
				try {
					_action();
					Handle.Set();
				}
				catch (Exception ex) {
					_exception = ex;
				}
			}
		}

		#endregion
	}
}