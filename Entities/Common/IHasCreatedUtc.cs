using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Common
{
    public interface IHasCreatedUtc
    {
        DateTime CreatedUtc { get; set; }
    }
}
