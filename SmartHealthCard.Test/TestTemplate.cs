using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartHealthCard.Test
{
  public class TestTemplate
  {

    [Fact]
    public void Test_one()
    {
      //### Prepare ######################################################


      //### Act ##########################################################


      //### Assert #######################################################

      Assert.True(true);
      Assert.Equal(1, 1);
    }

    [Theory]
    [InlineData(1, true, "three")]
    public void Test_two(int x, bool y, string z)
    {
      //### Prepare ######################################################


      //### Act ##########################################################


      //### Assert #######################################################

      Assert.Equal(1, x);
      Assert.True(y);      
      Assert.Equal("three", z);
    }
  }
}
