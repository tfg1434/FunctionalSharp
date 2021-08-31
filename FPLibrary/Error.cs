using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    public record Error(string Message) {
        public override string ToString() => $"Error({Message})";
    }
}
