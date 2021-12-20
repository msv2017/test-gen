using AutoFixture;
using Xunit;

namespace UnitTests
{
    public class AppServiceTest
    {
        private readonly Fixture _fixture;
        
        private readonly AppService _sut;
        
        // services
        
                
        public AppServiceTest
        {
            _fixture = new Fixture();
            
                        
            _sut = _fixture.Create<AppService>();
        }
        
            }
}


