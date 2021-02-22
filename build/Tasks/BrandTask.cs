using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.GitVersion;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Spectre.Console;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Build.Tasks
{
    [TaskName("Brand")]
    [TaskDescription("Brands Textrude and TextrudeInteractive")]
    public sealed class BrandTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            // TODO
        }
    }
}
