using System;
using System.ComponentModel;
using NUnit.Framework;

namespace NUnitTest.TestModels
{
    public class User
    {
        [System.ComponentModel.Description("User Id")]
        public int Id { get; set; }

        public string Name { get; set; }

        [System.ComponentModel.DisplayName(displayName:"年龄")]        
        public int Age { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
