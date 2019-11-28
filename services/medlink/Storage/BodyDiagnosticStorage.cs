using System.Collections.Generic;
using System.IO;
using System.Linq;
using medlink.Helpers;

namespace medlink.Storage
{
    public class BodyDiagnosticStorage : BaseStorage<BodyDiagnosticInfo>, IBodyModelsStorage
    {
        public BodyDiagnosticStorage(ISerializer serializer, ISettings settings) : base(serializer, settings)
        {
        }

        protected override string Folder => Settings.BodyDiagnosticFolder;

        public IEnumerable<string> GetAllModels()
        {
            return new DirectoryInfo(Folder).GetFiles().Select(info => info.Name);
        }
    }
}