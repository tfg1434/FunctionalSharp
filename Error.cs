using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    public record Error(string Message) {
        //peek into IL and find out why point-free wont work
    }
}
