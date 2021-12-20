using System;

namespace TestAppService
{
	public interface IAppService1
	{
		void Foo(int n);
		int Foo2();
	}

	public interface IAppService2
	{
		void Bar();
		void Bar2();
	}

	public class AppService : IAppService1
	{
		private readonly IAppService1 _service1;
		private readonly IAppService2 _service2;
		private readonly AppService _self;
		public int value { get; set; }
		public string name;

		public void Foo(int n)
		{
			_service2.Bar();
			Foo2Internal(n);

			_service1.Foo(n);
		}

		private void Foo2Internal(int n)
		{
			_service2.Bar2();
		}

        public int Foo2()
        {
			return 0;
        }
    }
}
