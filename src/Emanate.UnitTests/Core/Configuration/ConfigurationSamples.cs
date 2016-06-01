using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Autofac.Features.Indexed;
using Emanate.Core;
using Emanate.Core.Configuration;
using NSubstitute;

namespace Emanate.UnitTests.Core.Configuration
{
    public static class ConfigurationSamples
    {
        public static string Complete =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<emanate>
  <modules>
    <module key=""module1"" type=""input"">
      <devices>
        <device id=""30b0091d-6c5c-4460-8da7-8059a5461a41"" name=""iCare"" uri=""icarehealth"" polling-interval=""30"" username=""JPRoughan"" password=""íõïòêí±û°Õ÷ÌåôâÄìÆµÍ"" />
      </devices>
    </module>
    <module key=""module2"" type=""output"">
      <devices>
        <device id=""59e491aa-58cc-4a50-b6af-0975d8708833"" name=""Carina CI"" physical-device-id=""\\?\hid#vid_0fc5&amp;pid_b080#6&amp;16814d2d&amp;1&amp;0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"" />  
      </devices>
    </module>
  </modules>
  <mappings>
    <mapping output-device-id=""59e491aa-58cc-4a50-b6af-0975d8708833"" >
	  <inputs input-device-id=""30b0091d-6c5c-4460-8da7-8059a5461a41"" >
	    <input input-id=""MyInput"" />
	  </inputs>
	</mapping>
  </mappings>
</emanate>";
    }
}