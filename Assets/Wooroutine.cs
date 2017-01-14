using System;
using System.Collections;
using System.Diagnostics;

namespace Wooga.Coroutines
{
    public class Wooroutine : IEnumerator
    {
	    public const string CALLED_BY = "calledBy";

        public Exception Exception { get; protected set; }

        public bool Started
        {
            get { return _enumerator != null || Completed || Canceled; }
        }

        public bool Completed
        {
            get { return _success || HasError; }
        }

	    public bool Canceled { get; private set; }

        public bool HasError
        {
            get { return Exception != null; }
        }

        protected object _returnValue;
	    protected IEnumerator _enumerator;
        protected bool _success = false;

        public Wooroutine(IEnumerator routine)
        {
            _enumerator = routine;
        }

	    public virtual bool MoveNext ()
	    {
		    try
		    {
			    if (!_enumerator.MoveNext())
			    {
				    _returnValue = _enumerator.Current;
				    _success = true;
			    }
		    }
		    catch (Exception ex)
		    {
			    Exception = ex;
		    }
		    return !Completed;
	    }

	    public void Reset ()
	    {
		    throw new NotImplementedException();
	    }

	    public object Current {
		    get { return _enumerator.Current; }
	    }

        public void Stop()
        {
	        Canceled = true;
        }

        protected void AddAccessorToExceptionData()
        {
            var sf = new StackFrame(2, false);
            Exception.Data[CALLED_BY] = sf.GetMethod().DeclaringType.FullName + "." +
                                                                     sf.GetMethod().Name;
        }
    }

	public class Wooroutine<T> : Wooroutine
	{
		public Wooroutine (IEnumerator routine) : base(routine) { }

		public T ReturnValue
		{
			get
			{
				if (!Completed)
				{
					throw new Exception(typeof(Wooroutine<T>).FullName + " did not complete.");
				}
				if (Exception != null)
				{
					AddAccessorToExceptionData();
					throw Exception;
				}
				if (_returnValue != null)
				return (T)_returnValue;

				return default(T);
			}
		}

		public override bool MoveNext ()
		{
			try
			{
				if (!_enumerator.MoveNext() ||  _enumerator.Current is T)
				{
					_returnValue = _enumerator.Current;
					_success = true;
				}
			}
			catch (Exception ex)
			{
				Exception = ex;
			}
			return !Completed;
		}
	}
}