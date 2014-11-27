using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace wpfcm1.DataAccess
{
    public class DocumentRepository
    {
        public DocumentRepository(string path)
        {
            Files = Directory.EnumerateFiles(path).Select(p => new FileInfo(p));
        }

        public IEnumerable<FileInfo> Files { get; private set; }
    }
}
