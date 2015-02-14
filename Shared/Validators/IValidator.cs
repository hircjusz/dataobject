using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public interface IValidator
    {
        bool Validate(string sequence);
    }
}
