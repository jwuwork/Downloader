using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader.Services
{
    public interface IDownloadService
    {
        ICollection<DownloadDetail> FetchDownloadDetails(string sourceUrl);
    }
}
